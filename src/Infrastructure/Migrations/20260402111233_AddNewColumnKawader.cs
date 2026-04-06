using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewColumnKawader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "KawaderQids",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "KawaderQids",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InvitedAt",
                table: "KawaderQids",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsInvited",
                table: "KawaderQids",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "KawaderQids",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "KawaderQids");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "KawaderQids");

            migrationBuilder.DropColumn(
                name: "InvitedAt",
                table: "KawaderQids");

            migrationBuilder.DropColumn(
                name: "IsInvited",
                table: "KawaderQids");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "KawaderQids");
        }
    }
}
