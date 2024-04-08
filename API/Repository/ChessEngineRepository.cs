using API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using DeviceMicroservice.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace API.Repository
{
    public class ChessEngineRepository : IChessEngineRepository
    {
        private readonly IDbContextFactory<DataContext> _contextFactory;

        public ChessEngineRepository(IDbContextFactory<DataContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task AddChessEngine(ChessEngine chessEngine)
        {
            using var context = _contextFactory.CreateDbContext();
            await context.ChessEngines.AddAsync(chessEngine);
            await context.SaveChangesAsync();
        }

        public async Task DeleteChessEngine(long id)
        {
            using var context = _contextFactory.CreateDbContext();
            var chessEngine = await context.ChessEngines.FindAsync(id);
            if (chessEngine != null)
            {
                context.ChessEngines.Remove(chessEngine);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ChessEngine>> GetAllChessEnginesAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.ChessEngines.ToListAsync();
        }

        public async Task<ChessEngine> GetChessEngineByIdAsync(long id)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.ChessEngines.FindAsync(id);
        }

        public async Task<ChessEngine> GetChessEngineByNameAsync(string engineName)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.ChessEngines.FirstOrDefaultAsync(x => x.EngineName == engineName);
        }

        public async Task<bool> SaveChangesAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.SaveChangesAsync() > 0;
        }

        public void UpdateChessEngine(ChessEngine chessEngine)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Entry(chessEngine).State = EntityState.Modified;
            context.SaveChanges();
        }
    }
}
