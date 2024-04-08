using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;

namespace API.Logic.ChessEngines
{
    public class ChessEngineFactory : IChessEngineFactory
    {
        public IChessEngine CreateEngine(string engineName)
        {
            switch (engineName)
            {
                case "stockfish":
                    return new StockfishEngine();
                default:
                    throw new ArgumentException("Unsupported engine ID", nameof(engineName));
            }
        }
    }
}