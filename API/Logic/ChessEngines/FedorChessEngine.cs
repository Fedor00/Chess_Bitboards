using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;
using API.Logic.FedorChessEngine;
using static API.Utils.BitbboardUtils;
using static API.Utils.Mask;
using static API.Utils.ChessHelpers;

namespace API.Logic.ChessEngines
{
    public class FedorChessEngine : IChessEngine
    {
        public Task<string> GetBestMoveAsync(string fen, int depth)
        {
            Board board = new Board(fen);

            var search = new Search();
            Console.WriteLine(search.SearchPosition(board, depth));
            Console.WriteLine("NMode: " + search.Nodes);
            return Task.FromResult(MoveToString(search.BestMove));
        }
    }
}