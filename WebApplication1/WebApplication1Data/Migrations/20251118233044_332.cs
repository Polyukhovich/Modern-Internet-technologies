using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1Data.Migrations
{
    /// <inheritdoc />
    public partial class _332 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SecretWord",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecretWord",
                table: "AspNetUsers");
        }
    }
}
