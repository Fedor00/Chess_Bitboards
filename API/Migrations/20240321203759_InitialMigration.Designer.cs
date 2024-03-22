﻿// <auto-generated />
using System;
using DeviceMicroservice.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20240321203759_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("API.Models.Entities.Game", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<long?>("BottomPlayerId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("Duration")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Fen")
                        .HasColumnType("text");

                    b.Property<int>("IncrementSeconds")
                        .HasColumnType("integer");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Status")
                        .HasColumnType("text");

                    b.Property<long?>("TopPlayerId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("BottomPlayerId");

                    b.HasIndex("TopPlayerId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("API.Models.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("API.Models.Entities.Game", b =>
                {
                    b.HasOne("API.Models.Entities.User", "BottomPlayer")
                        .WithMany("BottomPlayerGames")
                        .HasForeignKey("BottomPlayerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("API.Models.Entities.User", "TopPlayer")
                        .WithMany("TopPlayerGames")
                        .HasForeignKey("TopPlayerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("BottomPlayer");

                    b.Navigation("TopPlayer");
                });

            modelBuilder.Entity("API.Models.Entities.User", b =>
                {
                    b.Navigation("BottomPlayerGames");

                    b.Navigation("TopPlayerGames");
                });
#pragma warning restore 612, 618
        }
    }
}
