using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;
using static API.Utils.BitbboardUtils;

namespace API.Models.Dtos
{
    public class GameDto
    {
        public char[][] Pieces { get; set; }
        public int Side { get; set; }
        public int Castle { get; set; }
        public int Enpassant { get; set; }
        public int PreviosEnpassant { get; set; }
        public int HalfmoveClock { get; set; }
        public int FullmoveNumber { get; set; }
    }
}