using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Entities;

namespace API.Models.Dtos
{
    public class MoveDto
    {
        public GameDto GameDto { get; set; }
        public bool IsCapture { get; set; }
        public bool IsCheck { get; set; }
        public bool IsCheckmate { get; set; }
        public bool IsStalemate { get; set; }
        public bool IsDraw { get; set; }
        public bool IsPromotion { get; set; }
        public bool IsCastling { get; set; }

    }
}