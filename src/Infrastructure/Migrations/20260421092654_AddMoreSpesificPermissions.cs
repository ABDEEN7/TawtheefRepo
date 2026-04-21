using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreSpesificPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { -1923873668, "permission", "cities.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -1887851879, "permission", "minister-office.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -1712025094, "permission", "cities.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -1664615521, "permission", "job-titles.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -1629212727, "permission", "job-titles.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -1547244454, "permission", "job-category-candidate-settings.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -1137221041, "permission", "job-titles.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -874467932, "permission", "job-category-candidate-settings.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -779259237, "permission", "minister-office.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -655765710, "permission", "cities.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -649410427, "permission", "minister-office.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -616470609, "permission", "job-category-candidate-settings.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -443960306, "permission", "job-category-candidate-settings.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -90565718, "permission", "minister-office.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -82237110, "permission", "cities.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -60620829, "permission", "job-titles.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Permission",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsAssignableToRole", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("19dacd27-70e1-cf58-9044-3f8d3c89b218"), "job-points-configuration.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 98, true, true, false, "إعدادات نقاط الوظيفة - عرض", "Job Points Configuration - View", null, null },
                    { new Guid("4707ef48-6d9b-f353-b98b-05a3c2c18fee"), "job-category-candidate-settings.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 96, true, true, false, "إعدادات المرشحين حسب تصنيف الوظيفة - عرض", "Job Category Candidate Settings - View", null, null },
                    { new Guid("ad3c96a7-f148-1951-a9b1-2f9ca4a2609d"), "job-points-configuration.manage", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 99, true, true, false, "إعدادات نقاط الوظيفة - إدارة", "Job Points Configuration - Manage", null, null },
                    { new Guid("d834840b-b71a-0158-8ca0-e8b69f279832"), "job-titles.manage", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 95, true, true, false, "المسميات الوظيفية - إدارة", "Job Titles - Manage", null, null },
                    { new Guid("d9a3ea88-d07d-c454-ab70-c5c02dd6cabf"), "job-titles.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 94, true, true, false, "المسميات الوظيفية - عرض", "Job Titles - View", null, null },
                    { new Guid("e6cdfe35-b480-b85e-be3e-05a8b67e9468"), "job-category-candidate-settings.manage", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 97, true, true, false, "إعدادات المرشحين حسب تصنيف الوظيفة - إدارة", "Job Category Candidate Settings - Manage", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1923873668);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1887851879);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1712025094);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1664615521);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1629212727);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1547244454);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1137221041);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -874467932);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -779259237);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -655765710);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -649410427);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -616470609);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -443960306);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -90565718);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -82237110);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -60620829);

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("19dacd27-70e1-cf58-9044-3f8d3c89b218"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("4707ef48-6d9b-f353-b98b-05a3c2c18fee"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("ad3c96a7-f148-1951-a9b1-2f9ca4a2609d"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("d834840b-b71a-0158-8ca0-e8b69f279832"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("d9a3ea88-d07d-c454-ab70-c5c02dd6cabf"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("e6cdfe35-b480-b85e-be3e-05a8b67e9468"));
        }
    }
}
