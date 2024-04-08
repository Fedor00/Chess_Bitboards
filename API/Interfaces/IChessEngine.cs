using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IChessEngine
    {
        Task<string> GetBestMoveAsync(string fen);
    }
}