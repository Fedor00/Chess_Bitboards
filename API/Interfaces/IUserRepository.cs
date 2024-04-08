using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Entities;

namespace UserMicroservice.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(long id);
        Task<bool> SaveChangesAsync();
        Task Update(User user);
        Task Delete(long id);
        Task Add(User user);
    }

}