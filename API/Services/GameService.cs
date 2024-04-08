using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Hubs;
using API.Interfaces;
using API.Logic;
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
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<ChessHub> _hubContext;

        public GameService(IGameRepository gameRepository, IHubContext<ChessHub> hubContext, IUserRepository userRepository)
        {
            _gameRepository = gameRepository;
            _hubContext = hubContext;
            _userRepository = userRepository;
        }

        public async Task<GameDto> MatchGame(long playerId, CreateGameDto createGameDto)
        {
            var existingGame = await _gameRepository.GetGameForPlayerAsync(playerId);
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
            game.SecondPlayer = await _userRepository.GetUserByIdAsync(playerId);
            game.Status = Playing;
            game.StartTime = DateTime.UtcNow;
            _gameRepository.UpdateGame(game);
            var board = new Board(game.Fen);
            var moves = board.GenerateMovesForBothSides();
            var gameDto = MapToDto.CreateGameDto(game, board, moves[0], moves[1]);
            await _gameRepository.SaveChangesAsync();
            await _hubContext.Clients.User(game.FirstPlayerId.ToString()).SendAsync("GameStarted", gameDto);
            return gameDto;
        }

        public async Task<Game> CreateGame(long playerId, CreateGameDto createGameDto)
        {
            var game = new Game
            {
                FirstPlayer = await _userRepository.GetUserByIdAsync(playerId),
                IsFirstPlayerWhite = new Random().Next(0, 2) == 0,
                Status = Waiting,
                Fen = INITIAL_FEN,
                IsPrivate = createGameDto.IsPrivate,
            };
            await _gameRepository.AddGameAsync(game);
            await _gameRepository.SaveChangesAsync();
            return game;
        }
        public async Task<MoveDto> MakeMove(long playerId, Move move)
        {
            var game = await GetGame(playerId);
            if (game == null) throw new ArgumentException("Player is not in a game");
            if (game.Status != Playing) throw new InvalidOperationException("Game is not in progress");
            var isBlack = game.IsFirstPlayerWhite && playerId == game.SecondPlayerId || !game.IsFirstPlayerWhite && playerId == game.FirstPlayerId;
            if (isBlack) move = ReverseMove(move);
            var board = new Board(game.Fen);
            if (!IsPlayerTurn(game, new Board(game.Fen), playerId)) throw new ArgumentException("It's not your turn");
            var moveCount = 0;
            var moves = board.GenerateLegalMoves(ref moveCount);
            var moveToMake = moves.FirstOrDefault(m => GetMoveSource(m) == move.From && GetMoveTarget(m) == move.To);
            if (moveToMake == 0) throw new ArgumentException("Invalid move");
            board.MakeMove(moveToMake, AllMoves);
            game.Fen = Fen.UpdateFen(board); //update fen
            var bothLegalMoves = board.GenerateMovesForBothSides();
            Console.WriteLine("Current Side: " + board.Side + " " + (board.Side == White ? "White" : "Black"));
            Console.WriteLine("Legal Moves: " + bothLegalMoves[0].Count() + " " + bothLegalMoves[1].Count());
            var check = board.IsInCheck();
            var outOfMoves = bothLegalMoves[0].Count() == 0;
            var checkMate = check && outOfMoves;
            var stalemate = !check && outOfMoves;
            var draw = board.IsDraw();
            Console.WriteLine("Check: " + check + " Out of Moves: " + outOfMoves + " Checkmate: " + checkMate + " Stalemate: " + stalemate + " Draw: " + draw);

            if (draw) { game.Status = Draw; }
            if (checkMate) { game.Status = board.Side == White ? BlackWin : WhiteWin; }
            if (stalemate) { game.Status = Stalemate; }

            _gameRepository.UpdateGame(game);
            await _gameRepository.SaveChangesAsync();
            var gameDto = MapToDto.CreateGameDto(game, board, bothLegalMoves[0], bothLegalMoves[1]);
            var moveDto = MapToDto.CreateMoveDto(gameDto, moveToMake, check, checkMate, stalemate, draw);
            await _hubContext.Clients.User(game.FirstPlayerId.ToString()).SendAsync("MoveMade", moveDto);
            await _hubContext.Clients.User(game.SecondPlayerId.ToString()).SendAsync("MoveMade", moveDto);
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

            game.SecondPlayer = secondPlayer;
            game.Status = Playing;
            game.StartTime = DateTime.UtcNow;
            _gameRepository.UpdateGame(game);
            var board = new Board(game.Fen);
            var moves = board.GenerateMovesForBothSides();
            var gameDto = MapToDto.CreateGameDto(game, board, moves[0], moves[1]);
            await _hubContext.Clients.User(game.FirstPlayerId.ToString()).SendAsync("GameStarted", gameDto);
            await _gameRepository.SaveChangesAsync();
            return gameDto;
        }

        public async Task<bool> ResignGame(long playerId)
        {
            var playingGame = await GetGame(playerId);
            if (playingGame == null) return false;
            if (playingGame.Status != Playing) return false;
            playingGame.Status = playingGame.SecondPlayerId == playerId ? BlackResign : WhiteResign;
            _gameRepository.UpdateGame(playingGame);
            await _gameRepository.SaveChangesAsync();
            await _hubContext.Clients.User(playingGame.FirstPlayerId.ToString()).SendAsync("GameResigned", playingGame.Status);
            await _hubContext.Clients.User(playingGame.SecondPlayerId.ToString()).SendAsync("GameResigned", playingGame.Status);
            return true;
        }

        public async Task<Game> GetGame(long playerId)
        {
            var game = await _gameRepository.GetGameForPlayerAsync(playerId);
            return game;
        }
        public async Task DeleteGame(string gameId)
        {
            await _gameRepository.DeleteGame(gameId);
            await _gameRepository.SaveChangesAsync();
        }
        private bool IsPlayerTurn(Game game, Board board, long playerId)
        {
            bool isPlayerWhite = (game.IsFirstPlayerWhite && playerId == game.FirstPlayerId) || (!game.IsFirstPlayerWhite && playerId == game.SecondPlayerId);
            bool isPlayerBlack = (game.IsFirstPlayerWhite && playerId == game.SecondPlayerId) || (!game.IsFirstPlayerWhite && playerId == game.FirstPlayerId);
            bool isWhitesTurn = board.Side == White;
            bool isBlacksTurn = board.Side == Black;

            if ((isWhitesTurn && !isPlayerWhite) || (isBlacksTurn && !isPlayerBlack))
            {
                return false;
            }
            return true;
        }
        private Move ReverseMove(Move move)
        {
            return new Move { From = h1 - move.From, To = h1 - move.To };
        }
    }
}
