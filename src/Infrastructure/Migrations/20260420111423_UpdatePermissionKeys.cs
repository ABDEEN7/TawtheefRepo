using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePermissionKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1866946281);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1852694989);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -454492332);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -419574808);

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("377a116d-4709-d65d-9565-c01e8908d25a"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("ab24906c-15de-7251-bbc0-d278fda72ae0"));

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { -1920461912, "permission", "jobs.send-invitation", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -1808986077, "permission", "jobs.create", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -1572135063, "permission", "jobs.edit", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -1439392780, "permission", "jobs.publish", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -1402030595, "permission", "jobs.edit", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -1373445632, "permission", "jobs.delete", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -1259038385, "permission", "jobs.points.edit", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -1161344859, "permission", "jobs.clone", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -1109251112, "permission", "jobs.send-invitation", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -838100981, "permission", "jobs.delete", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -790383713, "permission", "jobs.clone", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -686811409, "permission", "jobs.cancel", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -529932315, "permission", "jobs.create", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -446936914, "permission", "jobs.publish", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -92787468, "permission", "jobs.points.edit", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -86041534, "permission", "jobs.cancel", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Permission",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsAssignableToRole", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("13abdab6-ce4f-0c55-b969-c2ca56eb8fe6"), "jobs.edit", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 31, true, true, false, "الوظائف - تعديل", "Jobs - Edit", null, null },
                    { new Guid("36030234-5820-8756-89b9-dd1038173a45"), "jobs.delete", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 36, true, true, false, "الوظائف - حذف", "Jobs - Delete", null, null },
                    { new Guid("50642bb5-f557-6954-99f3-6d6732446624"), "jobs.points.edit", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 41, true, true, false, "نقاط الوظائف - تعديل", "Jobs - Points Edit", null, null },
                    { new Guid("6917c852-e9e4-6758-8e80-5a4797d8e7b9"), "jobs.create", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 34, true, true, false, "الوظائف - إنشاء", "Jobs - Create", null, null },
                    { new Guid("6fb9d7e1-c7e8-b05b-a2c4-16c5da50e1b0"), "jobs.publish", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 35, true, true, false, "الوظائف - نشر", "Jobs - Publish", null, null },
                    { new Guid("b9d42f3d-ab20-f85f-8a3f-448db7e727f2"), "jobs.clone", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 37, true, true, false, "الوظائف - استنساخ", "Jobs - Clone", null, null },
                    { new Guid("d714523d-6aac-a255-bab5-f1b74ae0bd30"), "jobs.cancel", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 33, true, true, false, "الوظائف - إلغاء", "Jobs - Cancel", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1920461912);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1808986077);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1572135063);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1439392780);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1402030595);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1373445632);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1259038385);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1161344859);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1109251112);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -838100981);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -790383713);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -686811409);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -529932315);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -446936914);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -92787468);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -86041534);

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("13abdab6-ce4f-0c55-b969-c2ca56eb8fe6"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("36030234-5820-8756-89b9-dd1038173a45"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("50642bb5-f557-6954-99f3-6d6732446624"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("6917c852-e9e4-6758-8e80-5a4797d8e7b9"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("6fb9d7e1-c7e8-b05b-a2c4-16c5da50e1b0"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("b9d42f3d-ab20-f85f-8a3f-448db7e727f2"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("d714523d-6aac-a255-bab5-f1b74ae0bd30"));

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { -1866946281, "permission", "jobs.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -1852694989, "permission", "jobs.points.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -454492332, "permission", "jobs.points.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -419574808, "permission", "jobs.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Permission",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsAssignableToRole", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("377a116d-4709-d65d-9565-c01e8908d25a"), "jobs.points.manage", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 33, true, true, false, "نقاط الوظائف - إدارة", "Jobs Points - Manage", null, null },
                    { new Guid("ab24906c-15de-7251-bbc0-d278fda72ae0"), "jobs.manage", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 31, true, true, false, "الوظائف - إدارة", "Jobs - Manage", null, null }
                });
        }
    }
}
