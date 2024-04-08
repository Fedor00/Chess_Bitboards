using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Logic;
using API.Models.Dtos;
using API.Models.Entities;
using static API.Utils.BitbboardUtils;
namespace API.Services
{
    public class MapToDto
    {
        public static MoveDto CreateMoveDto(GameDto gameDto, int move, bool check, bool checkMate, bool stalement, bool draw)
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
        public static GameDto CreateGameDto(Game game, Board board, IEnumerable<Move> currentPlayerMoves, IEnumerable<Move> opponentMoves)
        {
            return new GameDto
            {
                Id = game.Id.ToString(),
                FirstPlayer = game.FirstPlayer,
                SecondPlayer = game.SecondPlayer,
                IsFirstPlayerWhite = game.IsFirstPlayerWhite,
                IsWhiteTurn = board.Side == White,
                Pieces = board.TransformToPieces(),
                BlackMoves = board.Side == Black ? currentPlayerMoves : opponentMoves,
                WhiteMoves = board.Side == White ? currentPlayerMoves : opponentMoves,
                HalfMoveClock = board.HalfMoveClock,
                FullMoveNumber = board.FullMoveNumber,
                Status = game.Status,
            };
        }
    }
}