using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Entities;

namespace API.Interfaces
{
    public interface IChessEngineRepository
    {
        Task<IEnumerable<ChessEngine>> GetAllChessEnginesAsync();
        Task<ChessEngine> GetChessEngineByIdAsync(long id);
        Task<ChessEngine> GetChessEngineByNameAsync(string engineName);
        Task<bool> SaveChangesAsync();
        void UpdateChessEngine(ChessEngine chessEngine);
        Task DeleteChessEngine(long id);
        Task AddChessEngine(ChessEngine chessEngine);
    }
}