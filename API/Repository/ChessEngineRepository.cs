
using API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using DeviceMicroservice.Data;

namespace API.Repository
{
    public class ChessEngineRepository : IChessEngineRepository
    {
        private readonly DataContext _context;
        public ChessEngineRepository(DataContext dataContext)
        {
            _context = dataContext;
        }

        public async Task AddChessEngine(ChessEngine chessEngine)
        {
            await _context.ChessEngines.AddAsync(chessEngine);
        }

        public async Task DeleteChessEngine(long id)
        {
            var chessEngine = await _context.ChessEngines.FindAsync(id);
            _context.Remove(chessEngine);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<ChessEngine>> GetAllChessEnginesAsync()
        {
            return await _context.ChessEngines.ToListAsync();
        }

        public async Task<ChessEngine> GetChessEngineByIdAsync(long id)
        {
            return await _context.ChessEngines.FindAsync(id);
        }

        public void UpdateChessEngine(ChessEngine chessEngine)
        {
            _context.Entry(chessEngine).State = EntityState.Modified;
        }

        public async Task<ChessEngine> GetChessEngineByNameAsync(string engineName)
        {

            return await _context.ChessEngines.FirstOrDefaultAsync(x => x.EngineName == engineName);
        }
    }
}