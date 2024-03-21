using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;
using API.Logic;
using API.Models.Entities;
using DeviceMicroservice.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Repository
{
    public class GameRepository : IGameRepository
    {
        private readonly DataContext _context;
        public GameRepository(DataContext dataContext)
        {
            _context = dataContext;
        }

        public async Task AddGameAsync(Game game)
        {
            await _context.Games.AddAsync(game);
        }

        public async Task<Game> GetGameAsync(string id)
        {
            return await _context.Games.FindAsync(id);
        }

        public async Task<Game> GetGameForPlayerAsync(long id)
        {
            return await _context.Games.Where(game =>
                            (game.TopPlayerId == id ||
                            game.BottomPlayerId == id) &&
                            (game.Status == "waiting" || game.Status == "playing")).FirstOrDefaultAsync();
        }

        public void UpdateGame(Game game)
        {
            _context.Entry(game).State = EntityState.Modified;
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public Task<Game> GetWaitingGame()
        {
            return _context.Games.Where(game => game.Status == "waiting").FirstOrDefaultAsync();
        }
    }
}