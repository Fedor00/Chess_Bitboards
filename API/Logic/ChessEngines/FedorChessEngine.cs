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
            //Board boardFen = new Board("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1");
            //Console.WriteLine("perf test" + search.Perftest(boardFen, 5));
            Console.WriteLine(search.SearchPosition(board, depth));
            return Task.FromResult(MoveToString(search.BestMove));
        }
    }
}