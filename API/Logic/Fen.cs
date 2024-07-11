using System.Text;
using static API.Utils.BitbboardUtils;
using static API.Utils.ChessHelpers;
namespace API.Logic
{
    public static  class Fen
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
        public static string UpdateFen(Board board)
        {
            StringBuilder fen = new StringBuilder();
            for (int rank = 0; rank < 8; rank++)
            {
                if (rank > 0) fen.Append('/');
                int blankSpaces = 0;

                for (int file = 0; file < 8; file++)
                {
                    int square = rank * 8 + file;
                    bool foundPiece = false;

                    for (int piece = P; piece <= n; piece++)
                    {
                        ulong bit = 1UL << square;
                        if ((board.Bitboards[piece] & bit) != 0)
                        {
                            if (blankSpaces > 0)
                            {
                                fen.Append(blankSpaces);
                                blankSpaces = 0;
                            }
                            fen.Append(AsciiPiece[piece]);
                            foundPiece = true;
                            break;
                        }
                    }

                    if (!foundPiece) blankSpaces++;
                }

                if (blankSpaces > 0) fen.Append(blankSpaces);
            }
            fen.Append(' ').Append(board.Side == White ? 'w' : 'b');
            fen.Append(' ');
            if (board.Castle == 0) fen.Append('-');
            else
            {
                if ((board.Castle & WK) != 0) fen.Append('K');
                if ((board.Castle & WQ) != 0) fen.Append('Q');
                if ((board.Castle & BK) != 0) fen.Append('k');
                if ((board.Castle & BQ) != 0) fen.Append('q');
            }
            fen.Append(' ');
            fen.Append(board.Enpassant != None ? Coordinates[board.Enpassant] : "-");
            fen.Append(' ').Append(board.HalfMoveClock).Append(' ').Append(board.FullMoveNumber);
            return fen.ToString();
        }

    }
}