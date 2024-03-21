using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Entities
{
    public class User
    {
        public long Id { get; set; }
        public IEnumerable<Game> Games { get; set; }
    }
}