using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Logic;
using API.Models.Entities;

namespace API.Interfaces
{
    public interface IGameRepository
    {
        Task<Game> GetGameAsync(string id);
        Task<Game> GetGameForPlayerAsync(long id);
        Task<Game> GetMatchingGame(bool isPrivate);
        void UpdateGame(Game game);
        Task DeleteGame(string gameId);
        Task AddGameAsync(Game game);
        Task<bool> SaveChangesAsync();
    }
}