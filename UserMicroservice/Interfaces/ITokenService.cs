using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserMicroservice.Entities;

namespace UserMicroservice.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(User user);
    }
}