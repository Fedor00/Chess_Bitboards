
using System.Text.Json;
using API.Models.Entities;
using DeviceMicroservice.Data;
using Microsoft.EntityFrameworkCore;

namespace UserMicroservice.Data
{
    public class Seeder
    {
        public static async Task SeedUsers(DataContext context)
        {
            if (await context.Users.AnyAsync()) return; // if populated, don't populate again
            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<User>>(userData);

            foreach (var user in users)
            {
                Console.WriteLine(user.UserName);
                await context.Users.AddAsync(user);
            }
            await context.SaveChangesAsync();
        }
    }
}