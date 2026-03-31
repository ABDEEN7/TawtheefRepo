using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIndexUniqeOnJobEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Job_Unique_JobTitle_Department_Category_SubMajor",
                schema: "hr",
                table: "Job");

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("6608f560-4dc0-4f2a-a190-6743a9a8c5cb"),
                columns: new[] { "DescriptionAr", "NameAr" },
                values: new object[] { "قام المرشح برفع المرفقات الإلزامية وهو بانتظار اعتماد الموارد البشرية.", "بانتظار اعتماد المرفقات" });

            migrationBuilder.CreateIndex(
                name: "IX_Job_JobTitleId",
                schema: "hr",
                table: "Job",
                column: "JobTitleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Job_JobTitleId",
                schema: "hr",
                table: "Job");

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("6608f560-4dc0-4f2a-a190-6743a9a8c5cb"),
                columns: new[] { "DescriptionAr", "NameAr" },
                values: new object[] { "قام المرشح برفع المرفقات الإلزامية وهو بانتظار موافقة الموارد البشرية.", "بانتظار موافقة المرفقات" });

            migrationBuilder.CreateIndex(
                name: "IX_Job_Unique_JobTitle_Department_Category_SubMajor",
                schema: "hr",
                table: "Job",
                columns: new[] { "JobTitleId", "DepartmentId", "JobCategoryId", "SubMajorId" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }
    }
}
