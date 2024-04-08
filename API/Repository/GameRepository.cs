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
            return await _context.Games
                .Include(game => game.FirstPlayer)
                .Include(game => game.SecondPlayer)
                .SingleOrDefaultAsync(game => game.Id == id);
        }

        public async Task<Game> GetGameForPlayerAsync(long id)
        {
            return await _context.Games.Where(game =>
                            (game.SecondPlayerId == id ||
                            game.FirstPlayerId == id) &&
                            (game.Status == "waiting" || game.Status == "playing")).Include(game => game.FirstPlayer).Include(game => game.SecondPlayer).FirstOrDefaultAsync();
        }

        public void UpdateGame(Game game)
        {
            _context.Entry(game).State = EntityState.Modified;
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public Task<Game> GetMatchingGame(bool isPrivate)
        {
            return _context.Games.Where(game => game.Status == "waiting" && isPrivate == game.IsPrivate).Include(game => game.FirstPlayer).FirstOrDefaultAsync();
        }

        public async Task DeleteGame(string gameId)
        {
            var game = await _context.Games.FindAsync(gameId);
            if (game != null)
                _context.Games.Remove(game);
        }
    }
}