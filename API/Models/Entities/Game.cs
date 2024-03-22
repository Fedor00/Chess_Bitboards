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
        public long? TopPlayerId { get; set; }
        public long? BottomPlayerId { get; set; }
        public User TopPlayer { get; set; }
        public User BottomPlayer { get; set; }
        public string Fen { get; set; }
        public string Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime Duration { get; set; }
        public int IncrementSeconds { get; set; }
    }
}