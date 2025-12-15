using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoleIDs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "DescriptionAr", "DescriptionEn", "IsSystemRole", "Name", "NameAr", "NameEn", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("1361d691-53c5-4a84-aea1-64ff134cf082"), "12990d2f-1d47-4058-ab89-e77c0fded7de", "مدير النظام الكامل", "Full system administrator", true, "SystemAdmin", "مدير النظام", "System Admin", "SYSTEMADMIN" },
                    { new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd"), "2e6eab95-bad1-433c-9e7d-e9822f05be63", "مدير شؤون الموظفين", "Human resources administrator", true, "HRAdmin", "مدير الموارد البشرية", "HR Admin", "HRADMIN" },
                    { new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca"), "cb109cbd-1fbf-4a19-a19b-9750f1562ffb", "مدير المكتب والصلاحيات المرتبطة", "Office administrator", true, "OfficeAdmin", "مدير المكتب", "Office Admin", "OFFICEADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("1361d691-53c5-4a84-aea1-64ff134cf082"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "DescriptionAr", "DescriptionEn", "IsSystemRole", "Name", "NameAr", "NameEn", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "f9603896-e1f0-406d-ba27-d3b405afe890", "مدير النظام الكامل", "Full system administrator", true, "SystemAdmin", "مدير النظام", "System Admin", "SYSTEMADMIN" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "3aabf83d-09ab-456d-aadd-529a91395c16", "مدير شؤون الموظفين", "Human resources administrator", true, "HRAdmin", "مدير الموارد البشرية", "HR Admin", "HRADMIN" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "772086c8-869e-4d80-9fe5-19b707cc0617", "مدير المكتب والصلاحيات المرتبطة", "Office administrator", true, "OfficeAdmin", "مدير المكتب", "Office Admin", "OFFICEADMIN" }
                });
        }
    }
}
