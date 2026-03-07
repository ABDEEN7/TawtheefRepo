using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthSessionIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserSession_UserId",
                table: "UserSession");

            migrationBuilder.DropIndex(
                name: "IX_RefreshToken_UserId",
                table: "RefreshToken");

            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "UserSession",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_UserSession_SessionId",
                table: "UserSession",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSession_UserId_SessionId",
                table: "UserSession",
                columns: new[] { "UserId", "SessionId" });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_TokenHash",
                table: "RefreshToken",
                column: "TokenHash");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserId_SecurityStamp",
                table: "RefreshToken",
                columns: new[] { "UserId", "SecurityStamp" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserSession_SessionId",
                table: "UserSession");

            migrationBuilder.DropIndex(
                name: "IX_UserSession_UserId_SessionId",
                table: "UserSession");

            migrationBuilder.DropIndex(
                name: "IX_RefreshToken_TokenHash",
                table: "RefreshToken");

            migrationBuilder.DropIndex(
                name: "IX_RefreshToken_UserId_SecurityStamp",
                table: "RefreshToken");

            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "UserSession",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_UserSession_UserId",
                table: "UserSession",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserId",
                table: "RefreshToken",
                column: "UserId");
        }
    }
}
