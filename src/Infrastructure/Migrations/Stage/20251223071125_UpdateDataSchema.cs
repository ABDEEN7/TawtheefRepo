#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDataSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Provider",
                schema: "app",
                table: "UserProfile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Provider",
                schema: "app",
                table: "UserProfile");
        }
    }
}
