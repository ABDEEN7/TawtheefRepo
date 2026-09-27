using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCandidateExamResultWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExamResultCandidate_ExamResultReportId",
                schema: "hr",
                table: "ExamResultCandidate");

            migrationBuilder.DropColumn(
                name: "IsQualified",
                schema: "hr",
                table: "ExamResultCandidate");

            migrationBuilder.RenameColumn(
                name: "AppliedQualificationScore",
                schema: "hr",
                table: "ExamResultReport",
                newName: "SpecializedQualificationThreshold");

            migrationBuilder.AddColumn<Guid>(
                name: "RescheduledFromTestSessionCandidateId",
                schema: "hr",
                table: "TestSessionCandidate",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DecisionNotes",
                schema: "hr",
                table: "ExamResultReport",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedAt",
                schema: "hr",
                table: "ExamResultReport",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TestSessionCandidateId",
                schema: "hr",
                table: "ExamResultCandidate",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "ExamResultCandidateResultStatus",
                keyColumn: "Id",
                keyValue: new Guid("24bd3d0e-7980-494c-a84d-167a81419030"),
                columns: new[] { "BackendName", "NameAr", "NameEn" },
                values: new object[] { "NotQualifiedForInterview", "غير مؤهل للمقابلة", "Not Qualified for Interview" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "ExamResultCandidateResultStatus",
                keyColumn: "Id",
                keyValue: new Guid("76055057-d4b7-4791-a7df-8ab5c80e116d"),
                columns: new[] { "BackendName", "NameAr", "NameEn" },
                values: new object[] { "QualifiedForInterview", "مؤهل للمقابلة", "Qualified for Interview" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "ExamResultCandidateResultStatus",
                keyColumn: "Id",
                keyValue: new Guid("fb22f73d-0ab6-4031-a4bf-a219bbb07cd5"),
                column: "NameAr",
                value: "لم يكتمل الاختبار");

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "ExamResultCandidateResultStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[] { new Guid("1e5a6d1b-37ad-44c6-8df5-3aa0c3f0462d"), "RequiresAction", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 5, true, false, "بحاجة إلى معالجة", "Requires Action", null, null });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "ExamResultReportStatus",
                keyColumn: "Id",
                keyValue: new Guid("e5032918-cb5d-4ca4-a10a-95002a1e97a0"),
                column: "NameAr",
                value: "معاد للمعالجة");

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "TestAttemptInterruptionResolutionAction",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[] { new Guid("9a8d88c4-11a6-4b8f-97c6-783e21d15f54"), "AcceptCalculatedScore", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 4, true, false, "اعتماد الدرجة المحتسبة", "Accept Calculated Score", null, null });

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidate_RescheduledFromTestSessionCandidateId",
                schema: "hr",
                table: "TestSessionCandidate",
                column: "RescheduledFromTestSessionCandidateId",
                unique: true,
                filter: "[RescheduledFromTestSessionCandidateId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultCandidate_ExamResultReportId_InvitationId",
                schema: "hr",
                table: "ExamResultCandidate",
                columns: new[] { "ExamResultReportId", "InvitationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultCandidate_TestSessionCandidateId",
                schema: "hr",
                table: "ExamResultCandidate",
                column: "TestSessionCandidateId",
                unique: true,
                filter: "[TestSessionCandidateId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamResultCandidate_TestSessionCandidate_TestSessionCandidateId",
                schema: "hr",
                table: "ExamResultCandidate",
                column: "TestSessionCandidateId",
                principalSchema: "hr",
                principalTable: "TestSessionCandidate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestSessionCandidate_TestSessionCandidate_RescheduledFromTestSessionCandidateId",
                schema: "hr",
                table: "TestSessionCandidate",
                column: "RescheduledFromTestSessionCandidateId",
                principalSchema: "hr",
                principalTable: "TestSessionCandidate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamResultCandidate_TestSessionCandidate_TestSessionCandidateId",
                schema: "hr",
                table: "ExamResultCandidate");

            migrationBuilder.DropForeignKey(
                name: "FK_TestSessionCandidate_TestSessionCandidate_RescheduledFromTestSessionCandidateId",
                schema: "hr",
                table: "TestSessionCandidate");

            migrationBuilder.DropIndex(
                name: "IX_TestSessionCandidate_RescheduledFromTestSessionCandidateId",
                schema: "hr",
                table: "TestSessionCandidate");

            migrationBuilder.DropIndex(
                name: "IX_ExamResultCandidate_ExamResultReportId_InvitationId",
                schema: "hr",
                table: "ExamResultCandidate");

            migrationBuilder.DropIndex(
                name: "IX_ExamResultCandidate_TestSessionCandidateId",
                schema: "hr",
                table: "ExamResultCandidate");

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "ExamResultCandidateResultStatus",
                keyColumn: "Id",
                keyValue: new Guid("1e5a6d1b-37ad-44c6-8df5-3aa0c3f0462d"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "TestAttemptInterruptionResolutionAction",
                keyColumn: "Id",
                keyValue: new Guid("9a8d88c4-11a6-4b8f-97c6-783e21d15f54"));

            migrationBuilder.DropColumn(
                name: "RescheduledFromTestSessionCandidateId",
                schema: "hr",
                table: "TestSessionCandidate");

            migrationBuilder.DropColumn(
                name: "ClosedAt",
                schema: "hr",
                table: "ExamResultReport");

            migrationBuilder.DropColumn(
                name: "TestSessionCandidateId",
                schema: "hr",
                table: "ExamResultCandidate");

            migrationBuilder.RenameColumn(
                name: "SpecializedQualificationThreshold",
                schema: "hr",
                table: "ExamResultReport",
                newName: "AppliedQualificationScore");

            migrationBuilder.AlterColumn<string>(
                name: "DecisionNotes",
                schema: "hr",
                table: "ExamResultReport",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsQualified",
                schema: "hr",
                table: "ExamResultCandidate",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "ExamResultCandidateResultStatus",
                keyColumn: "Id",
                keyValue: new Guid("24bd3d0e-7980-494c-a84d-167a81419030"),
                columns: new[] { "BackendName", "NameAr", "NameEn" },
                values: new object[] { "Failed", "راسب", "Failed" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "ExamResultCandidateResultStatus",
                keyColumn: "Id",
                keyValue: new Guid("76055057-d4b7-4791-a7df-8ab5c80e116d"),
                columns: new[] { "BackendName", "NameAr", "NameEn" },
                values: new object[] { "Passed", "ناجح", "Passed" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "ExamResultCandidateResultStatus",
                keyColumn: "Id",
                keyValue: new Guid("fb22f73d-0ab6-4031-a4bf-a219bbb07cd5"),
                column: "NameAr",
                value: "غير مكتمل");

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "ExamResultReportStatus",
                keyColumn: "Id",
                keyValue: new Guid("e5032918-cb5d-4ca4-a10a-95002a1e97a0"),
                column: "NameAr",
                value: "معاد");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultCandidate_ExamResultReportId",
                schema: "hr",
                table: "ExamResultCandidate",
                column: "ExamResultReportId");
        }
    }
}
