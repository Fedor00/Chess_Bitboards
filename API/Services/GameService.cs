using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;
using API.Logic;
using API.Models.Dtos;
using API.Models.Entities;
using static API.Utils.BitbboardUtils;
using static API.Utils.ChessHelpers;
namespace API.Services
{
    public class GameService
    {
        private readonly IGameRepository _gameRepository;

        public GameService(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }
        public async Task<Game> CreateOrJoinGame(long playerId)
        {
            var playingGame = await GetGame(playerId);
            // if already playing a game or created one and waiting , return that game
            if (playingGame != null) return playingGame;
            var game = await _gameRepository.GetWaitingGame();
            // if there is no game to join create one
            if (game == null) return await CreateGame(playerId);
            // join the waiting game
            game.TopPlayerId = playerId;
            game.Status = "playing";
            _gameRepository.UpdateGame(game);
            await _gameRepository.SaveChangesAsync();
            return game;
        }

        private async Task<Game> CreateGame(long playerId)
        {
            Game game = new()
            {
                Status = "waiting",
                Fen = INITIAL_FEN,
                BottomPlayerId = playerId,
                TopPlayer = null
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
                FullMoveNumber = board.FullMoveNumber
            };
        }
        public async Task<MoveDto> MakeMove(long playerId, Move move)
        {
            Game game = await GetGame(playerId);
            if (game == null) throw new ArgumentException("Player is not in a game");
            if (playerId == game.TopPlayerId) move = ReverseMove(move);
            Board board = new(game.Fen);
            if (board.Side == White && game.BottomPlayerId != playerId
            || board.Side == Black && game.TopPlayerId != playerId)
                throw new ArgumentException("It's not your turn");
            int moveCount = 0;
            int[] moves = board.GenerateLegalMoves(ref moveCount);
            int moveMade = 0;
            for (int i = 0; i < moveCount; i++)
            {
                if (GetMoveSource(moves[i]) == move.From && GetMoveTarget(moves[i]) == move.To)
                {
                    moveMade = moves[i];
                    board.MakeMove(moves[i], AllMoves);
                    game.Fen = Fen.UpdateFen(board);

                    break;
                }
            }
            GameDto gameDto = GetGameDto(game);
            bool check = board.IsInCheck();
            bool outOfMoves = board.Side == White ? gameDto.WhiteMoves.Count() == 0 : gameDto.BlackMoves.Count() == 0;
            bool checkMate = check && outOfMoves;
            bool stalement = !check && outOfMoves;
            bool draw = board.IsDraw();
            if (draw) game.Status = "draw";
            if (checkMate) game.Status = "checkmate";
            if (stalement) game.Status = "stalemate";
            _gameRepository.UpdateGame(game);
            await _gameRepository.SaveChangesAsync();
            return CreateMoveDto(gameDto, moveMade, check, checkMate, stalement, draw);

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
            return await _gameRepository.GetGameForPlayerAsync(playerId);
        }
    }
}
