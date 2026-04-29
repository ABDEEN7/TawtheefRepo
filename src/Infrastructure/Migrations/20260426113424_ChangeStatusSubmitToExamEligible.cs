using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeStatusSubmitToExamEligible : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("22ef7e86-28cb-4a30-98bc-7d45f9b44de3"),
                columns: new[] { "BackendName", "DescriptionAr", "DescriptionEn", "NameAr", "NameEn" },
                values: new object[] { "ExamEligible", "المرشح قدم طلبه وجميع بياناته مكتملة وهو مؤهل للاختبار.", "The application is submitted and the candidate is eligible for the exam.", "مرشح للاختبار", "Exam Eligible" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("22ef7e86-28cb-4a30-98bc-7d45f9b44de3"),
                columns: new[] { "BackendName", "DescriptionAr", "DescriptionEn", "NameAr", "NameEn" },
                values: new object[] { "Submitted", "المرشح قدم طلبه وجميع بياناته مكتملة.", "The candidate submitted the application with all required information completed.", "تم التقديم", "Submitted" });
        }
    }
}
