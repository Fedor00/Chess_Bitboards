using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Entities
{
    public class ChatMessage
    {
        public long Id { get; set; }
        public string GameId { get; set; }

        public long SenderId { get; set; }
        public User Sender { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public Game Game { get; set; }

    }
}