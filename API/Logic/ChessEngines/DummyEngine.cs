using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;
using static API.Utils.BitbboardUtils;
namespace API.Logic.ChessEngines
{
    public class DummyEngine : IChessEngine
    {
        public Task<string> GetBestMoveAsync(string fen, int depth)
        {
            Board board = new Board(fen);
            int moveCount = 0;
            int[] moves = board.GenerateLegalMoves(ref moveCount);
            if (moveCount == 0) return null;
            Random random = new Random();
            int randomMoveIndex = random.Next(0, moveCount);
            int randomMove = moves[randomMoveIndex];
            return Task.FromResult(MoveToString(randomMove));
        }
    }
}