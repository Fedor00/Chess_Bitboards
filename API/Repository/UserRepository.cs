using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models.Entities;
using DeviceMicroservice.Data;
using Microsoft.EntityFrameworkCore;
using UserMicroservice.Interfaces;

namespace UserMicroservice.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbContextFactory<DataContext> _contextFactory;

        public UserRepository(IDbContextFactory<DataContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task Add(User user)
        {
            using var context = _contextFactory.CreateDbContext();
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task Delete(long id)
        {
            using var context = _contextFactory.CreateDbContext();
            var user = await context.Users.FindAsync(id);
            if (user != null)
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Users.ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(long id)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Users.FindAsync(id);
        }

        public async Task Update(User user)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Entry(user).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.SaveChangesAsync() > 0;
        }
    }
}
