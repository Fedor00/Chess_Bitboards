using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserMicroservice.Entities;

namespace UserMicroservice.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(long id);
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> SaveChangesAsync();
        void Update(User user);
        Task Delete(long id);
        Task Add(User user);
    }

}