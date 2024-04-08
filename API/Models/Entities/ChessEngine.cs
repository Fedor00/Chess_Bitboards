using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Entities
{
    public class ChessEngine
    {
        public long Id { get; set; }
        public string EngineName { get; set; }
        public ICollection<Game> EngineGames { get; set; }
    }
}