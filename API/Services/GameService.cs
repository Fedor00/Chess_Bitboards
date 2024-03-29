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
using static API.Utils.BitbboardUtils;
using static API.Utils.ChessHelpers;
namespace API.Services
{
    public class GameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IHubContext<ChessHub> _hubContext;

        public GameService(IGameRepository gameRepository, IHubContext<ChessHub> hubContext)
        {
            _gameRepository = gameRepository;
            _hubContext = hubContext;
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
                return GetGameDto(await CreateGame(playerId, createGameDto));
            }
            game.TopPlayerId = playerId;
            game.Status = Playing;
            game.StartTime = DateTime.UtcNow;
            _gameRepository.UpdateGame(game);
            GameDto gameDto = GetGameDto(game);
            await _hubContext.Clients.User(game.BottomPlayerId.ToString()).SendAsync("GameStarted", gameDto);
            await _gameRepository.SaveChangesAsync();
            return gameDto;
        }

        public async Task<Game> CreateGame(long playerId, CreateGameDto createGameDto)
        {
            var game = new Game
            {
                BottomPlayerId = playerId,
                Status = Waiting,
                Fen = INITIAL_FEN,
                IsPrivate = createGameDto.IsPrivate,
            };
            await _gameRepository.AddGameAsync(game);
            await _gameRepository.SaveChangesAsync();
            return game;
        }
        public GameDto GetGameDto(Game game)
        {
            Board board = new(game.Fen);
            int moveCount = 0;
            // get all legal moves for the current player
            int[] moves = board.GenerateLegalMoves(ref moveCount);
            IEnumerable<Move> firstMoves = moves.Take(moveCount).Select(m => new Move { From = GetMoveSource(m), To = GetMoveTarget(m) });

            //reset enpassant since generating moves for the other player 
            board.Side ^= 1;
            board.Enpassant = None;
            moveCount = 0;
            // get all legal moves for the other player
            moves = board.GenerateLegalMoves(ref moveCount);
            IEnumerable<Move> secondMoves = moves.Take(moveCount).Select(m => new Move { From = GetMoveSource(m), To = GetMoveTarget(m) });
            return new GameDto
            {
                Id = game.Id,
                Pieces = board.TransformToPieces(),
                BlackMoves = board.Side == White ? firstMoves : secondMoves,
                WhiteMoves = board.Side == White ? secondMoves : firstMoves,
                TopPlayerId = game.TopPlayerId,
                BottomPlayerId = game.BottomPlayerId,
                Status = game.Status,
                HalfMoveClock = board.HalfMoveClock,
                FullMoveNumber = board.FullMoveNumber,

            };
        }
        public async Task<MoveDto> MakeMove(long playerId, Move move)
        {
            var game = await GetGame(playerId);
            if (game == null) throw new ArgumentException("Player is not in a game");
            if (game.Status != Playing) throw new InvalidOperationException("Game is not in progress");
            if (playerId == game.TopPlayerId) move = ReverseMove(move);

            var board = new Board(game.Fen);
            if (board.Side == White && game.BottomPlayerId != playerId
            || board.Side == Black && game.TopPlayerId != playerId)
                throw new ArgumentException("It's not your turn");
            var moveCount = 0;
            var moves = board.GenerateLegalMoves(ref moveCount);
            var moveMade = 0;

            for (var i = 0; i < moveCount; i++)
            {
                if (GetMoveSource(moves[i]) == move.From && GetMoveTarget(moves[i]) == move.To)
                {
                    moveMade = moves[i];
                    board.MakeMove(moves[i], AllMoves);
                    game.Fen = Fen.UpdateFen(board);
                    break;
                }
            }

            var gameDto = GetGameDto(game);
            var check = board.IsInCheck();
            var outOfMoves = board.Side == White ? gameDto.WhiteMoves.Count() == 0 : gameDto.BlackMoves.Count() == 0;
            var checkMate = check && outOfMoves;
            var stalemate = !check && outOfMoves;
            var draw = board.IsDraw();

            if (draw) game.Status = Draw;
            if (checkMate) game.Status = board.Side == White ? BlackWin : WhiteWin;
            if (stalemate) game.Status = Stalemate;

            _gameRepository.UpdateGame(game);
            await _gameRepository.SaveChangesAsync();
            MoveDto moveDto = CreateMoveDto(gameDto, moveMade, check, checkMate, stalemate, draw);
            await _hubContext.Clients.User(game.BottomPlayerId.ToString()).SendAsync("MoveMade", moveDto);
            await _hubContext.Clients.User(game.TopPlayerId.ToString()).SendAsync("MoveMade", moveDto);
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
            game.TopPlayerId = playerId;
            game.Status = Playing;
            game.StartTime = DateTime.UtcNow;
            _gameRepository.UpdateGame(game);
            GameDto gameDto = GetGameDto(game);
            await _hubContext.Clients.User(game.BottomPlayerId.ToString()).SendAsync("GameStarted", gameDto);
            await _gameRepository.SaveChangesAsync();
            return gameDto;
        }
        private Move ReverseMove(Move move)
        {
            return new Move { From = h1 - move.From, To = h1 - move.To };
        }
        private MoveDto CreateMoveDto(GameDto gameDto, int move, bool check, bool checkMate, bool stalement, bool draw)
        {
            return new MoveDto
            {
                IsCapture = IsMoveCapture(move),
                IsCheck = check,
                IsCheckmate = checkMate,
                IsStalemate = stalement,
                IsDraw = draw,
                IsPromotion = GetMovePromoted(move) != 0,
                IsCastling = IsMoveCastle(move),
                GameDto = gameDto
            };

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
    }
}
