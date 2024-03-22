using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DeviceMicroservice.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>()
                .HasOne(g => g.TopPlayer)
                .WithMany(u => u.TopPlayerGames)
                .HasForeignKey(g => g.TopPlayerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.BottomPlayer)
                .WithMany(u => u.BottomPlayerGames)
                .HasForeignKey(g => g.BottomPlayerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }




    }

}
