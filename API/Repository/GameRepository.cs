using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;
using API.Models.Entities;
using DeviceMicroservice.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Repository
{
    public class GameRepository : IGameRepository
    {
        private readonly IDbContextFactory<DataContext> _contextFactory;

        public GameRepository(IDbContextFactory<DataContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task AddGameAsync(Game game)
        {
            using var context = _contextFactory.CreateDbContext();
            await context.Games.AddAsync(game);
            await context.SaveChangesAsync();
        }

        public async Task<Game> GetGameAsync(string id)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Games
                .Include(game => game.FirstPlayer)
                .Include(game => game.SecondPlayer)
                .Include(game => game.Engine)
                .SingleOrDefaultAsync(game => game.Id == id);
        }

        public async Task<Game> GetGameForPlayerAsync(long id)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Games.Where(game =>
                            (game.SecondPlayerId == id || game.FirstPlayerId == id) &&
                            (game.Status == "waiting" || game.Status == "playing"))
                            .Include(game => game.FirstPlayer)
                            .Include(game => game.SecondPlayer)
                            .Include(game => game.Engine)
                            .FirstOrDefaultAsync();
        }

        public async Task UpdateGame(Game game)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Entry(game).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }

        public async Task DeleteGame(string gameId)
        {
            using var context = _contextFactory.CreateDbContext();
            var game = await context.Games.FindAsync(gameId);
            if (game != null)
            {
                context.Games.Remove(game);
                await context.SaveChangesAsync();
            }
        }


        public async Task<Game> GetMatchingGame(bool isPrivate)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Games
                       .Where(game => game.Status == "waiting" && isPrivate == game.IsPrivate)
                       .Include(game => game.FirstPlayer)
                       .FirstOrDefaultAsync();
        }

    }
}
