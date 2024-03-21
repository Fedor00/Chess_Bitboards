using static API.Utils.BitbboardUtils;
using static API.Utils.ChessHelpers;
namespace API.Logic
{
    public class Fen
    {
        public static void InitializeBoard(Board board, string fen)
        {
            board.Bitboards = new ulong[12];
            board.Occuppancy = new ulong[3];
            if (string.IsNullOrEmpty(fen)) throw new ArgumentException("Fen cannot be null or empty");
            string[] parts = fen.Split(' ');
            if (parts.Length < 6) throw new ArgumentException("Fen must contain at least 4 parts");
            string piecePlacement = parts[0];
            string side = parts[1];
            string castle = parts[2];
            string enpassant = parts[3];
            string halfMoveClock = parts[4];
            string fullMoveNumber = parts[5];
            int square = 0;
            foreach (char c in piecePlacement)
            {
                if (char.IsLetter(c))
                {
                    int piece = CharToPiece[c];
                    SetBit(ref board.Bitboards[piece], square++);
                }
                else if (char.IsDigit(c))
                {
                    square += c - '0';
                }
            }
            board.Side = side == "w" ? White : Black;
            if (castle.Contains('K')) board.Castle |= WK;
            if (castle.Contains('Q')) board.Castle |= WQ;
            if (castle.Contains('k')) board.Castle |= BK;
            if (castle.Contains('q')) board.Castle |= BQ;

            if (enpassant != "-")
            {
                int file = enpassant[0] - 'a';
                int rank = 8 - (enpassant[1] - '1') - 1;
                board.Enpassant = rank * 8 + file;
            }
            else board.Enpassant = None;
            if (!int.TryParse(halfMoveClock, out int halfMove))
            {
                halfMove = 0;
            }
            board.HalfMoveClock = halfMove;
            if (!int.TryParse(fullMoveNumber, out int fullMove))
            {
                fullMove = 1;
            }
            board.FullMoveNumber = fullMove;
            board.PreviousCastle = board.Castle;
            board.PreviousEnpassant = board.Enpassant;
            board.UpdateOccupancy();
        }



        // public static string UpdateFen(Board board)
        // {
        //     StringBuilder fen = new StringBuilder();
        //     for (int row = 7; row >= 0; row--) //from a8 to a1
        //     {
        //         int emptyCount = 0;
        //         for (int col = 0; col < 8; col++) //from a to h
        //         {
        //             char piece = board.GetPieceAt(row * 8 + col);

        //             if (piece == 'x') emptyCount++;
        //             else
        //             {
        //                 if (emptyCount > 0)
        //                 {
        //                     fen.Append(emptyCount);
        //                     emptyCount = 0;
        //                 }
        //                 fen.Append(piece);
        //             }
        //         }
        //         if (emptyCount > 0) fen.Append(emptyCount);
        //         if (row > 0) fen.Append('/');

        //     }

        //     fen.Append(' ').Append(board.ActiveColor == Color.White ? 'w' : 'b');

        //     fen.Append(" KQkq - 0 1");

        //     return fen.ToString();
        // }
    }
}