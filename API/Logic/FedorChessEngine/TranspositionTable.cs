using static API.Utils.EvaluationHelpers;

namespace API.Logic.FedorChessEngine
{
    public class TranspositionTable
    {
        private Dictionary<ulong, TTEntry> table;

        public TranspositionTable()
        {
            table = new Dictionary<ulong, TTEntry>();
        }

        public void Store(ulong hashKey, int bestMove, int depth, int score, int flag, int ply)
        {
            // Adjust the score for mates to ensure the depth is taken into account.
            if (score < -MateScore) score -= ply;
            if (score > MateScore) score += ply;

            TTEntry entry = new TTEntry
            {
                PositionHash = hashKey,
                BestMove = bestMove,
                Depth = depth,
                Score = score,
                Flag = flag
            };

            table[hashKey] = entry;
        }

        public int ReadHashEntry(ulong hashKey, int alpha, int beta, int depth, int ply, ref int bestMove)
        {
            if (table.TryGetValue(hashKey, out TTEntry entry))
            {
                if (entry.Depth >= depth && entry.PositionHash == hashKey)
                {
                    int score = entry.Score;
                    if (score < -MateScore) score += ply;
                    if (score > MateScore) score -= ply;
                    if (entry.Flag == HashFlagExact)
                        return score;
                    else if ((entry.Flag == HashFlagAlpha) && score <= alpha)
                        return alpha;
                    else if ((entry.Flag == HashFlagBeta) && score >= beta)
                        return beta;
                }
                bestMove = entry.BestMove;
            }
            return NoHashEntry;
        }
    }
}
