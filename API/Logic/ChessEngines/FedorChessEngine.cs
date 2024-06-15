using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;
using API.Logic.FedorChessEngine;
using static API.Utils.BitbboardUtils;

namespace API.Logic.ChessEngines
{
    public class FedorChessEngine : IChessEngine
    {
        public Task<string> GetBestMoveAsync(string fen)
        {
            Board board = new Board(fen);
            int moveCount = 0;
            int[] moves = board.GenerateLegalMoves(ref moveCount);
            if (moveCount == 0) return null;
            var negamax = new NegaMax();
            negamax.NegaMaxSearch(board, 5, -50000, 50000);
            return Task.FromResult(MoveToString(negamax.BestMove));
        }
    }
}