using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class ChangeGameStructureToAccommodatePlayWithEngine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Users_BottomPlayerId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_Users_TopPlayerId",
                table: "Games");

            migrationBuilder.DropTable(
                name: "EngineGames");

            migrationBuilder.RenameColumn(
                name: "TopPlayerId",
                table: "Games",
                newName: "SecondPlayerId");

            migrationBuilder.RenameColumn(
                name: "BottomPlayerId",
                table: "Games",
                newName: "FirstPlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_Games_TopPlayerId",
                table: "Games",
                newName: "IX_Games_SecondPlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_Games_BottomPlayerId",
                table: "Games",
                newName: "IX_Games_FirstPlayerId");

            migrationBuilder.AddColumn<long>(
                name: "EngineId",
                table: "Games",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFirstPlayerWhite",
                table: "Games",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Games_EngineId",
                table: "Games",
                column: "EngineId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_ChessEngines_EngineId",
                table: "Games",
                column: "EngineId",
                principalTable: "ChessEngines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Users_FirstPlayerId",
                table: "Games",
                column: "FirstPlayerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Users_SecondPlayerId",
                table: "Games",
                column: "SecondPlayerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_ChessEngines_EngineId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_Users_FirstPlayerId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_Users_SecondPlayerId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_EngineId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "EngineId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "IsFirstPlayerWhite",
                table: "Games");

            migrationBuilder.RenameColumn(
                name: "SecondPlayerId",
                table: "Games",
                newName: "TopPlayerId");

            migrationBuilder.RenameColumn(
                name: "FirstPlayerId",
                table: "Games",
                newName: "BottomPlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_Games_SecondPlayerId",
                table: "Games",
                newName: "IX_Games_TopPlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_Games_FirstPlayerId",
                table: "Games",
                newName: "IX_Games_BottomPlayerId");

            migrationBuilder.CreateTable(
                name: "EngineGames",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    EngineId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    Fen = table.Column<string>(type: "text", nullable: true),
                    IsPlayerWhite = table.Column<bool>(type: "boolean", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: true)
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

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Users_BottomPlayerId",
                table: "Games",
                column: "BottomPlayerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Users_TopPlayerId",
                table: "Games",
                column: "TopPlayerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
