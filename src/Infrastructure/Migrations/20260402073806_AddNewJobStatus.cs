using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewJobStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("0d21e063-48d3-d320-4078-d85a7c2bf622"),
                column: "DisplayOrder",
                value: 8);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("1e3ecad5-63fa-a11c-7acb-dd4c62ef74fd"),
                column: "DisplayOrder",
                value: 9);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("5c360b07-157c-630a-254a-9c01587d80a8"),
                columns: new[] { "BackendName", "DescriptionAr", "DescriptionEn", "NameAr", "NameEn" },
                values: new object[] { "PendingPointConfiguration", "الوظيفة قيد اعداد النقاط.", "Job is pending point configuration.", "قيد اعداد النقاط", "Pending Point Configuration" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("f2e7748a-12fe-53ac-fbdf-e989f8aa498a"),
                column: "DisplayOrder",
                value: 10);

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "JobStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[] { new Guid("2c3f9a4e-7d1b-4e9a-8c3d-1f2b3c4d5e6f"), "PendingPointApproval", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "الوظيفة قيد اعتماد النقاط.", "Job is pending point approval.", 7, true, false, "قيد اعتماد النقاط", "Pending Point Approval", null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("2c3f9a4e-7d1b-4e9a-8c3d-1f2b3c4d5e6f"));

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("0d21e063-48d3-d320-4078-d85a7c2bf622"),
                column: "DisplayOrder",
                value: 7);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("1e3ecad5-63fa-a11c-7acb-dd4c62ef74fd"),
                column: "DisplayOrder",
                value: 8);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("5c360b07-157c-630a-254a-9c01587d80a8"),
                columns: new[] { "BackendName", "DescriptionAr", "DescriptionEn", "NameAr", "NameEn" },
                values: new object[] { "Approved", "الوظيفة قيد اعتماد النقاط.", "Job is pending points approval.", "قيد اعتماد النقاط", "Pending Points Approval" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("f2e7748a-12fe-53ac-fbdf-e989f8aa498a"),
                column: "DisplayOrder",
                value: 9);
        }
    }
}
