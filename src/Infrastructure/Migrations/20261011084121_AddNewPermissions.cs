using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TestSessionCandidate_TestSessionId",
                schema: "hr",
                table: "TestSessionCandidate");

            migrationBuilder.DropIndex(
                name: "IX_TestAttemptQuestion_TestAttemptId",
                schema: "hr",
                table: "TestAttemptQuestion");

            migrationBuilder.DropIndex(
                name: "IX_TestAttemptPart_TestAttemptId",
                schema: "hr",
                table: "TestAttemptPart");

            migrationBuilder.DropIndex(
                name: "IX_TestAttemptInterruption_TestAttemptId",
                schema: "hr",
                table: "TestAttemptInterruption");

            migrationBuilder.DropIndex(
                name: "IX_TestAttempt_StatusId",
                schema: "hr",
                table: "TestAttempt");

            migrationBuilder.DropIndex(
                name: "IX_TestAttempt_TestSessionCandidateId",
                schema: "hr",
                table: "TestAttempt");

            migrationBuilder.DropIndex(
                name: "IX_ExamResultCandidate_ExamResultReportId",
                schema: "hr",
                table: "ExamResultCandidate");

            migrationBuilder.DropColumn(
                name: "AppliedQualificationScore",
                schema: "hr",
                table: "ExamResultReport");

            migrationBuilder.DropColumn(
                name: "IsQualified",
                schema: "hr",
                table: "ExamResultCandidate");

            migrationBuilder.AddColumn<Guid>(
                name: "RescheduledFromTestSessionCandidateId",
                schema: "hr",
                table: "TestSessionCandidate",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "QuestionRevisionId",
                schema: "hr",
                table: "TestAttemptQuestion",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TestAttemptPartId",
                schema: "hr",
                table: "TestAttemptQuestion",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutesSnapshot",
                schema: "hr",
                table: "TestAttemptPart",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveDeadlineAt",
                schema: "hr",
                table: "TestAttemptPart",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ActiveSessionId",
                schema: "hr",
                table: "TestAttempt",
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

            migrationBuilder.AddColumn<decimal>(
                name: "SpecializedQualificationThreshold",
                schema: "hr",
                table: "ExamResultReport",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "TestSessionCandidateId",
                schema: "hr",
                table: "ExamResultCandidate",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CandidateExamConnection",
                schema: "hr",
                columns: table => new
                {
                    ConnectionId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    TestAttemptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastSeenAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateExamConnection", x => x.ConnectionId);
                    table.ForeignKey(
                        name: "FK_CandidateExamConnection_TestAttempt_TestAttemptId",
                        column: x => x.TestAttemptId,
                        principalSchema: "hr",
                        principalTable: "TestAttempt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { -1317131646, "permission", "exams.view-attempt-details", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -1067042838, "permission", "test-slots.send-candidate-notification", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -15171691, "permission", "test-slots.send-candidate-notification", new Guid("b6fc27dc-93ec-4b04-b3a7-311655e87b7f") },
                    { -12199036, "permission", "exams.approve-results", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") }
                });

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
                table: "Permission",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsAssignableToRole", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("5e1b4c26-4560-3c5d-95c0-0c1bb197f555"), "test-slots.send-candidate-notification", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 113, true, true, false, "فترات الاختبار - إرسال إشعار للمرشحين", "Exam Periods - Send Candidate Notification", null, null },
                    { new Guid("6773f879-bc20-ff5c-86af-19a7bb703f35"), "exams.approve-results", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 113, true, true, false, "اختبارات الوظائف - اعتماد النتائج", "Job Exams - Approve Results", null, null },
                    { new Guid("a1ebd47d-0d43-7c5b-992f-54adb52282fb"), "exams.view-attempt-details", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 114, true, true, false, "اختبارات الوظائف - عرض تفاصيل تنفيذ الاختبار", "Job Exams - View Attempt Details", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "TestAttemptInterruptionResolutionAction",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[] { new Guid("9a8d88c4-11a6-4b8f-97c6-783e21d15f54"), "AcceptCalculatedScore", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 4, true, false, "اعتماد الدرجة المحتسبة", "Accept Calculated Score", null, null });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "TestSessionStatus",
                keyColumn: "Id",
                keyValue: new Guid("16b3ac19-2ff1-491f-acca-7663c19c226b"),
                columns: new[] { "BackendName", "NameAr", "NameEn" },
                values: new object[] { "InProgress", "قيد التنفيذ", "In Progress" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "TestSessionStatus",
                keyColumn: "Id",
                keyValue: new Guid("6c7b42ab-a356-47e6-93a1-a5e5a2dd09d4"),
                columns: new[] { "DisplayOrder", "NameAr" },
                values: new object[] { 9, "معادة" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "TestSessionStatus",
                keyColumn: "Id",
                keyValue: new Guid("f44d7a03-9eaf-498c-a3ad-6b6810b4c2da"),
                column: "DisplayOrder",
                value: 8);

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "TestSessionStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[] { new Guid("3ccad4ec-72a6-43bc-9299-050b399c7aa5"), "PendingApproval", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 7, true, false, "بانتظار الاعتماد", "Pending Approval", null, null });

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidate_RescheduledFromTestSessionCandidateId",
                schema: "hr",
                table: "TestSessionCandidate",
                column: "RescheduledFromTestSessionCandidateId",
                unique: true,
                filter: "[RescheduledFromTestSessionCandidateId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidate_TestSessionId_InvitationId",
                schema: "hr",
                table: "TestSessionCandidate",
                columns: new[] { "TestSessionId", "InvitationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptQuestion_QuestionRevisionId",
                schema: "hr",
                table: "TestAttemptQuestion",
                column: "QuestionRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptQuestion_TestAttemptId_QuestionId",
                schema: "hr",
                table: "TestAttemptQuestion",
                columns: new[] { "TestAttemptId", "QuestionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptQuestion_TestAttemptPartId_OrderNo",
                schema: "hr",
                table: "TestAttemptQuestion",
                columns: new[] { "TestAttemptPartId", "OrderNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptPart_TestAttemptId_ExamPartId",
                schema: "hr",
                table: "TestAttemptPart",
                columns: new[] { "TestAttemptId", "ExamPartId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptInterruption_TestAttemptId_StatusId_ResolutionActionId_ResolvedAt",
                schema: "hr",
                table: "TestAttemptInterruption",
                columns: new[] { "TestAttemptId", "StatusId", "ResolutionActionId", "ResolvedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_TestAttempt_ActiveSessionId",
                schema: "hr",
                table: "TestAttempt",
                column: "ActiveSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttempt_StatusId_StartedAt",
                schema: "hr",
                table: "TestAttempt",
                columns: new[] { "StatusId", "StartedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_TestAttempt_TestSessionCandidateId",
                schema: "hr",
                table: "TestAttempt",
                column: "TestSessionCandidateId",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_CandidateExamConnection_TestAttemptId_ExpiresAt",
                schema: "hr",
                table: "CandidateExamConnection",
                columns: new[] { "TestAttemptId", "ExpiresAt" });

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
                name: "FK_TestAttemptQuestion_QuestionRevision_QuestionRevisionId",
                schema: "hr",
                table: "TestAttemptQuestion",
                column: "QuestionRevisionId",
                principalSchema: "hr",
                principalTable: "QuestionRevision",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestAttemptQuestion_TestAttemptPart_TestAttemptPartId",
                schema: "hr",
                table: "TestAttemptQuestion",
                column: "TestAttemptPartId",
                principalSchema: "hr",
                principalTable: "TestAttemptPart",
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
                name: "FK_TestAttemptQuestion_QuestionRevision_QuestionRevisionId",
                schema: "hr",
                table: "TestAttemptQuestion");

            migrationBuilder.DropForeignKey(
                name: "FK_TestAttemptQuestion_TestAttemptPart_TestAttemptPartId",
                schema: "hr",
                table: "TestAttemptQuestion");

            migrationBuilder.DropForeignKey(
                name: "FK_TestSessionCandidate_TestSessionCandidate_RescheduledFromTestSessionCandidateId",
                schema: "hr",
                table: "TestSessionCandidate");

            migrationBuilder.DropTable(
                name: "CandidateExamConnection",
                schema: "hr");

            migrationBuilder.DropIndex(
                name: "IX_TestSessionCandidate_RescheduledFromTestSessionCandidateId",
                schema: "hr",
                table: "TestSessionCandidate");

            migrationBuilder.DropIndex(
                name: "IX_TestSessionCandidate_TestSessionId_InvitationId",
                schema: "hr",
                table: "TestSessionCandidate");

            migrationBuilder.DropIndex(
                name: "IX_TestAttemptQuestion_QuestionRevisionId",
                schema: "hr",
                table: "TestAttemptQuestion");

            migrationBuilder.DropIndex(
                name: "IX_TestAttemptQuestion_TestAttemptId_QuestionId",
                schema: "hr",
                table: "TestAttemptQuestion");

            migrationBuilder.DropIndex(
                name: "IX_TestAttemptQuestion_TestAttemptPartId_OrderNo",
                schema: "hr",
                table: "TestAttemptQuestion");

            migrationBuilder.DropIndex(
                name: "IX_TestAttemptPart_TestAttemptId_ExamPartId",
                schema: "hr",
                table: "TestAttemptPart");

            migrationBuilder.DropIndex(
                name: "IX_TestAttemptInterruption_TestAttemptId_StatusId_ResolutionActionId_ResolvedAt",
                schema: "hr",
                table: "TestAttemptInterruption");

            migrationBuilder.DropIndex(
                name: "IX_TestAttempt_ActiveSessionId",
                schema: "hr",
                table: "TestAttempt");

            migrationBuilder.DropIndex(
                name: "IX_TestAttempt_StatusId_StartedAt",
                schema: "hr",
                table: "TestAttempt");

            migrationBuilder.DropIndex(
                name: "IX_TestAttempt_TestSessionCandidateId",
                schema: "hr",
                table: "TestAttempt");

            migrationBuilder.DropIndex(
                name: "IX_ExamResultCandidate_ExamResultReportId_InvitationId",
                schema: "hr",
                table: "ExamResultCandidate");

            migrationBuilder.DropIndex(
                name: "IX_ExamResultCandidate_TestSessionCandidateId",
                schema: "hr",
                table: "ExamResultCandidate");

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1317131646);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1067042838);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -15171691);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -12199036);

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "ExamResultCandidateResultStatus",
                keyColumn: "Id",
                keyValue: new Guid("1e5a6d1b-37ad-44c6-8df5-3aa0c3f0462d"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("5e1b4c26-4560-3c5d-95c0-0c1bb197f555"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("6773f879-bc20-ff5c-86af-19a7bb703f35"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("a1ebd47d-0d43-7c5b-992f-54adb52282fb"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "TestAttemptInterruptionResolutionAction",
                keyColumn: "Id",
                keyValue: new Guid("9a8d88c4-11a6-4b8f-97c6-783e21d15f54"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "TestSessionStatus",
                keyColumn: "Id",
                keyValue: new Guid("3ccad4ec-72a6-43bc-9299-050b399c7aa5"));

            migrationBuilder.DropColumn(
                name: "RescheduledFromTestSessionCandidateId",
                schema: "hr",
                table: "TestSessionCandidate");

            migrationBuilder.DropColumn(
                name: "QuestionRevisionId",
                schema: "hr",
                table: "TestAttemptQuestion");

            migrationBuilder.DropColumn(
                name: "TestAttemptPartId",
                schema: "hr",
                table: "TestAttemptQuestion");

            migrationBuilder.DropColumn(
                name: "DurationMinutesSnapshot",
                schema: "hr",
                table: "TestAttemptPart");

            migrationBuilder.DropColumn(
                name: "EffectiveDeadlineAt",
                schema: "hr",
                table: "TestAttemptPart");

            migrationBuilder.DropColumn(
                name: "ActiveSessionId",
                schema: "hr",
                table: "TestAttempt");

            migrationBuilder.DropColumn(
                name: "ClosedAt",
                schema: "hr",
                table: "ExamResultReport");

            migrationBuilder.DropColumn(
                name: "SpecializedQualificationThreshold",
                schema: "hr",
                table: "ExamResultReport");

            migrationBuilder.DropColumn(
                name: "TestSessionCandidateId",
                schema: "hr",
                table: "ExamResultCandidate");

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

            migrationBuilder.AddColumn<decimal>(
                name: "AppliedQualificationScore",
                schema: "hr",
                table: "ExamResultReport",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

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

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "TestSessionStatus",
                keyColumn: "Id",
                keyValue: new Guid("16b3ac19-2ff1-491f-acca-7663c19c226b"),
                columns: new[] { "BackendName", "NameAr", "NameEn" },
                values: new object[] { "PendingApproval", "بانتظار الاعتماد", "Pending Approval" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "TestSessionStatus",
                keyColumn: "Id",
                keyValue: new Guid("6c7b42ab-a356-47e6-93a1-a5e5a2dd09d4"),
                columns: new[] { "DisplayOrder", "NameAr" },
                values: new object[] { 8, "معاد" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "TestSessionStatus",
                keyColumn: "Id",
                keyValue: new Guid("f44d7a03-9eaf-498c-a3ad-6b6810b4c2da"),
                column: "DisplayOrder",
                value: 7);

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidate_TestSessionId",
                schema: "hr",
                table: "TestSessionCandidate",
                column: "TestSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptQuestion_TestAttemptId",
                schema: "hr",
                table: "TestAttemptQuestion",
                column: "TestAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptPart_TestAttemptId",
                schema: "hr",
                table: "TestAttemptPart",
                column: "TestAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptInterruption_TestAttemptId",
                schema: "hr",
                table: "TestAttemptInterruption",
                column: "TestAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttempt_StatusId",
                schema: "hr",
                table: "TestAttempt",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttempt_TestSessionCandidateId",
                schema: "hr",
                table: "TestAttempt",
                column: "TestSessionCandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultCandidate_ExamResultReportId",
                schema: "hr",
                table: "ExamResultCandidate",
                column: "ExamResultReportId");
        }
    }
}
