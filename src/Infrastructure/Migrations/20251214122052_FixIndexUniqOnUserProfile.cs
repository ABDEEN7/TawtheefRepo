using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixIndexUniqOnUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserProfile_NationalNumber",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_NationalNumber_NationalityId",
                schema: "app",
                table: "UserProfile",
                columns: new[] { "NationalNumber", "NationalityId" },
                unique: true,
                filter: "[NationalNumber] IS NOT NULL AND [NationalityId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserProfile_NationalNumber_NationalityId",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_NationalNumber",
                schema: "app",
                table: "UserProfile",
                column: "NationalNumber",
                unique: true,
                filter: "[NationalNumber] IS NOT NULL");
        }
    }
}
