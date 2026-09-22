using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExamsAndInterviewUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -2029722188);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1603357864);

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "TestSlotStatus",
                keyColumn: "Id",
                keyValue: new Guid("02a5a982-e326-403c-a8da-34b2f33d88f0"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "TestSlotStatus",
                keyColumn: "Id",
                keyValue: new Guid("4ff15b36-d258-48f4-ab1c-2380dfc0d1b0"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "TestSlotStatus",
                keyColumn: "Id",
                keyValue: new Guid("7995eaf8-baaf-4dcc-a30d-0becc5a8e4d8"));

            migrationBuilder.CreateSequence<int>(
                name: "InterviewCommitteeNumber",
                schema: "itv");

            migrationBuilder.AddColumn<int>(
                name: "Number",
                schema: "itv",
                table: "InterviewCommittee",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR [itv].[InterviewCommitteeNumber]");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                schema: "itv",
                table: "InterviewCommittee",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                computedColumnSql: "'COM-' + CAST(YEAR([CreatedDate]) AS varchar(4)) + '-' + CASE WHEN [Number] < 10000 THEN RIGHT('0000' + CAST([Number] AS varchar(10)), 4) ELSE CAST([Number] AS varchar(10)) END",
                stored: true);

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { -1986926594, "permission", "test-slots.manage-period", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -1856001488, "permission", "test-slots.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -485163916, "permission", "test-slots.create", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") }
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "DescriptionAr", "DescriptionEn", "IsSystemRole", "Name", "NameAr", "NameEn", "NormalizedName" },
                values: new object[] { new Guid("b6fc27dc-93ec-4b04-b3a7-311655e87b7f"), "8500aa5d-c126-4ef1-8b82-6a179e1d2d3f", "عضو فريق فترات الاختبار", "Test slot staff member", true, "TestSlotStaffMember", "عضو فريق فترات الاختبار", "Test Slot Staff Member", "TESTSLOTSTAFFMEMBER" });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Permission",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsAssignableToRole", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("501bf2ca-0ddf-695c-a463-7db62859dff3"), "test-slots.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 109, true, true, false, "فترات الاختبار - عرض", "Exam Periods - View", null, null },
                    { new Guid("94bc52f4-a685-af5c-85db-a37d5a1ddb49"), "test-slots.create", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 110, true, true, false, "فترات الاختبار - إنشاء", "Exam Periods - Create", null, null },
                    { new Guid("c579b2d8-b1a7-c25a-8643-9644fb276fd2"), "test-slots.manage-period", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 112, true, true, false, "فترات الاختبار - إدارة الفترة", "Exam Periods - Manage Period", null, null },
                    { new Guid("fa37e6d2-e157-ac55-8e10-2d4ae7ec2355"), "test-slots.view-access-code", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 111, true, true, false, "فترات الاختبار - عرض رمز الدخول", "Exam Periods - View Access Code", null, null }
                });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "TestSlotStatus",
                keyColumn: "Id",
                keyValue: new Guid("215d0bfd-1ad4-4613-af9e-79d977dfd637"),
                column: "DisplayOrder",
                value: 4);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "TestSlotStatus",
                keyColumn: "Id",
                keyValue: new Guid("46ae567b-7f97-47ef-a960-9fd9efd32a53"),
                column: "DisplayOrder",
                value: 3);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "TestSlotStatus",
                keyColumn: "Id",
                keyValue: new Guid("cc71a094-b1b0-4c44-ab10-f48a8cc12140"),
                column: "DisplayOrder",
                value: 2);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "TestSlotStatus",
                keyColumn: "Id",
                keyValue: new Guid("fd397696-6596-4482-ae56-4c187813a65e"),
                column: "DisplayOrder",
                value: 1);

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { -1615214602, "permission", "test-slots.manage-period", new Guid("b6fc27dc-93ec-4b04-b3a7-311655e87b7f") },
                    { -1337878609, "permission", "test-slots.view", new Guid("b6fc27dc-93ec-4b04-b3a7-311655e87b7f") },
                    { -42408079, "permission", "test-slots.view-access-code", new Guid("b6fc27dc-93ec-4b04-b3a7-311655e87b7f") }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("b6fc27dc-93ec-4b04-b3a7-311655e87b7f"), new Guid("781561c3-0175-4165-80c1-7c6a79130b25") },
                    { new Guid("b6fc27dc-93ec-4b04-b3a7-311655e87b7f"), new Guid("a8e0f354-2e23-41e1-9e1b-a1501b72dc4b") }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_InterviewAppointment_Room_RoomId",
                schema: "itv",
                table: "InterviewAppointment",
                column: "RoomId",
                principalSchema: "hr",
                principalTable: "Room",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestSlotStaff_AspNetUsers_StaffUserId",
                schema: "hr",
                table: "TestSlotStaff",
                column: "StaffUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterviewAppointment_Room_RoomId",
                schema: "itv",
                table: "InterviewAppointment");

            migrationBuilder.DropForeignKey(
                name: "FK_TestSlotStaff_AspNetUsers_StaffUserId",
                schema: "hr",
                table: "TestSlotStaff");

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1986926594);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1856001488);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1615214602);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1337878609);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -485163916);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -42408079);

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("b6fc27dc-93ec-4b04-b3a7-311655e87b7f"), new Guid("781561c3-0175-4165-80c1-7c6a79130b25") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("b6fc27dc-93ec-4b04-b3a7-311655e87b7f"), new Guid("a8e0f354-2e23-41e1-9e1b-a1501b72dc4b") });

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("501bf2ca-0ddf-695c-a463-7db62859dff3"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("94bc52f4-a685-af5c-85db-a37d5a1ddb49"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("c579b2d8-b1a7-c25a-8643-9644fb276fd2"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("fa37e6d2-e157-ac55-8e10-2d4ae7ec2355"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("b6fc27dc-93ec-4b04-b3a7-311655e87b7f"));

            migrationBuilder.DropColumn(
                name: "Code",
                schema: "itv",
                table: "InterviewCommittee");

            migrationBuilder.DropColumn(
                name: "Number",
                schema: "itv",
                table: "InterviewCommittee");

            migrationBuilder.DropSequence(
                name: "InterviewCommitteeNumber",
                schema: "itv");

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { -2029722188, "permission", "exams.create", new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd") },
                    { -1603357864, "permission", "exams.view", new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd") }
                });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "TestSlotStatus",
                keyColumn: "Id",
                keyValue: new Guid("215d0bfd-1ad4-4613-af9e-79d977dfd637"),
                column: "DisplayOrder",
                value: 6);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "TestSlotStatus",
                keyColumn: "Id",
                keyValue: new Guid("46ae567b-7f97-47ef-a960-9fd9efd32a53"),
                column: "DisplayOrder",
                value: 5);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "TestSlotStatus",
                keyColumn: "Id",
                keyValue: new Guid("cc71a094-b1b0-4c44-ab10-f48a8cc12140"),
                column: "DisplayOrder",
                value: 4);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "TestSlotStatus",
                keyColumn: "Id",
                keyValue: new Guid("fd397696-6596-4482-ae56-4c187813a65e"),
                column: "DisplayOrder",
                value: 3);

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "TestSlotStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("02a5a982-e326-403c-a8da-34b2f33d88f0"), "Draft", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "مسودة", "Draft", null, null },
                    { new Guid("4ff15b36-d258-48f4-ab1c-2380dfc0d1b0"), "Cancelled", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 7, true, false, "ملغي", "Cancelled", null, null },
                    { new Guid("7995eaf8-baaf-4dcc-a30d-0becc5a8e4d8"), "Approved", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "معتمد", "Approved", null, null }
                });
        }
    }
}
