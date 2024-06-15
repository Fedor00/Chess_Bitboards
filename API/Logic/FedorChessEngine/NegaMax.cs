using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static API.Utils.BitbboardUtils;
namespace API.Logic.FedorChessEngine
{
    public class NegaMax
    {
        public int Nodes { get; set; }
        public int HalfMoves { get; set; }
        public int BestMove { get; set; }

        public int NegaMaxSearch(Board board, int depth, int alpha, int beta)
        {
            if (depth == 0)
            {
                int eval = Evaluation.EvaluationScore(board);
                return eval;
            }

            Nodes++;
            int oldAlpha = alpha;
            int moveCount = 0;
            int[] moves = board.GenerateLegalMoves(ref moveCount);
            int bestSofar = moves[0];

            for (int count = 0; count < moveCount; count++)
            {
                int castle = board.Castle;

                board.MakeMove(moves[count], AllMoves);
                HalfMoves++;

                int score = -NegaMaxSearch(board, depth - 1, -beta, -alpha);

                board.UnmakeMove(moves[count], AllMoves);
                board.Castle = castle;
                HalfMoves--;


                if (score >= beta)
                {
                    return beta;
                }

                if (score > alpha)
                {
                    alpha = score;
                    if (HalfMoves == 0)
                    {
                        bestSofar = moves[count];
                    }
                }
            }
            if (moveCount == 0)
            {
                ulong kingBitboard = board.Bitboards[board.Side == White ? K : k];
                if (board.IsSquareAttacked(GetLSBIndex(kingBitboard), board.Side ^ 1))
                {
                    return -49000 + HalfMoves;
                }
                else
                {
                    return 0;
                }
            }
            if (oldAlpha != alpha)
            {
                BestMove = bestSofar;
            }

            return alpha;
        }
    }

}