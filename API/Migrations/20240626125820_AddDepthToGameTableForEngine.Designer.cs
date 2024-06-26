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
    [Migration("20240626125820_AddDepthToGameTableForEngine")]
    partial class AddDepthToGameTableForEngine
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("API.Models.Entities.ChatMessage", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<string>("GameId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("SenderId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("SenderId");

                    b.ToTable("ChatMessages");
                });

            modelBuilder.Entity("API.Models.Entities.ChessEngine", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("EngineName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ChessEngines");
                });

            modelBuilder.Entity("API.Models.Entities.Game", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("EngineDepth")
                        .HasColumnType("integer");

                    b.Property<long?>("EngineId")
                        .HasColumnType("bigint");

                    b.Property<string>("Fen")
                        .HasColumnType("text");

                    b.Property<long?>("FirstPlayerId")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsFirstPlayerWhite")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsPrivate")
                        .HasColumnType("boolean");

                    b.Property<long?>("SecondPlayerId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Status")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("EngineId");

                    b.HasIndex("FirstPlayerId");

                    b.HasIndex("SecondPlayerId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("API.Models.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("API.Models.Entities.ChatMessage", b =>
                {
                    b.HasOne("API.Models.Entities.Game", "Game")
                        .WithMany("ChatMessages")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API.Models.Entities.User", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("API.Models.Entities.Game", b =>
                {
                    b.HasOne("API.Models.Entities.ChessEngine", "Engine")
                        .WithMany()
                        .HasForeignKey("EngineId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("API.Models.Entities.User", "FirstPlayer")
                        .WithMany()
                        .HasForeignKey("FirstPlayerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("API.Models.Entities.User", "SecondPlayer")
                        .WithMany()
                        .HasForeignKey("SecondPlayerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Engine");

                    b.Navigation("FirstPlayer");

                    b.Navigation("SecondPlayer");
                });

            modelBuilder.Entity("API.Models.Entities.Game", b =>
                {
                    b.Navigation("ChatMessages");
                });
#pragma warning restore 612, 618
        }
    }
}
