using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingInterviewPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Permission",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsAssignableToRole", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("3126aa54-aa8b-4b51-bd72-992afea7c7dc"), "interview-evaluation.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 110, true, true, false, "تقيم المقابلات - عرض", "Interview Evaluation - View", null, null },
                    { new Guid("4f11ce47-e7c8-2f52-920a-2c992f6acd03"), "interview-evaluation.manage", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 111, true, true, false, "تقيم المقابلات - إدارة", "Interview Evaluation - Manage", null, null },
                    { new Guid("61963e13-e87c-935f-9b33-8a1c6fa4f29f"), "interview-result-report.manage", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 113, true, true, false, "تقرير نتائج المقابلات - إدارة", "Interview Result Report - Manage", null, null },
                    { new Guid("68994108-43c5-6a56-ac33-5471af9d8411"), "interview-schedule.manage", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 109, true, true, false, "جدولة المقابلات - إدارة", "Interview Schedule - Manage", null, null },
                    { new Guid("dded038e-a98d-f356-b267-851e94524810"), "interview-result-report.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 112, true, true, false, "تقرير نتائج المقابلات - عرض", "Interview Result Report - View", null, null },
                    { new Guid("fe4a15d1-6b80-b65d-83b2-0007b60737a0"), "interview-schedule.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 108, true, true, false, "جدولة المقابلات - عرض", "Interview Schedule - View", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("3126aa54-aa8b-4b51-bd72-992afea7c7dc"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("4f11ce47-e7c8-2f52-920a-2c992f6acd03"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("61963e13-e87c-935f-9b33-8a1c6fa4f29f"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("68994108-43c5-6a56-ac33-5471af9d8411"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("dded038e-a98d-f356-b267-851e94524810"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("fe4a15d1-6b80-b65d-83b2-0007b60737a0"));
        }
    }
}
