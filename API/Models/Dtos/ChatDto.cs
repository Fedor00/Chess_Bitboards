using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Entities;

namespace API.Models.Dtos
{
    public class ChatDto
    {
        public User FirstPlayer { get; set; }
        public User SecondPlayer { get; set; }
        public IEnumerable<ChatMessage> ChatMessages { get; set; }

    }
}