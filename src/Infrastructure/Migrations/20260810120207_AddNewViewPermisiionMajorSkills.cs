using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewViewPermisiionMajorSkills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { -2111228905, "permission", "major-skill.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -1788876049, "permission", "universities.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -1407099366, "permission", "universities.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -979937753, "permission", "universities.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -904949549, "permission", "universities.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -839366322, "permission", "universities.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -592066334, "permission", "major-skill.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -369093012, "permission", "universities.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Permission",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsAssignableToRole", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[] { new Guid("7a0053f5-6e71-3f5b-9b50-a98ce84cbf32"), "major-skill.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 69, true, true, false, "التخصصات والمهارات - عرض", "Major Skills - View", null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -2111228905);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1788876049);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1407099366);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -979937753);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -904949549);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -839366322);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -592066334);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -369093012);

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("7a0053f5-6e71-3f5b-9b50-a98ce84cbf32"));

        }
    }
}
