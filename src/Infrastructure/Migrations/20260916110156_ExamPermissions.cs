using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExamPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamCategory_ExamCategoryType_CategoryId",
                schema: "hr",
                table: "ExamCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_InterviewCommittee_InterviewCommitteeType_CommitteeTypeId",
                schema: "itv",
                table: "InterviewCommittee");

            migrationBuilder.DropForeignKey(
                name: "FK_InterviewCommittee_JobInterviewTemplate_JobInterviewTemplateId",
                schema: "itv",
                table: "InterviewCommittee");

            migrationBuilder.DropForeignKey(
                name: "FK_InterviewSchedule_JobInterviewTemplate_JobInterviewTemplateId",
                schema: "itv",
                table: "InterviewSchedule");

            migrationBuilder.DropTable(
                name: "ExamCategoryType",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "InterviewCommitteeType",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "JobInterviewTemplate",
                schema: "itv");

            migrationBuilder.DropIndex(
                name: "IX_InterviewCommittee_CommitteeTypeId",
                schema: "itv",
                table: "InterviewCommittee");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("9fd109fb-a2ad-4637-86f2-2be13ccfa6d4"));

            migrationBuilder.DropColumn(
                name: "CommitteeTypeId",
                schema: "itv",
                table: "InterviewCommittee");

            migrationBuilder.RenameColumn(
                name: "JobInterviewTemplateId",
                schema: "itv",
                table: "InterviewSchedule",
                newName: "InterviewTemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_InterviewSchedule_JobInterviewTemplateId",
                schema: "itv",
                table: "InterviewSchedule",
                newName: "IX_InterviewSchedule_InterviewTemplateId");

            migrationBuilder.RenameColumn(
                name: "JobInterviewTemplateId",
                schema: "itv",
                table: "InterviewCommittee",
                newName: "InterviewTemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_InterviewCommittee_JobInterviewTemplateId",
                schema: "itv",
                table: "InterviewCommittee",
                newName: "IX_InterviewCommittee_InterviewTemplateId");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                schema: "hr",
                table: "ExamCategory",
                newName: "QuestionBankTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ExamCategory_CategoryId",
                schema: "hr",
                table: "ExamCategory",
                newName: "IX_ExamCategory_QuestionBankTypeId");

            migrationBuilder.RenameColumn(
                name: "ApprovedById",
                schema: "hr",
                table: "Exam",
                newName: "DecisionById");

            migrationBuilder.RenameColumn(
                name: "ApprovedAt",
                schema: "hr",
                table: "Exam",
                newName: "DecisionAt");

            migrationBuilder.RenameIndex(
                name: "IX_Exam_ApprovedById",
                schema: "hr",
                table: "Exam",
                newName: "IX_Exam_DecisionById");

            migrationBuilder.AlterColumn<string>(
                name: "ExamNo",
                schema: "hr",
                table: "Exam",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { -2073191718, "permission", "interview-evaluation-bank.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -2029722188, "permission", "exams.create", new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd") },
                    { -1936895793, "permission", "interview-evaluation-template.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -1638583860, "permission", "interview-evaluation-bank.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -1603357864, "permission", "exams.view", new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd") },
                    { -1509642356, "permission", "interview-committee.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -1395855390, "permission", "interview-evaluation-bank.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -891317680, "permission", "exams.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -621503787, "permission", "interview-evaluation-template.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -571981562, "permission", "exams.create", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -400326806, "permission", "interview-evaluation-bank.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -288928577, "permission", "interview-committee.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -225124883, "permission", "interview-evaluation-template.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -213123505, "permission", "interview-evaluation-template.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -180002287, "permission", "interview-committee.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -6261347, "permission", "examWorkflowActions", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -2227263, "permission", "interview-committee.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "ExamStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[] { new Guid("c1d7506c-ff0a-42f5-9adf-cacf6d9d0681"), "REJECTED", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 6, true, false, "مرفوض", "Rejected", null, null });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Permission",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsAssignableToRole", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("18894fd8-d733-ab54-af0c-1d7ca8e76a05"), "interview-evaluation-bank.manage", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 103, true, true, false, "بنك المحاور والمعايير - إدارة", "Interview Evaluation Bank - Manage", null, null },
                    { new Guid("2d786f5b-0e0b-d55f-a12a-404d57f96b58"), "exams.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 106, true, true, false, "اختبارات الوظائف - عرض", "Job Exams - View", null, null },
                    { new Guid("42a6cccc-2dd1-3c5c-8fbf-3b0333db0aa2"), "interview-evaluation-template.manage", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 105, true, true, false, "قوالب المقابلات - إدارة", "Interview Evaluation Template - Manage", null, null },
                    { new Guid("46b0dbe9-af0c-f557-911a-11d955176a60"), "interview-committee.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 106, true, true, false, "لجان المقابلات - عرض", "Interview Committee - View", null, null },
                    { new Guid("46e7fb1a-7503-355f-8f83-016cfef177c1"), "examWorkflowActions", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 108, true, true, false, "إجراءات سير عمل اختبارات الوظائف", "Job Exams - Workflow Actions", null, null },
                    { new Guid("5918876e-41bf-6e59-aeae-4d5546534f4f"), "interview-committee.manage", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 107, true, true, false, "لجان المقابلات - إدارة", "Interview Committee - Manage", null, null },
                    { new Guid("6c983939-fc0e-7b5b-967c-90cb95289472"), "interview-evaluation-bank.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 102, true, true, false, "بنك المحاور والمعايير - عرض", "Interview Evaluation Bank - View", null, null },
                    { new Guid("909a691b-dde0-8d55-850e-6b9c12a67178"), "exams.create", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 107, true, true, false, "اختبارات الوظائف - إنشاء", "Job Exams - Create", null, null },
                    { new Guid("de1163e5-fe04-0a54-893f-e3e1ca98df10"), "interview-evaluation-template.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 104, true, true, false, "قوالب المقابلات - عرض", "Interview Evaluation Template - View", null, null }
                });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "QuestionBankType",
                keyColumn: "Id",
                keyValue: new Guid("0ef8f0d9-02d7-4863-9bfe-e63355f3686e"),
                column: "BackendName",
                value: "Educational");

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "QuestionBankType",
                keyColumn: "Id",
                keyValue: new Guid("686928db-b630-4689-b265-ae17eded73cf"),
                column: "BackendName",
                value: "Skills");

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "QuestionBankType",
                keyColumn: "Id",
                keyValue: new Guid("a7f9646b-fcc3-4a00-944c-ceed82acd557"),
                column: "BackendName",
                value: "Specialized");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_ExamNo",
                schema: "hr",
                table: "Exam",
                column: "ExamNo",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Exam_AspNetUsers_DecisionById",
                schema: "hr",
                table: "Exam",
                column: "DecisionById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamCategory_QuestionBankType_QuestionBankTypeId",
                schema: "hr",
                table: "ExamCategory",
                column: "QuestionBankTypeId",
                principalSchema: "lkp",
                principalTable: "QuestionBankType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterviewCommittee_InterviewTemplate_InterviewTemplateId",
                schema: "itv",
                table: "InterviewCommittee",
                column: "InterviewTemplateId",
                principalSchema: "itv",
                principalTable: "InterviewTemplate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterviewSchedule_InterviewTemplate_InterviewTemplateId",
                schema: "itv",
                table: "InterviewSchedule",
                column: "InterviewTemplateId",
                principalSchema: "itv",
                principalTable: "InterviewTemplate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exam_AspNetUsers_DecisionById",
                schema: "hr",
                table: "Exam");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamCategory_QuestionBankType_QuestionBankTypeId",
                schema: "hr",
                table: "ExamCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_InterviewCommittee_InterviewTemplate_InterviewTemplateId",
                schema: "itv",
                table: "InterviewCommittee");

            migrationBuilder.DropForeignKey(
                name: "FK_InterviewSchedule_InterviewTemplate_InterviewTemplateId",
                schema: "itv",
                table: "InterviewSchedule");

            migrationBuilder.DropIndex(
                name: "IX_Exam_ExamNo",
                schema: "hr",
                table: "Exam");

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -2073191718);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -2029722188);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1936895793);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1638583860);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1603357864);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1509642356);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1395855390);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -891317680);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -621503787);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -571981562);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -400326806);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -288928577);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -225124883);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -213123505);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -180002287);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -6261347);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -2227263);

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "ExamStatus",
                keyColumn: "Id",
                keyValue: new Guid("c1d7506c-ff0a-42f5-9adf-cacf6d9d0681"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("18894fd8-d733-ab54-af0c-1d7ca8e76a05"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("2d786f5b-0e0b-d55f-a12a-404d57f96b58"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("42a6cccc-2dd1-3c5c-8fbf-3b0333db0aa2"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("46b0dbe9-af0c-f557-911a-11d955176a60"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("46e7fb1a-7503-355f-8f83-016cfef177c1"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("5918876e-41bf-6e59-aeae-4d5546534f4f"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("6c983939-fc0e-7b5b-967c-90cb95289472"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("909a691b-dde0-8d55-850e-6b9c12a67178"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("de1163e5-fe04-0a54-893f-e3e1ca98df10"));

            migrationBuilder.RenameColumn(
                name: "InterviewTemplateId",
                schema: "itv",
                table: "InterviewSchedule",
                newName: "JobInterviewTemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_InterviewSchedule_InterviewTemplateId",
                schema: "itv",
                table: "InterviewSchedule",
                newName: "IX_InterviewSchedule_JobInterviewTemplateId");

            migrationBuilder.RenameColumn(
                name: "InterviewTemplateId",
                schema: "itv",
                table: "InterviewCommittee",
                newName: "JobInterviewTemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_InterviewCommittee_InterviewTemplateId",
                schema: "itv",
                table: "InterviewCommittee",
                newName: "IX_InterviewCommittee_JobInterviewTemplateId");

            migrationBuilder.RenameColumn(
                name: "QuestionBankTypeId",
                schema: "hr",
                table: "ExamCategory",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ExamCategory_QuestionBankTypeId",
                schema: "hr",
                table: "ExamCategory",
                newName: "IX_ExamCategory_CategoryId");

            migrationBuilder.RenameColumn(
                name: "DecisionById",
                schema: "hr",
                table: "Exam",
                newName: "ApprovedById");

            migrationBuilder.RenameColumn(
                name: "DecisionAt",
                schema: "hr",
                table: "Exam",
                newName: "ApprovedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Exam_DecisionById",
                schema: "hr",
                table: "Exam",
                newName: "IX_Exam_ApprovedById");

            migrationBuilder.AddColumn<Guid>(
                name: "CommitteeTypeId",
                schema: "itv",
                table: "InterviewCommittee",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExamNo",
                schema: "hr",
                table: "Exam",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "ExamCategoryType",
                schema: "lkp",
                columns: table => new
                {
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamCategoryType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamCategoryType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamCategoryType_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamCategoryType_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterviewCommitteeType",
                schema: "lkp",
                columns: table => new
                {
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewCommitteeType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewCommitteeType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewCommitteeType_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewCommitteeType_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobInterviewTemplate",
                schema: "itv",
                columns: table => new
                {
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InterviewTemplateVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DecisionNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobInterviewTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobInterviewTemplate_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JobInterviewTemplate_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobInterviewTemplate_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobInterviewTemplate_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobInterviewTemplate_InterviewTemplateVersion_InterviewTemplateVersionId",
                        column: x => x.InterviewTemplateVersionId,
                        principalSchema: "itv",
                        principalTable: "InterviewTemplateVersion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobInterviewTemplate_Job_JobId",
                        column: x => x.JobId,
                        principalSchema: "hr",
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Avatar", "ConcurrencyStamp", "CreatedById", "CreatedDate", "CurrentAuthToken", "DeletedById", "DeletedDate", "Discriminator", "Email", "EmailConfirmed", "EmployeeProfileId", "FullNameAr", "FullNameEn", "IsBlocked", "IsDeleted", "LastLoginDate", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "OtpAttempts", "OtpExpiry", "OtpLockedUntilUtc", "OtpReference", "OtpSendWindowStartUtc", "OtpSendsInWindow", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "PreferredLanguage", "SecurityStamp", "TwoFactorEnabled", "UpdatedById", "UpdatedDate", "UserName", "UserTypeId" },
                values: new object[] { new Guid("9fd109fb-a2ad-4637-86f2-2be13ccfa6d4"), 0, null, "75a677a7-c93d-4940-8666-4d648343104c", null, new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "EmployeeUser", "na.almarri@edu.gov.qa", true, null, "na.almarri", "na.almarri", false, false, null, false, null, "NA.ALMARRI@EDU.GOV.QA", "NA.ALMARRI@EDU.GOV.QA", 0, null, null, null, null, 0, null, null, false, null, "a984b6f5-e904-44b0-8d0d-5e93c07b1510", false, null, null, "na.almarri@edu.gov.qa", new Guid("a1b2c3d4-e5f6-4879-8a3b-5c6d7e8f9a0b") });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "ExamCategoryType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("6552e6b6-b339-4476-ab86-60f225ac3582"), "Skills", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "مهارات", "Skills", null, null },
                    { new Guid("6b165f67-6da7-4715-a815-e9f735704b49"), "Educational", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "تربوي", "Educational", null, null },
                    { new Guid("e7c03fb9-c9a3-4187-a731-88873a230983"), "Specialized", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "تخصصي", "Specialized", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "InterviewCommitteeType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("8e6f2a10-6a1e-4b8a-9f0a-1a2b3c4d5e01"), "Academic", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "أكاديمية", "Academic", null, null },
                    { new Guid("8e6f2a10-6a1e-4b8a-9f0a-1a2b3c4d5e02"), "Administrative", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "إدارية", "Administrative", null, null },
                    { new Guid("8e6f2a10-6a1e-4b8a-9f0a-1a2b3c4d5e03"), "Labor", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "عمالية", "Labor", null, null },
                    { new Guid("8e6f2a10-6a1e-4b8a-9f0a-1a2b3c4d5e04"), "Other", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 4, true, false, "أخرى", "Other", null, null }
                });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "QuestionBankType",
                keyColumn: "Id",
                keyValue: new Guid("0ef8f0d9-02d7-4863-9bfe-e63355f3686e"),
                column: "BackendName",
                value: "EDUCATIONAL");

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "QuestionBankType",
                keyColumn: "Id",
                keyValue: new Guid("686928db-b630-4689-b265-ae17eded73cf"),
                column: "BackendName",
                value: "SKILLS");

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "QuestionBankType",
                keyColumn: "Id",
                keyValue: new Guid("a7f9646b-fcc3-4a00-944c-ceed82acd557"),
                column: "BackendName",
                value: "SPECIALIZED");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommittee_CommitteeTypeId",
                schema: "itv",
                table: "InterviewCommittee",
                column: "CommitteeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamCategoryType_BackendName",
                schema: "lkp",
                table: "ExamCategoryType",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamCategoryType_CreatedById",
                schema: "lkp",
                table: "ExamCategoryType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamCategoryType_CreatedDate",
                schema: "lkp",
                table: "ExamCategoryType",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ExamCategoryType_DeletedById",
                schema: "lkp",
                table: "ExamCategoryType",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamCategoryType_DisplayOrder",
                schema: "lkp",
                table: "ExamCategoryType",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ExamCategoryType_IsDeleted",
                schema: "lkp",
                table: "ExamCategoryType",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ExamCategoryType_UpdatedById",
                schema: "lkp",
                table: "ExamCategoryType",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommitteeType_BackendName",
                schema: "lkp",
                table: "InterviewCommitteeType",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommitteeType_CreatedById",
                schema: "lkp",
                table: "InterviewCommitteeType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommitteeType_CreatedDate",
                schema: "lkp",
                table: "InterviewCommitteeType",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommitteeType_DeletedById",
                schema: "lkp",
                table: "InterviewCommitteeType",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommitteeType_DisplayOrder",
                schema: "lkp",
                table: "InterviewCommitteeType",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommitteeType_IsDeleted",
                schema: "lkp",
                table: "InterviewCommitteeType",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommitteeType_UpdatedById",
                schema: "lkp",
                table: "InterviewCommitteeType",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobInterviewTemplate_ApprovedById",
                schema: "itv",
                table: "JobInterviewTemplate",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobInterviewTemplate_CreatedById",
                schema: "itv",
                table: "JobInterviewTemplate",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobInterviewTemplate_CreatedDate",
                schema: "itv",
                table: "JobInterviewTemplate",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_JobInterviewTemplate_DeletedById",
                schema: "itv",
                table: "JobInterviewTemplate",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobInterviewTemplate_InterviewTemplateVersionId",
                schema: "itv",
                table: "JobInterviewTemplate",
                column: "InterviewTemplateVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_JobInterviewTemplate_IsDeleted",
                schema: "itv",
                table: "JobInterviewTemplate",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobInterviewTemplate_Status",
                schema: "itv",
                table: "JobInterviewTemplate",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_JobInterviewTemplate_UpdatedById",
                schema: "itv",
                table: "JobInterviewTemplate",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "UX_JobInterviewTemplate_Active",
                schema: "itv",
                table: "JobInterviewTemplate",
                column: "JobId",
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamCategory_ExamCategoryType_CategoryId",
                schema: "hr",
                table: "ExamCategory",
                column: "CategoryId",
                principalSchema: "lkp",
                principalTable: "ExamCategoryType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterviewCommittee_InterviewCommitteeType_CommitteeTypeId",
                schema: "itv",
                table: "InterviewCommittee",
                column: "CommitteeTypeId",
                principalSchema: "lkp",
                principalTable: "InterviewCommitteeType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterviewCommittee_JobInterviewTemplate_JobInterviewTemplateId",
                schema: "itv",
                table: "InterviewCommittee",
                column: "JobInterviewTemplateId",
                principalSchema: "itv",
                principalTable: "JobInterviewTemplate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterviewSchedule_JobInterviewTemplate_JobInterviewTemplateId",
                schema: "itv",
                table: "InterviewSchedule",
                column: "JobInterviewTemplateId",
                principalSchema: "itv",
                principalTable: "JobInterviewTemplate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
