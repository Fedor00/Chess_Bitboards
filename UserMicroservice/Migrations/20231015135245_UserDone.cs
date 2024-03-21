using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserMicroservice.Migrations
{
    /// <inheritdoc />
    public partial class UserDone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                table: "AspNetUsers",
                type: "bytea",
                nullable: true);
        }
    }
}
