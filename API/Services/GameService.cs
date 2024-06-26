using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Hubs;
using API.Interfaces;
using API.Logic;
using API.Logic.ChessEngines;
using API.Models.Dtos;
using API.Models.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using UserMicroservice.Interfaces;
using static API.Utils.BitbboardUtils;
using static API.Utils.ChessHelpers;
namespace API.Services
{
    public class GameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IChessEngineRepository _chessEngineRepository;
        private readonly ChessEngineService _chessEngineService;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<ChessHub> _hubContext;

        public GameService(IGameRepository gameRepository, IHubContext<ChessHub> hubContext, IUserRepository userRepository, IChessEngineRepository chessEngineRepository, ChessEngineService chessEngineService)
        {
            _gameRepository = gameRepository;
            _hubContext = hubContext;
            _userRepository = userRepository;
            _chessEngineRepository = chessEngineRepository;
            _chessEngineService = chessEngineService;
        }

        public async Task<GameDto> MatchGame(long playerId, CreateGameDto createGameDto)
        {
            var existingGame = await _gameRepository.GetGameForPlayerAsync(playerId);
            var secondPlayer = await _userRepository.GetUserByIdAsync(playerId);
            if (secondPlayer == null) throw new ArgumentException("Player not found.");
            if (existingGame != null)
            {
                throw new InvalidOperationException("Player is already in a game.");
            }
            var game = await _gameRepository.GetMatchingGame(createGameDto.IsPrivate);
            if (game == null)
            {
                game = await CreateGame(playerId, createGameDto);
                var _board = new Board(game.Fen);
                //[0] is for current player, [1] is for opponent
                var _moves = _board.GenerateMovesForBothSides();
                return MapToDto.CreateGameDto(game, _board, _moves[0], _moves[1]);
            }
            game.SecondPlayerId = playerId;
            game.Status = Playing;
            game.StartTime = DateTime.UtcNow;
            await _gameRepository.UpdateGame(game);
            game.SecondPlayer = secondPlayer;
            var board = new Board(game.Fen);
            var moves = board.GenerateMovesForBothSides();
            var gameDto = MapToDto.CreateGameDto(game, board, moves[0], moves[1]);
            await _hubContext.Clients.User(game.FirstPlayerId.ToString()).SendAsync("GameStarted", gameDto);
            return gameDto;
        }

        public async Task<Game> CreateGame(long playerId, CreateGameDto createGameDto)
        {
            var firstUser = await _userRepository.GetUserByIdAsync(playerId);
            if (firstUser == null) throw new ArgumentException("User not found.");
            var game = new Game
            {
                FirstPlayerId = playerId,
                IsFirstPlayerWhite = new Random().Next(0, 2) == 0,
                Status = Waiting,
                Fen = INITIAL_FEN,
                IsPrivate = createGameDto.IsPrivate,
            };

            await _gameRepository.AddGameAsync(game);
            game.FirstPlayer = firstUser;
            return game;
        }
        public async Task<Game> CreateEngineGame(long playerId, CreateEngineGameDto createGameDto)
        {
            var existingGame = await _gameRepository.GetGameForPlayerAsync(playerId);
            if (existingGame != null) throw new InvalidOperationException("Player is already in a game.");
            var firstUser = await _userRepository.GetUserByIdAsync(playerId);
            var engine = await _chessEngineRepository.GetChessEngineByNameAsync(createGameDto.EngineName);
            if (engine == null) throw new ArgumentException("Engine not found.");
            if (firstUser == null) throw new ArgumentException("User not found.");
            if (createGameDto.Depth < 1 || createGameDto.Depth > 20) createGameDto.Depth = 4;
            var game = new Game
            {
                FirstPlayerId = playerId,
                EngineId = engine.Id,
                IsFirstPlayerWhite = new Random().Next(0, 2) == 0,
                Status = Playing,
                Fen = INITIAL_FEN,
                StartTime = DateTime.UtcNow,
                IsPrivate = true,
                EngineDepth = createGameDto.Depth
            };
            await _gameRepository.AddGameAsync(game);
            game.Engine = engine;
            game.FirstPlayer = firstUser;
            var board = new Board(game.Fen);
            if (IsEnginesTurn(game, board))
            {
                var engineFactory = new ChessEngineFactory();
                var chessEngine = engineFactory.CreateEngine(game.Engine.EngineName);
                _ = Task.Run(() => _chessEngineService.MakeEngineMove(chessEngine, game, board));

            }
            return game;
        }
        public async Task<MoveDto> MakeMove(long playerId, Move move)
        {
            var game = await GetGame(playerId);
            if (game == null) throw new ArgumentException("Player is not in a game");
            if (game.Status != Playing) throw new InvalidOperationException("Game is not in progress");
            if (IsBlacksTurn(game, playerId)) move = ReverseMove(move);
            var board = new Board(game.Fen);
            if (!IsPlayerTurn(game, new Board(game.Fen), playerId)) throw new ArgumentException("It's not your turn");
            var moveCount = 0;
            var moves = board.GenerateLegalMoves(ref moveCount);
            var moveToMake = moves.FirstOrDefault(m => GetMoveSource(m) == move.From && GetMoveTarget(m) == move.To);
            if (moveToMake == 0) throw new ArgumentException("Invalid move");
            board.MakeMove(moveToMake, AllMoves);
            game.Fen = Fen.UpdateFen(board); //update fen
            var bothLegalMoves = board.GenerateMovesForBothSides();
            var check = board.IsInCheck();
            var outOfMoves = bothLegalMoves[0].Count() == 0;
            var checkMate = check && outOfMoves;
            var stalemate = !check && outOfMoves;
            var draw = board.IsDraw();

            if (draw) { game.Status = Draw; }
            if (checkMate) { game.Status = board.Side == White ? BlackWin : WhiteWin; }
            if (stalemate) { game.Status = Stalemate; }

            await _gameRepository.UpdateGame(game);
            var gameDto = MapToDto.CreateGameDto(game, board, bothLegalMoves[0], bothLegalMoves[1]);
            var moveDto = MapToDto.CreateMoveDto(gameDto, moveToMake, check, checkMate, stalemate, draw);
            await _hubContext.Clients.User(game.FirstPlayerId.ToString()).SendAsync("MoveMade", moveDto);
            await _hubContext.Clients.User(game.SecondPlayerId.ToString()).SendAsync("MoveMade", moveDto);
            var engineFactory = new ChessEngineFactory();
            var chessEngine = engineFactory.CreateEngine(game.Engine.EngineName);
            _ = Task.Run(() => _chessEngineService.MakeEngineMove(chessEngine, game, board));
            return moveDto;

        }
        public async Task<Game> CreatePrivateGame(long playerId, CreateGameDto createGameDto)
        {
            var playingGame = await GetGame(playerId);
            if (playingGame != null) return playingGame;
            return await CreateGame(playerId, createGameDto);
        }
        public async Task<GameDto> JoinPrivateGame(string gameId, long playerId)
        {
            var game = await _gameRepository.GetGameAsync(gameId);
            if (game == null)
            {
                throw new ArgumentException("Game not found.");
            }
            if (game.Status != Waiting || game.IsPrivate == false)
            {
                throw new InvalidOperationException("Game is not joinable.");
            }
            var secondPlayer = await _userRepository.GetUserByIdAsync(playerId);
            if (secondPlayer == null) throw new ArgumentException("Player not found.");

            game.SecondPlayerId = secondPlayer.Id;
            game.Status = Playing;
            game.StartTime = DateTime.UtcNow;
            await _gameRepository.UpdateGame(game);
            game.SecondPlayer = secondPlayer;
            var board = new Board(game.Fen);
            var moves = board.GenerateMovesForBothSides();
            var gameDto = MapToDto.CreateGameDto(game, board, moves[0], moves[1]);
            await _hubContext.Clients.User(game.FirstPlayerId.ToString()).SendAsync("GameStarted", gameDto);

            return gameDto;
        }

