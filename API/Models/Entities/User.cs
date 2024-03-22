using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Entities
{
    public class User
    {
        public long Id { get; set; }
        // games as black
        public ICollection<Game> TopPlayerGames { get; set; }
        // games as white
        public ICollection<Game> BottomPlayerGames { get; set; }
    }

}