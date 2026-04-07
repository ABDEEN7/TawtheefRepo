using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovePermissionOfficeUserFromHrManager : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -2089510203);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1728489694);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -305898939);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -10839799);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { -2089510203, "permission", "office.users.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -1728489694, "permission", "office.users.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -305898939, "permission", "office.users.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -10839799, "permission", "office.users.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") }
                });
        }
    }
}
