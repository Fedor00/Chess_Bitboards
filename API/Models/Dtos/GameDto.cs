using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;
using API.Logic;
using static API.Utils.BitbboardUtils;

namespace API.Models.Dtos
{
    public class GameDto
    {
        public string Id { get; set; }
        public long? TopPlayerId { get; set; }
        public long? BottomPlayerId { get; set; }
        public char[][] Pieces { get; set; }
        public IEnumerable<Move> BlackMoves { get; set; }
        public IEnumerable<Move> WhiteMoves { get; set; }
        public int HalfMoveClock { get; set; }
        public int FullMoveNumber { get; set; }
        public string Status { get; set; }



    }
}