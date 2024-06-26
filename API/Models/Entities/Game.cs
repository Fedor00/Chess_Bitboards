using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Entities
{
    public class Game
    {
        public Game()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        public long? FirstPlayerId { get; set; }
        public long? SecondPlayerId { get; set; }
        public long? EngineId { get; set; }
        public bool IsFirstPlayerWhite { get; set; }
        public User FirstPlayer { get; set; }
        public User SecondPlayer { get; set; }
        public ChessEngine Engine { get; set; }
        public string Fen { get; set; }
        public string Status { get; set; }
        public DateTime StartTime { get; set; }
        public bool IsPrivate { get; set; }
        public int EngineDepth { get; set; }
        public ICollection<ChatMessage> ChatMessages { get; set; }
    }
}