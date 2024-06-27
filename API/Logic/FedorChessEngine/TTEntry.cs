using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Logic.FedorChessEngine
{
    public struct TTEntry
    {

        public ulong PositionHash { get; set; }
        public int Depth { get; set; }
        public int Flag { get; set; }
        public int Score { get; set; }
        public int BestMove { get; set; }
    }
}