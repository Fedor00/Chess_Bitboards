using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserMicroservice.Entities;

namespace UserMicroservice.Data
{
    public class Seeder
    {
        public static async Task SeedUsers(UserManager<User> userManager, RoleManager<AppRole> roleManager)
        {
            if (await userManager.Users.AnyAsync()) return; // if populated, don't populate again
            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<User>>(userData);
            var roles = new List<AppRole>{
                new AppRole { Name = "User" },
                new AppRole { Name = "Manager" }
            };
            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }
            foreach (var user in users)
            {
                Console.WriteLine(user);
                user.UserName = user.UserName;
                await userManager.CreateAsync(user, "TrashTalker00@");
                await userManager.AddToRoleAsync(user, "User");
            }
            var admin = new User
            {
                UserName = "admin",
                Email = "admin@gmail.com"
            };
            await userManager.CreateAsync(admin, "TrashTalker00@");
            await userManager.AddToRoleAsync(admin, "Manager");

        }
    }
}