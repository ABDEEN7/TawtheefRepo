using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDepNoToManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartmentNumber",
                schema: "lkp",
                table: "Management",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DefaultBufferMinutes",
                schema: "itv",
                table: "InterviewSchedule",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastReminderSentAt",
                schema: "itv",
                table: "InterviewAppointment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReminderCount",
                schema: "itv",
                table: "InterviewAppointment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { -2088380062, "permission", "interview-schedule.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -2055373830, "permission", "interview-result-report.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -1640940872, "permission", "interview-result-report.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -1068966641, "permission", "interview-schedule.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -1054938563, "permission", "interview-schedule.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -1041187402, "permission", "interview-evaluation.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -1039765607, "permission", "interview-schedule.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -828071010, "permission", "interview-result-report.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -782392015, "permission", "interview-result-report.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -231492257, "permission", "interview-evaluation.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -176127080, "permission", "interview-evaluation.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -8245703, "permission", "interview-evaluation.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "InvitationStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("3f6b6e1e-9c2b-4c7e-8b3e-2b6a6e7b9b10"), "InterviewEligible", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "المرشح اجتاز مرحلة الاختبار وهو مؤهل لجدولة مقابلة.", "The candidate passed the exam stage and is eligible to be scheduled for an interview.", 10, true, false, "مؤهل للمقابلة", "Interview Eligible", null, null },
                    { new Guid("5a2c3e4f-7b8d-4f1a-9c6e-1d3b5a7c9e2f"), "CandidateForHiringProcess", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "تم اعتماد تقرير نتائج المقابلة مع اختيار هذا المرشح لمواصلة عملية التعيين.", "The interview result report was approved with this candidate selected to proceed to hiring.", 11, true, false, "مرشح لإستكمال اجراءات التعيين", "Candidate For Hiring Process", null, null },
                    { new Guid("8d4e6f1a-2b3c-4d5e-8f9a-1b2c3d4e5f6a"), "WaitingList", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "تم اعتماد تقرير نتائج المقابلة مع وضع هذا المرشح على قائمة الانتظار.", "The interview result report was approved with this candidate placed on the waiting list.", 12, true, false, "قائمة الانتظار", "Waiting List", null, null }
                });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Management",
                keyColumn: "Id",
                keyValue: new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"),
                column: "DepartmentNumber",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Management",
                keyColumn: "Id",
                keyValue: new Guid("4575068a-4f0d-b6cd-8ea8-be680d8dc992"),
                column: "DepartmentNumber",
                value: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -2088380062);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -2055373830);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1640940872);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1068966641);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1054938563);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1041187402);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1039765607);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -828071010);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -782392015);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -231492257);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -176127080);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -8245703);

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("3f6b6e1e-9c2b-4c7e-8b3e-2b6a6e7b9b10"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("5a2c3e4f-7b8d-4f1a-9c6e-1d3b5a7c9e2f"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("8d4e6f1a-2b3c-4d5e-8f9a-1b2c3d4e5f6a"));

            migrationBuilder.DropColumn(
                name: "DepartmentNumber",
                schema: "lkp",
                table: "Management");

            migrationBuilder.DropColumn(
                name: "DefaultBufferMinutes",
                schema: "itv",
                table: "InterviewSchedule");

            migrationBuilder.DropColumn(
                name: "LastReminderSentAt",
                schema: "itv",
                table: "InterviewAppointment");

            migrationBuilder.DropColumn(
                name: "ReminderCount",
                schema: "itv",
                table: "InterviewAppointment");
        }
    }
}
