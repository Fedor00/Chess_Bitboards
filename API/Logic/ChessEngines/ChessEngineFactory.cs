using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;
using static API.Utils.ChessHelpers;
namespace API.Logic.ChessEngines
{
    public class ChessEngineFactory : IChessEngineFactory
    {
        public IChessEngine CreateEngine(string engineName)
        {
            switch (engineName.Trim().ToLower())
            {
                case StockfishEngineName:
                    return new StockfishEngine();
                case DummyEngineName:
                    return new DummyEngine();
                default:
                    throw new ArgumentException("Unsupported engine ID", nameof(engineName));
            }
        }
    }
}