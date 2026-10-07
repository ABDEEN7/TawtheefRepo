using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionAndReviewBankPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -6261347);

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("46e7fb1a-7503-355f-8f83-016cfef177c1"));

            migrationBuilder.AlterColumn<Guid>(
                name: "TestSlotId",
                schema: "hr",
                table: "TestSession",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "SessionNo",
                schema: "hr",
                table: "TestSession",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "EndTime",
                schema: "hr",
                table: "TestSession",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GenderFilter",
                schema: "hr",
                table: "TestSession",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NationalityFilter",
                schema: "hr",
                table: "TestSession",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "StartTime",
                schema: "hr",
                table: "TestSession",
                type: "time",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { -1822399971, "permission", "test-sessions.create", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -1786189931, "permission", "test-sessions.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -1196982434, "permission", "question-bank-requests.review", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -1050949868, "permission", "exams.workflow-action", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -599213015, "permission", "test-sessions.workflow-action", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Permission",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsAssignableToRole", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0ef13ae9-123c-af53-a99e-323edeb522fa"), "question-bank-requests.review", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 89, true, true, false, "طلبات بنوك الأسئلة - مراجعة", "Question Bank Requests - Review", null, null },
                    { new Guid("764cb011-3839-0158-b863-8a0345721de6"), "test-sessions.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 113, true, true, false, "جلسات الاختبار - عرض", "Test Sessions - View", null, null },
                    { new Guid("8dd78700-f9aa-0050-9912-3ee1ca4917c0"), "test-sessions.workflow-action", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 115, true, true, false, "إجراءات سير عمل جلسات الاختبار", "Test Sessions - Workflow Actions", null, null },
                    { new Guid("a2e10097-67d8-dc5b-a79d-0f2d6d7c87bb"), "exams.workflow-action", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 108, true, true, false, "إجراءات سير عمل اختبارات الوظائف", "Job Exams - Workflow Actions", null, null },
                    { new Guid("b49b90e1-57b6-e150-9bdc-2b8edcf98bf3"), "test-sessions.create", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 114, true, true, false, "جلسات الاختبار - إنشاء", "Test Sessions - Create", null, null }
                });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "TestSessionStatus",
                keyColumn: "Id",
                keyValue: new Guid("16b3ac19-2ff1-491f-acca-7663c19c226b"),
                columns: new[] { "BackendName", "NameAr", "NameEn" },
                values: new object[] { "PendingApproval", "بانتظار الاعتماد", "Pending Approval" });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "TestSessionStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("6c7b42ab-a356-47e6-93a1-a5e5a2dd09d4"), "Returned", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 8, true, false, "معاد", "Returned", null, null },
                    { new Guid("f44d7a03-9eaf-498c-a3ad-6b6810b4c2da"), "Rejected", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 7, true, false, "مرفوضة", "Rejected", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestSession_SessionNo",
                schema: "hr",
                table: "TestSession",
                column: "SessionNo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TestSession_SessionNo",
                schema: "hr",
                table: "TestSession");

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1822399971);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1786189931);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1196982434);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1050949868);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -599213015);

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("0ef13ae9-123c-af53-a99e-323edeb522fa"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("764cb011-3839-0158-b863-8a0345721de6"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("8dd78700-f9aa-0050-9912-3ee1ca4917c0"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("a2e10097-67d8-dc5b-a79d-0f2d6d7c87bb"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("b49b90e1-57b6-e150-9bdc-2b8edcf98bf3"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "TestSessionStatus",
                keyColumn: "Id",
                keyValue: new Guid("6c7b42ab-a356-47e6-93a1-a5e5a2dd09d4"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "TestSessionStatus",
                keyColumn: "Id",
                keyValue: new Guid("f44d7a03-9eaf-498c-a3ad-6b6810b4c2da"));

            migrationBuilder.DropColumn(
                name: "EndTime",
                schema: "hr",
                table: "TestSession");

            migrationBuilder.DropColumn(
                name: "GenderFilter",
                schema: "hr",
                table: "TestSession");

            migrationBuilder.DropColumn(
                name: "NationalityFilter",
                schema: "hr",
                table: "TestSession");

            migrationBuilder.DropColumn(
                name: "StartTime",
                schema: "hr",
                table: "TestSession");

            migrationBuilder.AlterColumn<Guid>(
                name: "TestSlotId",
                schema: "hr",
                table: "TestSession",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SessionNo",
                schema: "hr",
                table: "TestSession",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32);

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[] { -6261347, "permission", "examWorkflowActions", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Permission",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsAssignableToRole", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[] { new Guid("46e7fb1a-7503-355f-8f83-016cfef177c1"), "examWorkflowActions", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 108, true, true, false, "إجراءات سير عمل اختبارات الوظائف", "Job Exams - Workflow Actions", null, null });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "TestSessionStatus",
                keyColumn: "Id",
                keyValue: new Guid("16b3ac19-2ff1-491f-acca-7663c19c226b"),
                columns: new[] { "BackendName", "NameAr", "NameEn" },
                values: new object[] { "InProgress", "قيد التنفيذ", "In Progress" });
        }
    }
}
