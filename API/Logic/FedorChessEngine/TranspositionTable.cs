using static API.Utils.EvaluationHelpers;
namespace API.Logic.FedorChessEngine
{
    public class TranspositionTable
    {
        private TTEntry?[] table;
        private int size;

        public TranspositionTable(int size = 1_000_00000) // Default size, can be adjusted based on memory availability
        {
            this.size = size;
            table = new TTEntry?[size];
        }

        private int ComputeIndex(ulong hash)
        {
            // Simple modulo operation for hash indexing
            return (int)(hash % (ulong)size);
        }

        public void Store(ulong hash, int bestMove, int depth, int score, int flag, int ply)
        {
            int index = ComputeIndex(hash);
            if (score < -MateScore) score -= ply;
            if (score > MateScore) score += ply;
            var entry = new TTEntry
            {
                PositionHash = hash,
                BestMove = bestMove,
                Depth = depth,
                Score = score,
                Flag = flag
            };
            table[index] = entry; // Store or overwrite the entry
        }

        public TTEntry? Retrieve(ulong hash)
        {
            int index = ComputeIndex(hash);
            return table[index]; // Returns null if no entry exists
        }

        public int ReadHashEntry(int alpha, int beta, int depth, ulong hash_key, int ply)
        {
            TTEntry? entry = Retrieve(hash_key);
            if (entry != null)
            {
                int score = entry.Value.Score;
                if (score < -MateScore) score += ply;
                if (score > MateScore) score -= ply;
                // Check if the depth and flag conditions are met
                if (entry.Value.Depth >= depth)
                {
                    if (entry.Value.Flag == HashFlagExact)
                    {
                        return score;
                    }
                    else if (entry.Value.Flag == HashFlagAlpha && score <= alpha)
                    {
                        return alpha;
                    }
                    else if (entry.Value.Flag == HashFlagBeta && score >= beta)
                    {
                        return beta;
                    }
                }
            }
            return int.MinValue; // Return null when no relevant entry is found
        }
    }
}
