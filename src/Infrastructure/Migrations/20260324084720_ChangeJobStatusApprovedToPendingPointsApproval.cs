using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeJobStatusApprovedToPendingPointsApproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("5c360b07-157c-630a-254a-9c01587d80a8"),
                columns: new[] { "DescriptionAr", "DescriptionEn", "NameAr", "NameEn" },
                values: new object[] { "الوظيفة قيد اعتماد النقاط.", "Job is pending points approval.", "قيد اعتماد النقاط", "Pending Points Approval" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("5c360b07-157c-630a-254a-9c01587d80a8"),
                columns: new[] { "DescriptionAr", "DescriptionEn", "NameAr", "NameEn" },
                values: new object[] { "تم اعتماد الوظيفة.", "Job was approved.", "معتمدة", "Approved" });
        }
    }
}
