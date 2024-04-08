using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddChessEngineAndEngineGames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChessEngines",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EngineName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChessEngines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EngineGames",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    EngineId = table.Column<long>(type: "bigint", nullable: false),
                    IsPlayerWhite = table.Column<bool>(type: "boolean", nullable: false),
                    Fen = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EngineGames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EngineGames_ChessEngines_EngineId",
                        column: x => x.EngineId,
                        principalTable: "ChessEngines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EngineGames_Users_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EngineGames_EngineId",
                table: "EngineGames",
                column: "EngineId");

            migrationBuilder.CreateIndex(
                name: "IX_EngineGames_PlayerId",
                table: "EngineGames",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EngineGames");

            migrationBuilder.DropTable(
                name: "ChessEngines");
        }
    }
}
