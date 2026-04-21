using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPreferredLanguage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserId_IsRead_IsDismissed",
                table: "Notifications");

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Notifications",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "ar");

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "ContactVerification",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "ar");

            migrationBuilder.AddColumn<string>(
                name: "PreferredLanguage",
                table: "AspNetUsers",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "ar");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_Channel_IsDismissed_IsRead",
                table: "Notifications",
                columns: new[] { "UserId", "Channel", "IsDismissed", "IsRead" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserId_Channel_IsDismissed_IsRead",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "ContactVerification");

            migrationBuilder.DropColumn(
                name: "PreferredLanguage",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_IsRead_IsDismissed",
                table: "Notifications",
                columns: new[] { "UserId", "IsRead", "IsDismissed" });
        }
    }
}