        public async Task<bool> ResignGame(long playerId)
        {
            var playingGame = await GetGame(playerId);
            if (playingGame == null) return false;
            if (playingGame.Status != Playing) return false;
            playingGame.Status = playingGame.SecondPlayerId == playerId ? BlackResign : WhiteResign;
            await _gameRepository.UpdateGame(playingGame);

            await _hubContext.Clients.User(playingGame.FirstPlayerId.ToString()).SendAsync("GameResigned", playingGame.Status);
            await _hubContext.Clients.User(playingGame.SecondPlayerId.ToString()).SendAsync("GameResigned", playingGame.Status);
            return true;
        }

        public async Task<Game> GetGame(long playerId)
        {
            var game = await _gameRepository.GetGameForPlayerAsync(playerId);
            return game;
        }
        public async Task<GameDto> GetCurrentGameForPlayer(long playerId)
        {
            var game = await GetGame(playerId);
            if (game == null) throw new ArgumentException("Player is not in a game");
            var board = new Board(game.Fen);
            if (IsEnginesTurn(game, board))
            {
                var engineFactory = new ChessEngineFactory();
                var chessEngine = engineFactory.CreateEngine(game.Engine.EngineName);
                _ = Task.Run(() => _chessEngineService.MakeEngineMove(chessEngine, game, board));
            }

            var moves = board.GenerateMovesForBothSides();
            return MapToDto.CreateGameDto(game, board, moves[0], moves[1]);
        }

        public async Task DeleteGame(string gameId)
        {
            await _gameRepository.DeleteGame(gameId);

        }
        private bool IsEnginesTurn(Game game, Board board)
        {
            if (game.EngineId != null)
            {
                bool isEnginesTurn = (game.SecondPlayerId == null) &&
                                     ((board.Side == White && !game.IsFirstPlayerWhite) ||
                                      (board.Side == Black && game.IsFirstPlayerWhite));
                if (isEnginesTurn)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsPlayerTurn(Game game, Board board, long playerId)
        {
            if (IsEnginesTurn(game, board)) return false;
            bool isPlayerWhite = (playerId == game.FirstPlayerId && game.IsFirstPlayerWhite) ||
                                 (playerId == game.SecondPlayerId && !game.IsFirstPlayerWhite);
            bool isPlayerBlack = !isPlayerWhite;
            bool isWhitesTurn = board.Side == White;
            bool isBlacksTurn = !isWhitesTurn;
            return (isWhitesTurn && isPlayerWhite) || (isBlacksTurn && isPlayerBlack);
        }

        private bool IsBlacksTurn(Game game, long playerId)
        {
            if (game.EngineId != null)
            {
                return !game.IsFirstPlayerWhite;
            }
            return game.IsFirstPlayerWhite && playerId == game.SecondPlayerId || !game.IsFirstPlayerWhite && playerId == game.FirstPlayerId;
        }
        private Move ReverseMove(Move move)
        {
            return new Move { From = h1 - move.From, To = h1 - move.To };
        }
    }
}
