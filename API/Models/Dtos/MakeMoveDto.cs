using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Dtos
{
    public class MakeMoveDto
    {
        public int From { get; set; }
        public int To { get; set; }
        public char Promotion { get; set; }
    }
}