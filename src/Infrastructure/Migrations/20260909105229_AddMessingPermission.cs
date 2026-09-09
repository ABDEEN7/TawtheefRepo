using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMessingPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "itv");

            migrationBuilder.AddColumn<Guid>(
                name: "QuestionId",
                schema: "hr",
                table: "TestAttemptQuestion",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SelectedQuestionRevisionOptionId",
                schema: "hr",
                table: "TestAttemptQuestion",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "QuestionBankVersionId",
                schema: "hr",
                table: "ExamCategory",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ExamNo",
                schema: "hr",
                table: "Exam",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "InterviewApprovalAction",
                schema: "itv",
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
                    EntityType = table.Column<string>(type: "varchar(60)", nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<byte>(type: "tinyint", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewApprovalAction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewApprovalAction_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewApprovalAction_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewApprovalAction_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterviewAuditLog",
                schema: "itv",
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
                    EntityType = table.Column<string>(type: "varchar(60)", nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "varchar(60)", nullable: false),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewAuditLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewAuditLog_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewAuditLog_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewAuditLog_AspNetUsers_UpdatedById",
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                name: "InterviewEvaluationAxis",
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
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewEvaluationAxis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewEvaluationAxis_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewEvaluationAxis_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewEvaluationAxis_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterviewNoteType",
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewNoteType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewNoteType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewNoteType_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewNoteType_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterviewOrganizationScope",
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewOrganizationScope", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewOrganizationScope_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewOrganizationScope_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewOrganizationScope_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterviewEvaluationCriterion",
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
                    InterviewEvaluationAxisId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewEvaluationCriterion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewEvaluationCriterion_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewEvaluationCriterion_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewEvaluationCriterion_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewEvaluationCriterion_InterviewEvaluationAxis_InterviewEvaluationAxisId",
                        column: x => x.InterviewEvaluationAxisId,
                        principalSchema: "itv",
                        principalTable: "InterviewEvaluationAxis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterviewTemplate",
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
                    TitleAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OrganizationScopeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    JobTitleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewTemplate_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewTemplate_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewTemplate_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewTemplate_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "lkp",
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewTemplate_InterviewOrganizationScope_OrganizationScopeId",
                        column: x => x.OrganizationScopeId,
                        principalSchema: "lkp",
                        principalTable: "InterviewOrganizationScope",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewTemplate_JobTitle_JobTitleId",
                        column: x => x.JobTitleId,
                        principalSchema: "lkp",
                        principalTable: "JobTitle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterviewTemplateVersion",
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
                    InterviewTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VersionNo = table.Column<int>(type: "int", nullable: false),
                    FinalScore = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    QualificationScore = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    CalculationMethod = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DecisionNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewTemplateVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewTemplateVersion_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InterviewTemplateVersion_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewTemplateVersion_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewTemplateVersion_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewTemplateVersion_InterviewTemplate_InterviewTemplateId",
                        column: x => x.InterviewTemplateId,
                        principalSchema: "itv",
                        principalTable: "InterviewTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterviewTemplateEvaluationAxis",
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
                    InterviewTemplateVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterviewEvaluationAxisId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaxScore = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    QualificationScore = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    OrderNo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewTemplateEvaluationAxis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewTemplateEvaluationAxis_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewTemplateEvaluationAxis_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewTemplateEvaluationAxis_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewTemplateEvaluationAxis_InterviewEvaluationAxis_InterviewEvaluationAxisId",
                        column: x => x.InterviewEvaluationAxisId,
                        principalSchema: "itv",
                        principalTable: "InterviewEvaluationAxis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewTemplateEvaluationAxis_InterviewTemplateVersion_InterviewTemplateVersionId",
                        column: x => x.InterviewTemplateVersionId,
                        principalSchema: "itv",
                        principalTable: "InterviewTemplateVersion",
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
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterviewTemplateVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ApprovedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DecisionNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "InterviewTemplateEvaluationCriterion",
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
                    InterviewTemplateEvaluationAxisId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterviewEvaluationCriterionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxScore = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewTemplateEvaluationCriterion", x => x.Id);
                    table.CheckConstraint("CK_TemplateCriterion_Source", "([InterviewEvaluationCriterionId] IS NOT NULL AND [NameAr] IS NULL) OR ([InterviewEvaluationCriterionId] IS NULL AND [NameAr] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_InterviewTemplateEvaluationCriterion_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewTemplateEvaluationCriterion_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewTemplateEvaluationCriterion_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewTemplateEvaluationCriterion_InterviewEvaluationCriterion_InterviewEvaluationCriterionId",
                        column: x => x.InterviewEvaluationCriterionId,
                        principalSchema: "itv",
                        principalTable: "InterviewEvaluationCriterion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewTemplateEvaluationCriterion_InterviewTemplateEvaluationAxis_InterviewTemplateEvaluationAxisId",
                        column: x => x.InterviewTemplateEvaluationAxisId,
                        principalSchema: "itv",
                        principalTable: "InterviewTemplateEvaluationAxis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterviewCommittee",
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
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobInterviewTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CommitteeTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ScopeDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DecisionNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewCommittee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewCommittee_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InterviewCommittee_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewCommittee_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewCommittee_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewCommittee_InterviewCommitteeType_CommitteeTypeId",
                        column: x => x.CommitteeTypeId,
                        principalSchema: "lkp",
                        principalTable: "InterviewCommitteeType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewCommittee_JobInterviewTemplate_JobInterviewTemplateId",
                        column: x => x.JobInterviewTemplateId,
                        principalSchema: "itv",
                        principalTable: "JobInterviewTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewCommittee_Job_JobId",
                        column: x => x.JobId,
                        principalSchema: "hr",
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterviewSchedule",
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
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobInterviewTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TitleAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DefaultInterviewType = table.Column<int>(type: "int", nullable: false),
                    DefaultDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApprovedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DecisionNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewSchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewSchedule_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InterviewSchedule_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewSchedule_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewSchedule_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewSchedule_JobInterviewTemplate_JobInterviewTemplateId",
                        column: x => x.JobInterviewTemplateId,
                        principalSchema: "itv",
                        principalTable: "JobInterviewTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewSchedule_Job_JobId",
                        column: x => x.JobId,
                        principalSchema: "hr",
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterviewCommitteeMember",
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
                    InterviewCommitteeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    ParticipatesInEvaluation = table.Column<bool>(type: "bit", nullable: false),
                    EvaluationScope = table.Column<int>(type: "int", nullable: false),
                    CanViewCandidates = table.Column<bool>(type: "bit", nullable: false),
                    CanAddNotes = table.Column<bool>(type: "bit", nullable: false),
                    CanSubmitEvaluation = table.Column<bool>(type: "bit", nullable: false),
                    CanViewOtherEvaluations = table.Column<bool>(type: "bit", nullable: false),
                    CanViewCommitteeSummary = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RemovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RemovalReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewCommitteeMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewCommitteeMember_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewCommitteeMember_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewCommitteeMember_AspNetUsers_MemberUserId",
                        column: x => x.MemberUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewCommitteeMember_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewCommitteeMember_InterviewCommittee_InterviewCommitteeId",
                        column: x => x.InterviewCommitteeId,
                        principalSchema: "itv",
                        principalTable: "InterviewCommittee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterviewAppointment",
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
                    InterviewScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvitationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InterviewCommitteeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterviewType = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RemoteMeetingUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RemoteMeetingInstructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AttendanceStatus = table.Column<int>(type: "int", nullable: true),
                    ActualStartAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualEndAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RescheduledFromAppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RescheduleReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvitationSentAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewAppointment", x => x.Id);
                    table.CheckConstraint("CK_Appointment_Time", "[EndAt] > [StartAt]");
                    table.CheckConstraint("CK_Appointment_Venue", "([InterviewType] = 1 AND [RoomId] IS NOT NULL) OR ([InterviewType] = 2 AND [RemoteMeetingUrl] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_InterviewAppointment_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewAppointment_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewAppointment_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewAppointment_InterviewAppointment_RescheduledFromAppointmentId",
                        column: x => x.RescheduledFromAppointmentId,
                        principalSchema: "itv",
                        principalTable: "InterviewAppointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewAppointment_InterviewCommittee_InterviewCommitteeId",
                        column: x => x.InterviewCommitteeId,
                        principalSchema: "itv",
                        principalTable: "InterviewCommittee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewAppointment_InterviewSchedule_InterviewScheduleId",
                        column: x => x.InterviewScheduleId,
                        principalSchema: "itv",
                        principalTable: "InterviewSchedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewAppointment_Invitation_InvitationId",
                        column: x => x.InvitationId,
                        principalSchema: "hr",
                        principalTable: "Invitation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterviewResultReport",
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
                    InterviewScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppliedQualificationScore = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApprovedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DecisionNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewResultReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewResultReport_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewResultReport_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewResultReport_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewResultReport_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewResultReport_InterviewSchedule_InterviewScheduleId",
                        column: x => x.InterviewScheduleId,
                        principalSchema: "itv",
                        principalTable: "InterviewSchedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterviewCommitteeMemberEvaluationAxis",
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
                    InterviewCommitteeMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterviewTemplateEvaluationAxisId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewCommitteeMemberEvaluationAxis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewCommitteeMemberEvaluationAxis_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewCommitteeMemberEvaluationAxis_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewCommitteeMemberEvaluationAxis_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewCommitteeMemberEvaluationAxis_InterviewCommitteeMember_InterviewCommitteeMemberId",
                        column: x => x.InterviewCommitteeMemberId,
                        principalSchema: "itv",
                        principalTable: "InterviewCommitteeMember",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewCommitteeMemberEvaluationAxis_InterviewTemplateEvaluationAxis_InterviewTemplateEvaluationAxisId",
                        column: x => x.InterviewTemplateEvaluationAxisId,
                        principalSchema: "itv",
                        principalTable: "InterviewTemplateEvaluationAxis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterviewAppointmentNote",
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
                    InterviewAppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NoteTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorCommitteeMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVisibleToCommittee = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewAppointmentNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewAppointmentNote_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewAppointmentNote_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewAppointmentNote_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewAppointmentNote_InterviewAppointment_InterviewAppointmentId",
                        column: x => x.InterviewAppointmentId,
                        principalSchema: "itv",
                        principalTable: "InterviewAppointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewAppointmentNote_InterviewCommitteeMember_AuthorCommitteeMemberId",
                        column: x => x.AuthorCommitteeMemberId,
                        principalSchema: "itv",
                        principalTable: "InterviewCommitteeMember",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewAppointmentNote_InterviewNoteType_NoteTypeId",
                        column: x => x.NoteTypeId,
                        principalSchema: "lkp",
                        principalTable: "InterviewNoteType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterviewMemberEvaluation",
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
                    InterviewAppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterviewCommitteeMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalScore = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReopenedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReopenedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReopenReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GeneralNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewMemberEvaluation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewMemberEvaluation_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewMemberEvaluation_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewMemberEvaluation_AspNetUsers_ReopenedById",
                        column: x => x.ReopenedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InterviewMemberEvaluation_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewMemberEvaluation_InterviewAppointment_InterviewAppointmentId",
                        column: x => x.InterviewAppointmentId,
                        principalSchema: "itv",
                        principalTable: "InterviewAppointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewMemberEvaluation_InterviewCommitteeMember_InterviewCommitteeMemberId",
                        column: x => x.InterviewCommitteeMemberId,
                        principalSchema: "itv",
                        principalTable: "InterviewCommitteeMember",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterviewOperationalIssue",
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
                    InterviewAppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IssueType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsBlocking = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ResolvedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolutionNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewOperationalIssue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewOperationalIssue_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewOperationalIssue_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewOperationalIssue_AspNetUsers_ResolvedById",
                        column: x => x.ResolvedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InterviewOperationalIssue_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewOperationalIssue_InterviewAppointment_InterviewAppointmentId",
                        column: x => x.InterviewAppointmentId,
                        principalSchema: "itv",
                        principalTable: "InterviewAppointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterviewResultCandidate",
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
                    InterviewResultReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterviewAppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FinalScore = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    QualificationScore = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    IsQualified = table.Column<bool>(type: "bit", nullable: false),
                    CalculationMethod = table.Column<int>(type: "int", nullable: false),
                    FinalDecision = table.Column<int>(type: "int", nullable: true),
                    DecisionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DecidedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DecidedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SnapshotAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewResultCandidate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewResultCandidate_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewResultCandidate_AspNetUsers_DecidedById",
                        column: x => x.DecidedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewResultCandidate_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewResultCandidate_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewResultCandidate_InterviewAppointment_InterviewAppointmentId",
                        column: x => x.InterviewAppointmentId,
                        principalSchema: "itv",
                        principalTable: "InterviewAppointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewResultCandidate_InterviewResultReport_InterviewResultReportId",
                        column: x => x.InterviewResultReportId,
                        principalSchema: "itv",
                        principalTable: "InterviewResultReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterviewMemberEvaluationCriterion",
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
                    InterviewMemberEvaluationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterviewTemplateEvaluationCriterionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewMemberEvaluationCriterion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewMemberEvaluationCriterion_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewMemberEvaluationCriterion_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewMemberEvaluationCriterion_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewMemberEvaluationCriterion_InterviewMemberEvaluation_InterviewMemberEvaluationId",
                        column: x => x.InterviewMemberEvaluationId,
                        principalSchema: "itv",
                        principalTable: "InterviewMemberEvaluation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewMemberEvaluationCriterion_InterviewTemplateEvaluationCriterion_InterviewTemplateEvaluationCriterionId",
                        column: x => x.InterviewTemplateEvaluationCriterionId,
                        principalSchema: "itv",
                        principalTable: "InterviewTemplateEvaluationCriterion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterviewResultCandidateAxis",
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
                    InterviewResultCandidateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterviewTemplateEvaluationAxisId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    QualificationScore = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    QualificationMet = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewResultCandidateAxis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewResultCandidateAxis_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewResultCandidateAxis_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewResultCandidateAxis_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewResultCandidateAxis_InterviewResultCandidate_InterviewResultCandidateId",
                        column: x => x.InterviewResultCandidateId,
                        principalSchema: "itv",
                        principalTable: "InterviewResultCandidate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewResultCandidateAxis_InterviewTemplateEvaluationAxis_InterviewTemplateEvaluationAxisId",
                        column: x => x.InterviewTemplateEvaluationAxisId,
                        principalSchema: "itv",
                        principalTable: "InterviewTemplateEvaluationAxis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[] { -366377821, "permission", "question-banks.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

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

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "InterviewNoteType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("8e6f2a12-6a1e-4b8a-9f0a-1a2b3c4d5e01"), "ChairmanNote", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "ملاحظة رئيس اللجنة", "Chairman Note", null, null },
                    { new Guid("8e6f2a12-6a1e-4b8a-9f0a-1a2b3c4d5e02"), "HRNote", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "ملاحظة الموارد البشرية", "HR Note", null, null },
                    { new Guid("8e6f2a12-6a1e-4b8a-9f0a-1a2b3c4d5e03"), "OperationalNote", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "ملاحظة تشغيلية", "Operational Note", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "InterviewOrganizationScope",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("8e6f2a11-6a1e-4b8a-9f0a-1a2b3c4d5e01"), "Ministry", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "الوزارة", "Ministry", null, null },
                    { new Guid("8e6f2a11-6a1e-4b8a-9f0a-1a2b3c4d5e02"), "Schools", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "المدارس", "Schools", null, null }
                });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("0342b09b-1b90-335f-ba63-c0c9a7bf0f3b"),
                column: "NameAr",
                value: "إعدادات انتهاء صلاحية الدعوة - عرض");

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("30b1b4f9-70da-af5d-888b-f434086b5af0"),
                column: "NameAr",
                value: "إعدادات انتهاء صلاحية الدعوة - إدارة");

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Permission",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsAssignableToRole", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[] { new Guid("298210b0-f475-a85c-9ec2-1b689ad00614"), "question-banks.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 84, true, true, false, "بنوك الأسئلة - عرض", "Question Banks - View", null, null });

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptQuestion_QuestionId",
                schema: "hr",
                table: "TestAttemptQuestion",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptQuestion_SelectedQuestionRevisionOptionId",
                schema: "hr",
                table: "TestAttemptQuestion",
                column: "SelectedQuestionRevisionOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamCategory_QuestionBankVersionId",
                schema: "hr",
                table: "ExamCategory",
                column: "QuestionBankVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_Conflict",
                schema: "itv",
                table: "InterviewAppointment",
                columns: new[] { "InterviewCommitteeId", "StartAt", "EndAt" },
                filter: "[Status] IN (1, 2, 3, 4, 5)")
                .Annotation("SqlServer:Include", new[] { "RoomId", "InvitationId" });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAppointment_CreatedById",
                schema: "itv",
                table: "InterviewAppointment",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAppointment_CreatedDate",
                schema: "itv",
                table: "InterviewAppointment",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAppointment_DeletedById",
                schema: "itv",
                table: "InterviewAppointment",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAppointment_InterviewScheduleId",
                schema: "itv",
                table: "InterviewAppointment",
                column: "InterviewScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAppointment_InvitationId",
                schema: "itv",
                table: "InterviewAppointment",
                column: "InvitationId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAppointment_IsDeleted",
                schema: "itv",
                table: "InterviewAppointment",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAppointment_RescheduledFromAppointmentId",
                schema: "itv",
                table: "InterviewAppointment",
                column: "RescheduledFromAppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAppointment_RoomId",
                schema: "itv",
                table: "InterviewAppointment",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAppointment_Status",
                schema: "itv",
                table: "InterviewAppointment",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAppointment_UpdatedById",
                schema: "itv",
                table: "InterviewAppointment",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAppointmentNote_AuthorCommitteeMemberId",
                schema: "itv",
                table: "InterviewAppointmentNote",
                column: "AuthorCommitteeMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAppointmentNote_CreatedById",
                schema: "itv",
                table: "InterviewAppointmentNote",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAppointmentNote_CreatedDate",
                schema: "itv",
                table: "InterviewAppointmentNote",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAppointmentNote_DeletedById",
                schema: "itv",
                table: "InterviewAppointmentNote",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAppointmentNote_InterviewAppointmentId",
                schema: "itv",
                table: "InterviewAppointmentNote",
                column: "InterviewAppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAppointmentNote_IsDeleted",
                schema: "itv",
                table: "InterviewAppointmentNote",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAppointmentNote_NoteTypeId",
                schema: "itv",
                table: "InterviewAppointmentNote",
                column: "NoteTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAppointmentNote_UpdatedById",
                schema: "itv",
                table: "InterviewAppointmentNote",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewApprovalAction_CreatedById",
                schema: "itv",
                table: "InterviewApprovalAction",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewApprovalAction_CreatedDate",
                schema: "itv",
                table: "InterviewApprovalAction",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewApprovalAction_DeletedById",
                schema: "itv",
                table: "InterviewApprovalAction",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewApprovalAction_EntityId",
                schema: "itv",
                table: "InterviewApprovalAction",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewApprovalAction_EntityType_EntityId",
                schema: "itv",
                table: "InterviewApprovalAction",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewApprovalAction_IsDeleted",
                schema: "itv",
                table: "InterviewApprovalAction",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewApprovalAction_UpdatedById",
                schema: "itv",
                table: "InterviewApprovalAction",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAuditLog_CreatedById",
                schema: "itv",
                table: "InterviewAuditLog",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAuditLog_CreatedDate",
                schema: "itv",
                table: "InterviewAuditLog",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAuditLog_DeletedById",
                schema: "itv",
                table: "InterviewAuditLog",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAuditLog_EntityId",
                schema: "itv",
                table: "InterviewAuditLog",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAuditLog_EntityType_EntityId",
                schema: "itv",
                table: "InterviewAuditLog",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAuditLog_IsDeleted",
                schema: "itv",
                table: "InterviewAuditLog",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAuditLog_UpdatedById",
                schema: "itv",
                table: "InterviewAuditLog",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommittee_ApprovedById",
                schema: "itv",
                table: "InterviewCommittee",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommittee_CommitteeTypeId",
                schema: "itv",
                table: "InterviewCommittee",
                column: "CommitteeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommittee_CreatedById",
                schema: "itv",
                table: "InterviewCommittee",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommittee_CreatedDate",
                schema: "itv",
                table: "InterviewCommittee",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommittee_DeletedById",
                schema: "itv",
                table: "InterviewCommittee",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommittee_IsDeleted",
                schema: "itv",
                table: "InterviewCommittee",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommittee_JobInterviewTemplateId",
                schema: "itv",
                table: "InterviewCommittee",
                column: "JobInterviewTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommittee_Status",
                schema: "itv",
                table: "InterviewCommittee",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommittee_UpdatedById",
                schema: "itv",
                table: "InterviewCommittee",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "UX_InterviewCommittee_ActiveJob",
                schema: "itv",
                table: "InterviewCommittee",
                column: "JobId",
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommitteeMember_CreatedById",
                schema: "itv",
                table: "InterviewCommitteeMember",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommitteeMember_CreatedDate",
                schema: "itv",
                table: "InterviewCommitteeMember",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommitteeMember_DeletedById",
                schema: "itv",
                table: "InterviewCommitteeMember",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommitteeMember_IsDeleted",
                schema: "itv",
                table: "InterviewCommitteeMember",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommitteeMember_MemberUserId",
                schema: "itv",
                table: "InterviewCommitteeMember",
                column: "MemberUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommitteeMember_UpdatedById",
                schema: "itv",
                table: "InterviewCommitteeMember",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "UX_Committee_Chair",
                schema: "itv",
                table: "InterviewCommitteeMember",
                column: "InterviewCommitteeId",
                unique: true,
                filter: "[Role] = 1 AND [IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "UX_Committee_Member",
                schema: "itv",
                table: "InterviewCommitteeMember",
                columns: new[] { "InterviewCommitteeId", "MemberUserId" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommitteeMemberEvaluationAxis_CreatedById",
                schema: "itv",
                table: "InterviewCommitteeMemberEvaluationAxis",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommitteeMemberEvaluationAxis_CreatedDate",
                schema: "itv",
                table: "InterviewCommitteeMemberEvaluationAxis",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommitteeMemberEvaluationAxis_DeletedById",
                schema: "itv",
                table: "InterviewCommitteeMemberEvaluationAxis",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommitteeMemberEvaluationAxis_InterviewCommitteeMemberId_InterviewTemplateEvaluationAxisId",
                schema: "itv",
                table: "InterviewCommitteeMemberEvaluationAxis",
                columns: new[] { "InterviewCommitteeMemberId", "InterviewTemplateEvaluationAxisId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommitteeMemberEvaluationAxis_InterviewTemplateEvaluationAxisId",
                schema: "itv",
                table: "InterviewCommitteeMemberEvaluationAxis",
                column: "InterviewTemplateEvaluationAxisId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommitteeMemberEvaluationAxis_IsDeleted",
                schema: "itv",
                table: "InterviewCommitteeMemberEvaluationAxis",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewCommitteeMemberEvaluationAxis_UpdatedById",
                schema: "itv",
                table: "InterviewCommitteeMemberEvaluationAxis",
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
                name: "IX_InterviewEvaluationAxis_CreatedById",
                schema: "itv",
                table: "InterviewEvaluationAxis",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewEvaluationAxis_CreatedDate",
                schema: "itv",
                table: "InterviewEvaluationAxis",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewEvaluationAxis_DeletedById",
                schema: "itv",
                table: "InterviewEvaluationAxis",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewEvaluationAxis_IsDeleted",
                schema: "itv",
                table: "InterviewEvaluationAxis",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewEvaluationAxis_UpdatedById",
                schema: "itv",
                table: "InterviewEvaluationAxis",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewEvaluationCriterion_CreatedById",
                schema: "itv",
                table: "InterviewEvaluationCriterion",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewEvaluationCriterion_CreatedDate",
                schema: "itv",
                table: "InterviewEvaluationCriterion",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewEvaluationCriterion_DeletedById",
                schema: "itv",
                table: "InterviewEvaluationCriterion",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewEvaluationCriterion_InterviewEvaluationAxisId",
                schema: "itv",
                table: "InterviewEvaluationCriterion",
                column: "InterviewEvaluationAxisId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewEvaluationCriterion_IsDeleted",
                schema: "itv",
                table: "InterviewEvaluationCriterion",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewEvaluationCriterion_UpdatedById",
                schema: "itv",
                table: "InterviewEvaluationCriterion",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewMemberEvaluation_CreatedById",
                schema: "itv",
                table: "InterviewMemberEvaluation",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewMemberEvaluation_CreatedDate",
                schema: "itv",
                table: "InterviewMemberEvaluation",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewMemberEvaluation_DeletedById",
                schema: "itv",
                table: "InterviewMemberEvaluation",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewMemberEvaluation_InterviewCommitteeMemberId",
                schema: "itv",
                table: "InterviewMemberEvaluation",
                column: "InterviewCommitteeMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewMemberEvaluation_IsDeleted",
                schema: "itv",
                table: "InterviewMemberEvaluation",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewMemberEvaluation_ReopenedById",
                schema: "itv",
                table: "InterviewMemberEvaluation",
                column: "ReopenedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewMemberEvaluation_Status",
                schema: "itv",
                table: "InterviewMemberEvaluation",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewMemberEvaluation_UpdatedById",
                schema: "itv",
                table: "InterviewMemberEvaluation",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "UQ_MemberEvaluation",
                schema: "itv",
                table: "InterviewMemberEvaluation",
                columns: new[] { "InterviewAppointmentId", "InterviewCommitteeMemberId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterviewMemberEvaluationCriterion_CreatedById",
                schema: "itv",
                table: "InterviewMemberEvaluationCriterion",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewMemberEvaluationCriterion_CreatedDate",
                schema: "itv",
                table: "InterviewMemberEvaluationCriterion",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewMemberEvaluationCriterion_DeletedById",
                schema: "itv",
                table: "InterviewMemberEvaluationCriterion",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewMemberEvaluationCriterion_InterviewMemberEvaluationId_InterviewTemplateEvaluationCriterionId",
                schema: "itv",
                table: "InterviewMemberEvaluationCriterion",
                columns: new[] { "InterviewMemberEvaluationId", "InterviewTemplateEvaluationCriterionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterviewMemberEvaluationCriterion_InterviewTemplateEvaluationCriterionId",
                schema: "itv",
                table: "InterviewMemberEvaluationCriterion",
                column: "InterviewTemplateEvaluationCriterionId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewMemberEvaluationCriterion_IsDeleted",
                schema: "itv",
                table: "InterviewMemberEvaluationCriterion",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewMemberEvaluationCriterion_UpdatedById",
                schema: "itv",
                table: "InterviewMemberEvaluationCriterion",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewNoteType_BackendName",
                schema: "lkp",
                table: "InterviewNoteType",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterviewNoteType_CreatedById",
                schema: "lkp",
                table: "InterviewNoteType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewNoteType_CreatedDate",
                schema: "lkp",
                table: "InterviewNoteType",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewNoteType_DeletedById",
                schema: "lkp",
                table: "InterviewNoteType",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewNoteType_DisplayOrder",
                schema: "lkp",
                table: "InterviewNoteType",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewNoteType_IsDeleted",
                schema: "lkp",
                table: "InterviewNoteType",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewNoteType_UpdatedById",
                schema: "lkp",
                table: "InterviewNoteType",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewOperationalIssue_CreatedById",
                schema: "itv",
                table: "InterviewOperationalIssue",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewOperationalIssue_CreatedDate",
                schema: "itv",
                table: "InterviewOperationalIssue",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewOperationalIssue_DeletedById",
                schema: "itv",
                table: "InterviewOperationalIssue",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewOperationalIssue_InterviewAppointmentId",
                schema: "itv",
                table: "InterviewOperationalIssue",
                column: "InterviewAppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewOperationalIssue_IsDeleted",
                schema: "itv",
                table: "InterviewOperationalIssue",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewOperationalIssue_ResolvedById",
                schema: "itv",
                table: "InterviewOperationalIssue",
                column: "ResolvedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewOperationalIssue_Status",
                schema: "itv",
                table: "InterviewOperationalIssue",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewOperationalIssue_UpdatedById",
                schema: "itv",
                table: "InterviewOperationalIssue",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewOrganizationScope_BackendName",
                schema: "lkp",
                table: "InterviewOrganizationScope",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterviewOrganizationScope_CreatedById",
                schema: "lkp",
                table: "InterviewOrganizationScope",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewOrganizationScope_CreatedDate",
                schema: "lkp",
                table: "InterviewOrganizationScope",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewOrganizationScope_DeletedById",
                schema: "lkp",
                table: "InterviewOrganizationScope",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewOrganizationScope_DisplayOrder",
                schema: "lkp",
                table: "InterviewOrganizationScope",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewOrganizationScope_IsDeleted",
                schema: "lkp",
                table: "InterviewOrganizationScope",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewOrganizationScope_UpdatedById",
                schema: "lkp",
                table: "InterviewOrganizationScope",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewResultCandidate_CreatedById",
                schema: "itv",
                table: "InterviewResultCandidate",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewResultCandidate_CreatedDate",
                schema: "itv",
                table: "InterviewResultCandidate",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewResultCandidate_DecidedById",
                schema: "itv",
                table: "InterviewResultCandidate",
                column: "DecidedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewResultCandidate_DeletedById",
                schema: "itv",
                table: "InterviewResultCandidate",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewResultCandidate_InterviewResultReportId",
                schema: "itv",
                table: "InterviewResultCandidate",
                column: "InterviewResultReportId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewResultCandidate_IsDeleted",
                schema: "itv",
                table: "InterviewResultCandidate",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewResultCandidate_UpdatedById",
                schema: "itv",
                table: "InterviewResultCandidate",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "UQ_ResultCandidate",
                schema: "itv",
                table: "InterviewResultCandidate",
                column: "InterviewAppointmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterviewResultCandidateAxis_CreatedById",
                schema: "itv",
                table: "InterviewResultCandidateAxis",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewResultCandidateAxis_CreatedDate",
                schema: "itv",
                table: "InterviewResultCandidateAxis",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewResultCandidateAxis_DeletedById",
                schema: "itv",
                table: "InterviewResultCandidateAxis",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewResultCandidateAxis_InterviewResultCandidateId_InterviewTemplateEvaluationAxisId",
                schema: "itv",
                table: "InterviewResultCandidateAxis",
                columns: new[] { "InterviewResultCandidateId", "InterviewTemplateEvaluationAxisId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterviewResultCandidateAxis_InterviewTemplateEvaluationAxisId",
                schema: "itv",
                table: "InterviewResultCandidateAxis",
                column: "InterviewTemplateEvaluationAxisId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewResultCandidateAxis_IsDeleted",
                schema: "itv",
                table: "InterviewResultCandidateAxis",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewResultCandidateAxis_UpdatedById",
                schema: "itv",
                table: "InterviewResultCandidateAxis",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewResultReport_ApprovedById",
                schema: "itv",
                table: "InterviewResultReport",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewResultReport_CreatedById",
                schema: "itv",
                table: "InterviewResultReport",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewResultReport_CreatedDate",
                schema: "itv",
                table: "InterviewResultReport",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewResultReport_DeletedById",
                schema: "itv",
                table: "InterviewResultReport",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewResultReport_IsDeleted",
                schema: "itv",
                table: "InterviewResultReport",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewResultReport_Status",
                schema: "itv",
                table: "InterviewResultReport",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewResultReport_UpdatedById",
                schema: "itv",
                table: "InterviewResultReport",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "UQ_ResultReport",
                schema: "itv",
                table: "InterviewResultReport",
                column: "InterviewScheduleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSchedule_ApprovedById",
                schema: "itv",
                table: "InterviewSchedule",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSchedule_CreatedById",
                schema: "itv",
                table: "InterviewSchedule",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSchedule_CreatedDate",
                schema: "itv",
                table: "InterviewSchedule",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSchedule_DeletedById",
                schema: "itv",
                table: "InterviewSchedule",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSchedule_IsDeleted",
                schema: "itv",
                table: "InterviewSchedule",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSchedule_JobId",
                schema: "itv",
                table: "InterviewSchedule",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSchedule_JobInterviewTemplateId",
                schema: "itv",
                table: "InterviewSchedule",
                column: "JobInterviewTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSchedule_Status",
                schema: "itv",
                table: "InterviewSchedule",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSchedule_UpdatedById",
                schema: "itv",
                table: "InterviewSchedule",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplate_CreatedById",
                schema: "itv",
                table: "InterviewTemplate",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplate_CreatedDate",
                schema: "itv",
                table: "InterviewTemplate",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplate_DeletedById",
                schema: "itv",
                table: "InterviewTemplate",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplate_DepartmentId",
                schema: "itv",
                table: "InterviewTemplate",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplate_IsDeleted",
                schema: "itv",
                table: "InterviewTemplate",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplate_JobTitleId",
                schema: "itv",
                table: "InterviewTemplate",
                column: "JobTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplate_OrganizationScopeId",
                schema: "itv",
                table: "InterviewTemplate",
                column: "OrganizationScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplate_UpdatedById",
                schema: "itv",
                table: "InterviewTemplate",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplateEvaluationAxis_CreatedById",
                schema: "itv",
                table: "InterviewTemplateEvaluationAxis",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplateEvaluationAxis_CreatedDate",
                schema: "itv",
                table: "InterviewTemplateEvaluationAxis",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplateEvaluationAxis_DeletedById",
                schema: "itv",
                table: "InterviewTemplateEvaluationAxis",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplateEvaluationAxis_InterviewEvaluationAxisId",
                schema: "itv",
                table: "InterviewTemplateEvaluationAxis",
                column: "InterviewEvaluationAxisId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplateEvaluationAxis_InterviewTemplateVersionId_InterviewEvaluationAxisId",
                schema: "itv",
                table: "InterviewTemplateEvaluationAxis",
                columns: new[] { "InterviewTemplateVersionId", "InterviewEvaluationAxisId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplateEvaluationAxis_IsDeleted",
                schema: "itv",
                table: "InterviewTemplateEvaluationAxis",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplateEvaluationAxis_UpdatedById",
                schema: "itv",
                table: "InterviewTemplateEvaluationAxis",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplateEvaluationCriterion_CreatedById",
                schema: "itv",
                table: "InterviewTemplateEvaluationCriterion",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplateEvaluationCriterion_CreatedDate",
                schema: "itv",
                table: "InterviewTemplateEvaluationCriterion",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplateEvaluationCriterion_DeletedById",
                schema: "itv",
                table: "InterviewTemplateEvaluationCriterion",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplateEvaluationCriterion_InterviewEvaluationCriterionId",
                schema: "itv",
                table: "InterviewTemplateEvaluationCriterion",
                column: "InterviewEvaluationCriterionId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplateEvaluationCriterion_InterviewTemplateEvaluationAxisId",
                schema: "itv",
                table: "InterviewTemplateEvaluationCriterion",
                column: "InterviewTemplateEvaluationAxisId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplateEvaluationCriterion_IsDeleted",
                schema: "itv",
                table: "InterviewTemplateEvaluationCriterion",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplateEvaluationCriterion_UpdatedById",
                schema: "itv",
                table: "InterviewTemplateEvaluationCriterion",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplateVersion_ApprovedById",
                schema: "itv",
                table: "InterviewTemplateVersion",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplateVersion_CreatedById",
                schema: "itv",
                table: "InterviewTemplateVersion",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplateVersion_CreatedDate",
                schema: "itv",
                table: "InterviewTemplateVersion",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplateVersion_DeletedById",
                schema: "itv",
                table: "InterviewTemplateVersion",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplateVersion_InterviewTemplateId_VersionNo",
                schema: "itv",
                table: "InterviewTemplateVersion",
                columns: new[] { "InterviewTemplateId", "VersionNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplateVersion_IsDeleted",
                schema: "itv",
                table: "InterviewTemplateVersion",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplateVersion_Status",
                schema: "itv",
                table: "InterviewTemplateVersion",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewTemplateVersion_UpdatedById",
                schema: "itv",
                table: "InterviewTemplateVersion",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "UX_TemplateVersion_Approved",
                schema: "itv",
                table: "InterviewTemplateVersion",
                column: "InterviewTemplateId",
                unique: true,
                filter: "[Status] = 4 AND [IsDeleted] = 0");

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
                name: "FK_ExamCategory_QuestionBankVersion_QuestionBankVersionId",
                schema: "hr",
                table: "ExamCategory",
                column: "QuestionBankVersionId",
                principalSchema: "hr",
                principalTable: "QuestionBankVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestAttemptQuestion_QuestionRevisionOption_SelectedQuestionRevisionOptionId",
                schema: "hr",
                table: "TestAttemptQuestion",
                column: "SelectedQuestionRevisionOptionId",
                principalSchema: "hr",
                principalTable: "QuestionRevisionOption",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestAttemptQuestion_Question_QuestionId",
                schema: "hr",
                table: "TestAttemptQuestion",
                column: "QuestionId",
                principalSchema: "hr",
                principalTable: "Question",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamCategory_QuestionBankVersion_QuestionBankVersionId",
                schema: "hr",
                table: "ExamCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_TestAttemptQuestion_QuestionRevisionOption_SelectedQuestionRevisionOptionId",
                schema: "hr",
                table: "TestAttemptQuestion");

            migrationBuilder.DropForeignKey(
                name: "FK_TestAttemptQuestion_Question_QuestionId",
                schema: "hr",
                table: "TestAttemptQuestion");

            migrationBuilder.DropTable(
                name: "InterviewAppointmentNote",
                schema: "itv");

            migrationBuilder.DropTable(
                name: "InterviewApprovalAction",
                schema: "itv");

            migrationBuilder.DropTable(
                name: "InterviewAuditLog",
                schema: "itv");

            migrationBuilder.DropTable(
                name: "InterviewCommitteeMemberEvaluationAxis",
                schema: "itv");

            migrationBuilder.DropTable(
                name: "InterviewMemberEvaluationCriterion",
                schema: "itv");

            migrationBuilder.DropTable(
                name: "InterviewOperationalIssue",
                schema: "itv");

            migrationBuilder.DropTable(
                name: "InterviewResultCandidateAxis",
                schema: "itv");

            migrationBuilder.DropTable(
                name: "InterviewNoteType",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "InterviewMemberEvaluation",
                schema: "itv");

            migrationBuilder.DropTable(
                name: "InterviewTemplateEvaluationCriterion",
                schema: "itv");

            migrationBuilder.DropTable(
                name: "InterviewResultCandidate",
                schema: "itv");

            migrationBuilder.DropTable(
                name: "InterviewCommitteeMember",
                schema: "itv");

            migrationBuilder.DropTable(
                name: "InterviewEvaluationCriterion",
                schema: "itv");

            migrationBuilder.DropTable(
                name: "InterviewTemplateEvaluationAxis",
                schema: "itv");

            migrationBuilder.DropTable(
                name: "InterviewAppointment",
                schema: "itv");

            migrationBuilder.DropTable(
                name: "InterviewResultReport",
                schema: "itv");

            migrationBuilder.DropTable(
                name: "InterviewEvaluationAxis",
                schema: "itv");

            migrationBuilder.DropTable(
                name: "InterviewCommittee",
                schema: "itv");

            migrationBuilder.DropTable(
                name: "InterviewSchedule",
                schema: "itv");

            migrationBuilder.DropTable(
                name: "InterviewCommitteeType",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "JobInterviewTemplate",
                schema: "itv");

            migrationBuilder.DropTable(
                name: "InterviewTemplateVersion",
                schema: "itv");

            migrationBuilder.DropTable(
                name: "InterviewTemplate",
                schema: "itv");

            migrationBuilder.DropTable(
                name: "InterviewOrganizationScope",
                schema: "lkp");

            migrationBuilder.DropIndex(
                name: "IX_TestAttemptQuestion_QuestionId",
                schema: "hr",
                table: "TestAttemptQuestion");

            migrationBuilder.DropIndex(
                name: "IX_TestAttemptQuestion_SelectedQuestionRevisionOptionId",
                schema: "hr",
                table: "TestAttemptQuestion");

            migrationBuilder.DropIndex(
                name: "IX_ExamCategory_QuestionBankVersionId",
                schema: "hr",
                table: "ExamCategory");

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -366377821);

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("298210b0-f475-a85c-9ec2-1b689ad00614"));

            migrationBuilder.DropColumn(
                name: "QuestionId",
                schema: "hr",
                table: "TestAttemptQuestion");

            migrationBuilder.DropColumn(
                name: "SelectedQuestionRevisionOptionId",
                schema: "hr",
                table: "TestAttemptQuestion");

            migrationBuilder.DropColumn(
                name: "QuestionBankVersionId",
                schema: "hr",
                table: "ExamCategory");

            migrationBuilder.DropColumn(
                name: "ExamNo",
                schema: "hr",
                table: "Exam");

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("0342b09b-1b90-335f-ba63-c0c9a7bf0f3b"),
                column: "NameAr",
                value: "Invitation Expiry Configuration - View");

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("30b1b4f9-70da-af5d-888b-f434086b5af0"),
                column: "NameAr",
                value: "Invitation Expiry Configuration - Manage");
        }
    }
}
