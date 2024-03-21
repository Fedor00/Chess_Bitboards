using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserMicroservice.Entities;

namespace DeviceMicroservice.Data
{
    public class DataContext : IdentityDbContext<User, AppRole, long,
    IdentityUserClaim<long>, AppUserRole, IdentityUserLogin<long>,
    IdentityRoleClaim<long>, IdentityUserToken<long>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();

            builder.Entity<AppRole>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();
        }
    }
}