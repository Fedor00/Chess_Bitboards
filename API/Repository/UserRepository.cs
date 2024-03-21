using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Entities;
using DeviceMicroservice.Data;
using Microsoft.EntityFrameworkCore;
using UserMicroservice.Interfaces;

namespace UserMicroservice.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext dataContext)
        {
            _context = dataContext;
        }

        public async Task Add(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task Delete(long id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Remove(user);
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<User> GetUserByIdAsync(long id)
        {
            return await _context.Users.FindAsync(id);
        }

        public void Update(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}