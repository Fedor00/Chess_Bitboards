using API.Models.Dtos;
using static API.Utils.BitbboardUtils;

namespace API.Logic
{
    public class G
    {
        public Board Board { get; set; }
        public const string INITIAL_FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        public string CurrentFen { get; set; }
        public G(string fen)
        {
            Board = new Board(fen);
        }

        public G()
        {
            Board = new Board(INITIAL_FEN);
        }
        public ulong GetAllPossibleMoves(int depth)
        {
            return GetAllPossibleMovesHelper(depth);
        }

        private ulong GetAllPossibleMovesHelper(int depth)
        {
            if (depth == 0)
            {
                return 1;
            }
            ulong positions = 0;
            int moveCount = 0;
            int[] moves = Board.GenerateLegalMoves(ref moveCount);
            for (int i = 0; i < moveCount; i++)
            {
                int castle = Board.Castle;
                Board.MakeMove(moves[i], AllMoves);
                ulong tempPos = GetAllPossibleMovesHelper(depth - 1);
                Board.Castle = castle;
                Board.UnmakeMove(moves[i], AllMoves);
                positions += tempPos;
            };

            return positions;
        }


        public GameDto TransformToGameDto()
        {
            return new GameDto
            {
                Pieces = Board.TransformToPieces(),
                Side = Board.Side,
                Castle = Board.Castle,
                Enpassant = Board.Enpassant,
                HalfmoveClock = Board.HalfMoveClock,
                FullmoveNumber = Board.FullMoveNumber,
                PreviosEnpassant = Board.PreviousEnpassant
            };
        }

    }
}