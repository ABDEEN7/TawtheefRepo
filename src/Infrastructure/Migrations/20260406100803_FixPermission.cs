using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1412499915);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1236739545);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1223443107);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1179022365);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1087299740);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -260640278);

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("5d68fab4-cd5e-1958-a5f0-19d93f0db665"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("64b8bb0d-a959-9555-ad77-3c3ab94bd7a6"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { -1412499915, "permission", "nominations.view", new Guid("ac011a30-6b0e-496c-a8ef-ba8132bd1808") },
                    { -1236739545, "permission", "nominations.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -1223443107, "permission", "nominations.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -1179022365, "permission", "dashboard.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -1087299740, "permission", "nominations.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -260640278, "permission", "nominations.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Permission",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsAssignableToRole", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("5d68fab4-cd5e-1958-a5f0-19d93f0db665"), "nominations.manage", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 41, true, true, false, "الترشيحات - إدارة", "Nominations - Manage", null, null },
                    { new Guid("64b8bb0d-a959-9555-ad77-3c3ab94bd7a6"), "nominations.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 40, true, true, false, "الترشيحات - عرض", "Nominations - View", null, null }
                });
        }
    }
}
