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
        public DbSet<ChessEngine> ChessEngines { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.Game)
                .WithMany(g => g.ChatMessages)
                .HasForeignKey(cm => cm.GameId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.FirstPlayer)
                .WithMany()
                .HasForeignKey(g => g.FirstPlayerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.SecondPlayer)
                .WithMany()
                .HasForeignKey(g => g.SecondPlayerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Game>()
                .HasOne(eg => eg.Engine)
                .WithMany()
                .HasForeignKey(eg => eg.EngineId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }




    }

}
