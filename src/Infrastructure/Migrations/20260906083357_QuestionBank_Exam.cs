using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class QuestionBank_Exam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DifficultyLevel",
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
                    table.PrimaryKey("PK_DifficultyLevel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DifficultyLevel_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DifficultyLevel_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DifficultyLevel_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "ExamExemptionDecisionStatus",
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
                    table.PrimaryKey("PK_ExamExemptionDecisionStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamExemptionDecisionStatus_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamExemptionDecisionStatus_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamExemptionDecisionStatus_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamInterruptionPolicy",
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
                    table.PrimaryKey("PK_ExamInterruptionPolicy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamInterruptionPolicy_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamInterruptionPolicy_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamInterruptionPolicy_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamResultCandidateResultStatus",
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
                    table.PrimaryKey("PK_ExamResultCandidateResultStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamResultCandidateResultStatus_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamResultCandidateResultStatus_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamResultCandidateResultStatus_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamResultReportStatus",
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
                    table.PrimaryKey("PK_ExamResultReportStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamResultReportStatus_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamResultReportStatus_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamResultReportStatus_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamStatus",
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
                    table.PrimaryKey("PK_ExamStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamStatus_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamStatus_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamStatus_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                schema: "hr",
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
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationLink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Location_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Location_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Location_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Question",
                schema: "hr",
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
                    QuestionCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Question_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Question_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Question_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionBankAssignmentStatus",
                schema: "lkp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionBankAssignmentStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionBankRequestItemStatus",
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
                    table.PrimaryKey("PK_QuestionBankRequestItemStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestItemStatus_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestItemStatus_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestItemStatus_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionBankRequestStatus",
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
                    table.PrimaryKey("PK_QuestionBankRequestStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestStatus_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestStatus_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestStatus_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionBankRequestType",
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
                    table.PrimaryKey("PK_QuestionBankRequestType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestType_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestType_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionBankType",
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
                    table.PrimaryKey("PK_QuestionBankType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionBankType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankType_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankType_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionChangeType",
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
                    table.PrimaryKey("PK_QuestionChangeType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionChangeType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionChangeType_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionChangeType_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionReviewDecision",
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
                    table.PrimaryKey("PK_QuestionReviewDecision", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionReviewDecision_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionReviewDecision_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionReviewDecision_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionType",
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
                    table.PrimaryKey("PK_QuestionType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionType_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionType_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoomStatus",
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
                    table.PrimaryKey("PK_RoomStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomStatus_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomStatus_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomStatus_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoomType",
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
                    table.PrimaryKey("PK_RoomType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomType_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomType_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Stage",
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
                    table.PrimaryKey("PK_Stage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stage_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stage_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stage_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestAttemptInterruptionResolutionAction",
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
                    table.PrimaryKey("PK_TestAttemptInterruptionResolutionAction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestAttemptInterruptionResolutionAction_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttemptInterruptionResolutionAction_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttemptInterruptionResolutionAction_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestAttemptInterruptionStatus",
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
                    table.PrimaryKey("PK_TestAttemptInterruptionStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestAttemptInterruptionStatus_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttemptInterruptionStatus_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttemptInterruptionStatus_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestAttemptPartStatus",
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
                    table.PrimaryKey("PK_TestAttemptPartStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestAttemptPartStatus_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttemptPartStatus_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttemptPartStatus_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestAttemptStatus",
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
                    table.PrimaryKey("PK_TestAttemptStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestAttemptStatus_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttemptStatus_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttemptStatus_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestSessionCandidateAttendanceStatus",
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
                    table.PrimaryKey("PK_TestSessionCandidateAttendanceStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestSessionCandidateAttendanceStatus_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSessionCandidateAttendanceStatus_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSessionCandidateAttendanceStatus_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestSessionCandidateIdentityVerificationStatus",
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
                    table.PrimaryKey("PK_TestSessionCandidateIdentityVerificationStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestSessionCandidateIdentityVerificationStatus_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSessionCandidateIdentityVerificationStatus_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSessionCandidateIdentityVerificationStatus_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestSessionCandidateStatus",
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
                    table.PrimaryKey("PK_TestSessionCandidateStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestSessionCandidateStatus_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSessionCandidateStatus_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSessionCandidateStatus_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestSessionStatus",
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
                    table.PrimaryKey("PK_TestSessionStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestSessionStatus_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSessionStatus_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSessionStatus_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestSlotStaffRole",
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
                    table.PrimaryKey("PK_TestSlotStaffRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestSlotStaffRole_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSlotStaffRole_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSlotStaffRole_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestSlotStatus",
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
                    table.PrimaryKey("PK_TestSlotStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestSlotStatus_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSlotStatus_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSlotStatus_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamExemptionDecision",
                schema: "hr",
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
                    InvitationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamExemptionDecision", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamExemptionDecision_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamExemptionDecision_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamExemptionDecision_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamExemptionDecision_ExamExemptionDecisionStatus_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "lkp",
                        principalTable: "ExamExemptionDecisionStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamExemptionDecision_Invitation_InvitationId",
                        column: x => x.InvitationId,
                        principalSchema: "hr",
                        principalTable: "Invitation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Exam",
                schema: "hr",
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
                    TitleAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalQuestions = table.Column<int>(type: "int", nullable: false),
                    AllowPreviousQuestion = table.Column<bool>(type: "bit", nullable: false),
                    InterruptionPolicyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DecisionNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exam", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exam_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Exam_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Exam_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Exam_ExamInterruptionPolicy_InterruptionPolicyId",
                        column: x => x.InterruptionPolicyId,
                        principalSchema: "lkp",
                        principalTable: "ExamInterruptionPolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Exam_ExamStatus_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "lkp",
                        principalTable: "ExamStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Exam_Job_JobId",
                        column: x => x.JobId,
                        principalSchema: "hr",
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Room",
                schema: "hr",
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
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Room_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Room_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Room_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Room_Location_LocationId",
                        column: x => x.LocationId,
                        principalSchema: "hr",
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Room_RoomStatus_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "lkp",
                        principalTable: "RoomStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Room_RoomType_RoomTypeId",
                        column: x => x.RoomTypeId,
                        principalSchema: "lkp",
                        principalTable: "RoomType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamPart",
                schema: "hr",
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
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartNo = table.Column<int>(type: "int", nullable: false),
                    TitleAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    QualificationScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamPart", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamPart_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamPart_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamPart_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamPart_Exam_ExamId",
                        column: x => x.ExamId,
                        principalSchema: "hr",
                        principalTable: "Exam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamResultReport",
                schema: "hr",
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
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppliedQualificationScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DecisionNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamResultReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamResultReport_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamResultReport_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamResultReport_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamResultReport_ExamResultReportStatus_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "lkp",
                        principalTable: "ExamResultReportStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamResultReport_Exam_ExamId",
                        column: x => x.ExamId,
                        principalSchema: "hr",
                        principalTable: "Exam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestSlot",
                schema: "hr",
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
                    TitleAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SlotDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    AccessCodeHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestSlot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestSlot_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSlot_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSlot_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSlot_Room_RoomId",
                        column: x => x.RoomId,
                        principalSchema: "hr",
                        principalTable: "Room",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSlot_TestSlotStatus_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "lkp",
                        principalTable: "TestSlotStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamCategory",
                schema: "hr",
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
                    ExamPartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionCount = table.Column<int>(type: "int", nullable: false),
                    WeightPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EasyQuestionCount = table.Column<int>(type: "int", nullable: false),
                    MediumQuestionCount = table.Column<int>(type: "int", nullable: false),
                    HardQuestionCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamCategory_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamCategory_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamCategory_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamCategory_ExamCategoryType_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "lkp",
                        principalTable: "ExamCategoryType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamCategory_ExamPart_ExamPartId",
                        column: x => x.ExamPartId,
                        principalSchema: "hr",
                        principalTable: "ExamPart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestSession",
                schema: "hr",
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
                    TestSlotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionNo = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestSession_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSession_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSession_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSession_Exam_ExamId",
                        column: x => x.ExamId,
                        principalSchema: "hr",
                        principalTable: "Exam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSession_TestSessionStatus_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "lkp",
                        principalTable: "TestSessionStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSession_TestSlot_TestSlotId",
                        column: x => x.TestSlotId,
                        principalSchema: "hr",
                        principalTable: "TestSlot",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestSlotStaff",
                schema: "hr",
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
                    TestSlotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestSlotStaff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestSlotStaff_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSlotStaff_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSlotStaff_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSlotStaff_TestSlotStaffRole_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "lkp",
                        principalTable: "TestSlotStaffRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSlotStaff_TestSlot_TestSlotId",
                        column: x => x.TestSlotId,
                        principalSchema: "hr",
                        principalTable: "TestSlot",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestSessionCandidate",
                schema: "hr",
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
                    TestSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvitationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttendanceStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdentityVerificationStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdentityVerificationNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorizedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RescheduleReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestSessionCandidate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestSessionCandidate_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSessionCandidate_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSessionCandidate_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSessionCandidate_Invitation_InvitationId",
                        column: x => x.InvitationId,
                        principalSchema: "hr",
                        principalTable: "Invitation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSessionCandidate_TestSessionCandidateAttendanceStatus_AttendanceStatusId",
                        column: x => x.AttendanceStatusId,
                        principalSchema: "lkp",
                        principalTable: "TestSessionCandidateAttendanceStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSessionCandidate_TestSessionCandidateIdentityVerificationStatus_IdentityVerificationStatusId",
                        column: x => x.IdentityVerificationStatusId,
                        principalSchema: "lkp",
                        principalTable: "TestSessionCandidateIdentityVerificationStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSessionCandidate_TestSessionCandidateStatus_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "lkp",
                        principalTable: "TestSessionCandidateStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestSessionCandidate_TestSession_TestSessionId",
                        column: x => x.TestSessionId,
                        principalSchema: "hr",
                        principalTable: "TestSession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestAttempt",
                schema: "hr",
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
                    TestSessionCandidateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestAttempt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestAttempt_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttempt_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttempt_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttempt_TestAttemptStatus_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "lkp",
                        principalTable: "TestAttemptStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttempt_TestSessionCandidate_TestSessionCandidateId",
                        column: x => x.TestSessionCandidateId,
                        principalSchema: "hr",
                        principalTable: "TestSessionCandidate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamResultCandidate",
                schema: "hr",
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
                    ExamResultReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvitationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestAttemptId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SpecializedScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EducationalScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SkillsScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FinalScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Rank = table.Column<int>(type: "int", nullable: true),
                    IsQualified = table.Column<bool>(type: "bit", nullable: false),
                    ResultStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SnapshotAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamResultCandidate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamResultCandidate_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamResultCandidate_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamResultCandidate_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamResultCandidate_ExamResultCandidateResultStatus_ResultStatusId",
                        column: x => x.ResultStatusId,
                        principalSchema: "lkp",
                        principalTable: "ExamResultCandidateResultStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamResultCandidate_ExamResultReport_ExamResultReportId",
                        column: x => x.ExamResultReportId,
                        principalSchema: "hr",
                        principalTable: "ExamResultReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamResultCandidate_Invitation_InvitationId",
                        column: x => x.InvitationId,
                        principalSchema: "hr",
                        principalTable: "Invitation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamResultCandidate_TestAttempt_TestAttemptId",
                        column: x => x.TestAttemptId,
                        principalSchema: "hr",
                        principalTable: "TestAttempt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestAttemptPart",
                schema: "hr",
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
                    TestAttemptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamPartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastQuestionNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestAttemptPart", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestAttemptPart_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttemptPart_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttemptPart_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttemptPart_ExamPart_ExamPartId",
                        column: x => x.ExamPartId,
                        principalSchema: "hr",
                        principalTable: "ExamPart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttemptPart_TestAttemptPartStatus_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "lkp",
                        principalTable: "TestAttemptPartStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttemptPart_TestAttempt_TestAttemptId",
                        column: x => x.TestAttemptId,
                        principalSchema: "hr",
                        principalTable: "TestAttempt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestAttemptQuestion",
                schema: "hr",
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
                    TestAttemptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    SavedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestAttemptQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestAttemptQuestion_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttemptQuestion_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttemptQuestion_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttemptQuestion_ExamCategory_ExamCategoryId",
                        column: x => x.ExamCategoryId,
                        principalSchema: "hr",
                        principalTable: "ExamCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttemptQuestion_TestAttempt_TestAttemptId",
                        column: x => x.TestAttemptId,
                        principalSchema: "hr",
                        principalTable: "TestAttempt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestAttemptInterruption",
                schema: "hr",
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
                    TestAttemptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestAttemptPartId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InterruptedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResolutionActionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResolutionNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResolvedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestAttemptInterruption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestAttemptInterruption_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttemptInterruption_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttemptInterruption_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttemptInterruption_TestAttemptInterruptionResolutionAction_ResolutionActionId",
                        column: x => x.ResolutionActionId,
                        principalSchema: "lkp",
                        principalTable: "TestAttemptInterruptionResolutionAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttemptInterruption_TestAttemptInterruptionStatus_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "lkp",
                        principalTable: "TestAttemptInterruptionStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttemptInterruption_TestAttemptPart_TestAttemptPartId",
                        column: x => x.TestAttemptPartId,
                        principalSchema: "hr",
                        principalTable: "TestAttemptPart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestAttemptInterruption_TestAttempt_TestAttemptId",
                        column: x => x.TestAttemptId,
                        principalSchema: "hr",
                        principalTable: "TestAttempt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionBank",
                schema: "hr",
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
                    ManagementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    JobTitleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    QuestionBankTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CurrentApprovedVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionBank", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionBank_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBank_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBank_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBank_JobTitle_JobTitleId",
                        column: x => x.JobTitleId,
                        principalSchema: "lkp",
                        principalTable: "JobTitle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBank_Management_ManagementId",
                        column: x => x.ManagementId,
                        principalSchema: "lkp",
                        principalTable: "Management",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBank_QuestionBankType_QuestionBankTypeId",
                        column: x => x.QuestionBankTypeId,
                        principalSchema: "lkp",
                        principalTable: "QuestionBankType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBank_Stage_StageId",
                        column: x => x.StageId,
                        principalSchema: "lkp",
                        principalTable: "Stage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionBankAssignment",
                schema: "hr",
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
                    QuestionBankRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MinimumQuestionCount = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QuestionEntryStartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QuestionEntryCompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastReturnedForModificationAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModificationCompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionBankAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionBankAssignment_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankAssignment_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankAssignment_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankAssignment_EmployeeProfile_AssignedByUserId",
                        column: x => x.AssignedByUserId,
                        principalTable: "EmployeeProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankAssignment_EmployeeProfile_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EmployeeProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankAssignment_QuestionBankAssignmentStatus_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "lkp",
                        principalTable: "QuestionBankAssignmentStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionBankRequest",
                schema: "hr",
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
                    QuestionBankId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaseVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentReviewRound = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmittedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinalDecisionById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FinalDecisionAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinalDecisionNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionBankRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequest_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequest_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequest_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequest_EmployeeProfile_FinalDecisionById",
                        column: x => x.FinalDecisionById,
                        principalTable: "EmployeeProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequest_EmployeeProfile_SubmittedById",
                        column: x => x.SubmittedById,
                        principalTable: "EmployeeProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequest_QuestionBankRequestStatus_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "lkp",
                        principalTable: "QuestionBankRequestStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequest_QuestionBankRequestType_RequestTypeId",
                        column: x => x.RequestTypeId,
                        principalSchema: "lkp",
                        principalTable: "QuestionBankRequestType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequest_QuestionBank_QuestionBankId",
                        column: x => x.QuestionBankId,
                        principalSchema: "hr",
                        principalTable: "QuestionBank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionBankRequestHistory",
                schema: "hr",
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
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PerformedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerformedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionBankRequestHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestHistory_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestHistory_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestHistory_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestHistory_QuestionBankRequestStatus_FromStatusId",
                        column: x => x.FromStatusId,
                        principalSchema: "lkp",
                        principalTable: "QuestionBankRequestStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestHistory_QuestionBankRequestStatus_ToStatusId",
                        column: x => x.ToStatusId,
                        principalSchema: "lkp",
                        principalTable: "QuestionBankRequestStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestHistory_QuestionBankRequest_RequestId",
                        column: x => x.RequestId,
                        principalSchema: "hr",
                        principalTable: "QuestionBankRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionBankRequestReview",
                schema: "hr",
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
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewRound = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ReviewedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionBankRequestReview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestReview_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestReview_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestReview_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestReview_EmployeeProfile_ReviewedById",
                        column: x => x.ReviewedById,
                        principalTable: "EmployeeProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestReview_QuestionBankRequest_RequestId",
                        column: x => x.RequestId,
                        principalSchema: "hr",
                        principalTable: "QuestionBankRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionBankVersion",
                schema: "hr",
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
                    QuestionBankId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VersionNo = table.Column<int>(type: "int", nullable: false),
                    PreviousVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedFromRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionBankVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersion_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersion_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersion_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersion_QuestionBankRequest_CreatedFromRequestId",
                        column: x => x.CreatedFromRequestId,
                        principalSchema: "hr",
                        principalTable: "QuestionBankRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersion_QuestionBankVersion_PreviousVersionId",
                        column: x => x.PreviousVersionId,
                        principalSchema: "hr",
                        principalTable: "QuestionBankVersion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersion_QuestionBank_QuestionBankId",
                        column: x => x.QuestionBankId,
                        principalSchema: "hr",
                        principalTable: "QuestionBank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionBankRequestItem",
                schema: "hr",
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
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionBankAssignmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangeTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginalRevisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CurrentProposedRevisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RemovedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RemovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RemovalNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionBankRequestItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestItem_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestItem_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestItem_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestItem_EmployeeProfile_RemovedById",
                        column: x => x.RemovedById,
                        principalTable: "EmployeeProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestItem_QuestionBankAssignment_QuestionBankAssignmentId",
                        column: x => x.QuestionBankAssignmentId,
                        principalSchema: "hr",
                        principalTable: "QuestionBankAssignment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestItem_QuestionBankRequestItemStatus_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "lkp",
                        principalTable: "QuestionBankRequestItemStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestItem_QuestionBankRequest_RequestId",
                        column: x => x.RequestId,
                        principalSchema: "hr",
                        principalTable: "QuestionBankRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestItem_QuestionChangeType_ChangeTypeId",
                        column: x => x.ChangeTypeId,
                        principalSchema: "lkp",
                        principalTable: "QuestionChangeType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestItem_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalSchema: "hr",
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionRevision",
                schema: "hr",
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
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RevisionNo = table.Column<int>(type: "int", nullable: false),
                    QuestionTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DifficultyLevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionTextAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuestionTextEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExplanationAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExplanationEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceRequestItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionRevision", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionRevision_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionRevision_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionRevision_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionRevision_DifficultyLevel_DifficultyLevelId",
                        column: x => x.DifficultyLevelId,
                        principalSchema: "lkp",
                        principalTable: "DifficultyLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionRevision_QuestionBankRequestItem_SourceRequestItemId",
                        column: x => x.SourceRequestItemId,
                        principalSchema: "hr",
                        principalTable: "QuestionBankRequestItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionRevision_QuestionType_QuestionTypeId",
                        column: x => x.QuestionTypeId,
                        principalSchema: "lkp",
                        principalTable: "QuestionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionRevision_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalSchema: "hr",
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionBankRequestItemReview",
                schema: "hr",
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
                    RequestItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewRound = table.Column<int>(type: "int", nullable: false),
                    ReviewedRevisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DecisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionBankRequestItemReview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestItemReview_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestItemReview_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestItemReview_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestItemReview_EmployeeProfile_ReviewedById",
                        column: x => x.ReviewedById,
                        principalTable: "EmployeeProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestItemReview_QuestionBankRequestItem_RequestItemId",
                        column: x => x.RequestItemId,
                        principalSchema: "hr",
                        principalTable: "QuestionBankRequestItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestItemReview_QuestionReviewDecision_DecisionId",
                        column: x => x.DecisionId,
                        principalSchema: "lkp",
                        principalTable: "QuestionReviewDecision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankRequestItemReview_QuestionRevision_ReviewedRevisionId",
                        column: x => x.ReviewedRevisionId,
                        principalSchema: "hr",
                        principalTable: "QuestionRevision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionBankVersionChange",
                schema: "hr",
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
                    QuestionBankId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangeRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangeTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromBankVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToBankVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OldQuestionRevisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NewQuestionRevisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OldSnapshotJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewSnapshotJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionBankVersionChange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersionChange_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersionChange_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersionChange_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersionChange_QuestionBankRequest_ChangeRequestId",
                        column: x => x.ChangeRequestId,
                        principalSchema: "hr",
                        principalTable: "QuestionBankRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersionChange_QuestionBankVersion_FromBankVersionId",
                        column: x => x.FromBankVersionId,
                        principalSchema: "hr",
                        principalTable: "QuestionBankVersion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersionChange_QuestionBankVersion_ToBankVersionId",
                        column: x => x.ToBankVersionId,
                        principalSchema: "hr",
                        principalTable: "QuestionBankVersion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersionChange_QuestionBank_QuestionBankId",
                        column: x => x.QuestionBankId,
                        principalSchema: "hr",
                        principalTable: "QuestionBank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersionChange_QuestionChangeType_ChangeTypeId",
                        column: x => x.ChangeTypeId,
                        principalSchema: "lkp",
                        principalTable: "QuestionChangeType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersionChange_QuestionRevision_NewQuestionRevisionId",
                        column: x => x.NewQuestionRevisionId,
                        principalSchema: "hr",
                        principalTable: "QuestionRevision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersionChange_QuestionRevision_OldQuestionRevisionId",
                        column: x => x.OldQuestionRevisionId,
                        principalSchema: "hr",
                        principalTable: "QuestionRevision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersionChange_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalSchema: "hr",
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionBankVersionQuestion",
                schema: "hr",
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
                    QuestionBankVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionRevisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceRequestItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionBankVersionQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersionQuestion_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersionQuestion_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersionQuestion_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersionQuestion_QuestionBankRequestItem_SourceRequestItemId",
                        column: x => x.SourceRequestItemId,
                        principalSchema: "hr",
                        principalTable: "QuestionBankRequestItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersionQuestion_QuestionBankVersion_QuestionBankVersionId",
                        column: x => x.QuestionBankVersionId,
                        principalSchema: "hr",
                        principalTable: "QuestionBankVersion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersionQuestion_QuestionRevision_QuestionRevisionId",
                        column: x => x.QuestionRevisionId,
                        principalSchema: "hr",
                        principalTable: "QuestionRevision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersionQuestion_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalSchema: "hr",
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionRevisionOption",
                schema: "hr",
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
                    QuestionRevisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OptionTextAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OptionTextEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionRevisionOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionRevisionOption_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionRevisionOption_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionRevisionOption_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionRevisionOption_QuestionRevision_QuestionRevisionId",
                        column: x => x.QuestionRevisionId,
                        principalSchema: "hr",
                        principalTable: "QuestionRevision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionBankVersionChangeDetail",
                schema: "hr",
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
                    VersionChangeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionBankVersionChangeDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersionChangeDetail_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersionChangeDetail_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersionChangeDetail_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionBankVersionChangeDetail_QuestionBankVersionChange_VersionChangeId",
                        column: x => x.VersionChangeId,
                        principalSchema: "hr",
                        principalTable: "QuestionBankVersionChange",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { -1900294407, "permission", "rooms.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -853440182, "permission", "rooms.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -721093517, "permission", "locations.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -688729387, "permission", "locations.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") }
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("26058720-5808-435a-abbf-d7a4e751f51e"),
                column: "EmployeeProfileId",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a8e0f354-2e23-41e1-9e1b-a1501b72dc4b"),
                column: "EmployeeProfileId",
                value: null);

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "CandidateType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[] { new Guid("f650aa0c-ab45-422c-9c9e-3b2246409ef2"), "PermanentResident", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "حامل الإقامة الدائمة", "Permanent Resident", 8, true, false, "حامل الإقامة الدائمة", "Permanent Resident", null, null });

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
                table: "ExamExemptionDecisionStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("5a79c067-a14d-4c8d-acac-97b965814679"), "Proposed", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "مقترح", "Proposed", null, null },
                    { new Guid("8df71003-be3b-4cdd-a965-9ca3f73a2551"), "Rejected", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "مرفوض", "Rejected", null, null },
                    { new Guid("c8bf2d58-54b1-40bc-a1ec-ec7af1427ed0"), "Cancelled", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 4, true, false, "ملغي", "Cancelled", null, null },
                    { new Guid("e902ef1e-7c78-4e44-a978-b40e962a41b8"), "Approved", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "معتمد", "Approved", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "ExamInterruptionPolicy",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("2153126f-8d36-4c35-a698-edb984b85829"), "RescheduleOnly", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "إعادة الجدولة فقط", "Reschedule Only", null, null },
                    { new Guid("546f8fe0-29ed-4dd2-a7dc-611248258166"), "ResumeOnly", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "الاستئناف فقط", "Resume Only", null, null },
                    { new Guid("7994a759-00d2-4dc7-806a-dbb311276160"), "ResumeOrReschedule", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "الاستئناف أو إعادة الجدولة", "Resume or Reschedule", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "ExamResultCandidateResultStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("24bd3d0e-7980-494c-a84d-167a81419030"), "Failed", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "راسب", "Failed", null, null },
                    { new Guid("385ff451-2293-4023-a33d-235f8d5fdb3f"), "NoShow", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "لم يحضر", "No Show", null, null },
                    { new Guid("76055057-d4b7-4791-a7df-8ab5c80e116d"), "Passed", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "ناجح", "Passed", null, null },
                    { new Guid("fb22f73d-0ab6-4031-a4bf-a219bbb07cd5"), "NotCompleted", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 4, true, false, "غير مكتمل", "Not Completed", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "ExamResultReportStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("3369b08f-2dd3-4485-a899-1515c63ce8b1"), "UnderReview", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "قيد المراجعة", "Under Review", null, null },
                    { new Guid("bac8cd24-bbb0-40ac-a800-9fe838622ad8"), "Approved", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 4, true, false, "معتمد", "Approved", null, null },
                    { new Guid("d2afd367-0383-44cf-acd6-dcf563acb053"), "Closed", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 5, true, false, "مغلق", "Closed", null, null },
                    { new Guid("ddff4a54-538e-4082-a799-97f6243f3d26"), "Creating", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "قيد الإنشاء", "Creating", null, null },
                    { new Guid("e5032918-cb5d-4ca4-a10a-95002a1e97a0"), "Returned", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "معاد", "Returned", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "ExamStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("3f33a442-953f-418c-af00-5220ee6a92b8"), "PendingApproval", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "قيد الاعتماد", "Pending Approval", null, null },
                    { new Guid("8d3322b5-9743-440d-a5d3-be71edc793a1"), "Draft", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "مسودة", "Draft", null, null },
                    { new Guid("a72cd921-edff-4f23-aea8-176b5b1ff3b7"), "Approved", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 4, true, false, "معتمد", "Approved", null, null },
                    { new Guid("e332af61-057e-4e65-a1a0-306be4ef41a1"), "Cancelled", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 5, true, false, "ملغي", "Cancelled", null, null },
                    { new Guid("eca730c1-fdac-4b1b-a678-70a34cdc607f"), "Returned", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "معاد", "Returned", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Permission",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsAssignableToRole", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0eb0193c-0ace-ac59-b02d-08e1679a4347"), "locations.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 82, true, true, false, "المواقع - عرض", "Locations - View", null, null },
                    { new Guid("6d31ec9d-8e04-005e-b326-7221d0fa2438"), "rooms.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 80, true, true, false, "القاعات - عرض", "Rooms - View", null, null },
                    { new Guid("a86ab789-83ea-2152-891b-382c2e970edc"), "rooms.manage", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 81, true, true, false, "إدارة القاعات", "Rooms - Manage", null, null },
                    { new Guid("ba0e8dfa-d8a2-5158-b2b9-2272331ea692"), "locations.manage", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 83, true, true, false, "إدارة المواقع", "Locations - Manage", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "RoomStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("4a728d18-ba82-4c00-a053-f95a6a638de4"), "Active", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "نشطة", "Active", null, null },
                    { new Guid("4e3a41f3-20d4-430b-a9fa-d175777b90ff"), "Inactive", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "غير نشطة", "Inactive", null, null },
                    { new Guid("c588f1a1-3666-41f7-a826-0c0b32594243"), "Maintenance", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "صيانة", "Maintenance", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "RoomType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("268c28c4-5ab5-4f2a-a050-901a46da9b86"), "LabRoom", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "غرفة مختبر", "Lab Room", null, null },
                    { new Guid("779f2dce-f563-492a-a434-44f24dd2d0ae"), "Other", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 4, true, false, "أخرى", "Other", null, null },
                    { new Guid("aa2ccf78-fff4-4b2c-a843-1d88f6029c68"), "ExamRoom", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "غرفة اختبار", "Exam Room", null, null },
                    { new Guid("fd3c28b0-60f4-4b4e-a61a-a148090691a2"), "InterviewRoom", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "غرفة مقابلة", "Interview Room", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "TestAttemptInterruptionResolutionAction",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("4ec227a3-e99b-4f7f-b7e7-c732a3d58b29"), "Reschedule", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "إعادة الجدولة", "Reschedule", null, null },
                    { new Guid("76698ef2-ed94-47e0-8772-72a49372f384"), "Resume", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "استئناف", "Resume", null, null },
                    { new Guid("a10637d3-dd47-4cf0-8c0f-9e480d7e0576"), "Cancel", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "إلغاء", "Cancel", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "TestAttemptInterruptionStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("b31c3a15-6d18-4e7b-bb38-c03e7baedc42"), "Resolved", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "تم الحل", "Resolved", null, null },
                    { new Guid("b3aa445b-ebfe-4278-a79c-e59e105844aa"), "Open", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "مفتوح", "Open", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "TestAttemptPartStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("07c2f269-6135-40f9-a146-6957271a4415"), "InProgress", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "قيد التنفيذ", "In Progress", null, null },
                    { new Guid("251ec109-529e-49c3-a3c0-626b974b5d37"), "Interrupted", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 5, true, false, "متوقف", "Interrupted", null, null },
                    { new Guid("2e520f0b-1fe3-4d5d-aa03-2d192de69599"), "NotStarted", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "لم يبدأ", "Not Started", null, null },
                    { new Guid("6a32c2bc-11ca-4834-a3ed-3eaa7f622f50"), "TimedOut", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 4, true, false, "انتهى الوقت", "Timed Out", null, null },
                    { new Guid("cee388fa-585f-480a-ac6f-6688143ff92e"), "Completed", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "مكتمل", "Completed", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "TestAttemptStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("470a5a87-8bf5-42e1-a350-22958764153d"), "TimedOut", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 5, true, false, "انتهى الوقت", "Timed Out", null, null },
                    { new Guid("4b10262f-4d4d-4075-a4fa-8dabcdedbeb1"), "Completed", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 4, true, false, "مكتمل", "Completed", null, null },
                    { new Guid("5a970db4-a244-449e-abe5-2500cd0d8a9a"), "Interrupted", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "متوقف", "Interrupted", null, null },
                    { new Guid("7adf1e9d-24f7-4fff-a2ec-248d14a6254b"), "Cancelled", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 6, true, false, "ملغي", "Cancelled", null, null },
                    { new Guid("f31ffad4-c04a-451a-abdd-877e4e9919a6"), "NotStarted", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "لم يبدأ", "Not Started", null, null },
                    { new Guid("f449c806-019b-48b6-a58d-1ba047ccc822"), "InProgress", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "قيد التنفيذ", "In Progress", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "TestSessionCandidateAttendanceStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("5f4bc80b-ca7f-4f55-a394-cc4102007a39"), "NoShow", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "لم يحضر", "No Show", null, null },
                    { new Guid("5fec8448-15bc-406e-a70c-7d90be7fd362"), "Pending", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "قيد الانتظار", "Pending", null, null },
                    { new Guid("a3250ce0-5679-43fa-a257-9f545507fa5e"), "Present", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "حاضر", "Present", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "TestSessionCandidateIdentityVerificationStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("58e27acd-91c7-4b11-a874-c28daa7916a5"), "Matched", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "مطابق", "Matched", null, null },
                    { new Guid("bcd7322c-f79d-41ba-a3dd-00407fc984a6"), "NotMatched", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "غير مطابق", "Not Matched", null, null },
                    { new Guid("d086861a-62a7-4f66-a014-b4245da3ff82"), "Pending", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "قيد الانتظار", "Pending", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "TestSessionCandidateStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("213abc60-4724-4f6c-a939-04eaf4deef78"), "Rescheduled", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 5, true, false, "أعيدت جدولته", "Rescheduled", null, null },
                    { new Guid("29747ce1-e3b6-45ff-a740-59730f5e5833"), "Authorized", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "مصرح", "Authorized", null, null },
                    { new Guid("b60c31b5-b819-4cf3-ae25-fd9d075b4db7"), "Completed", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 4, true, false, "مكتمل", "Completed", null, null },
                    { new Guid("c8a60391-f3b2-4afd-a0e3-15492812b141"), "Started", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "بدأ", "Started", null, null },
                    { new Guid("ca2ae8f7-8fc3-4c7d-ae0c-872b4e13b451"), "Cancelled", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 6, true, false, "ملغي", "Cancelled", null, null },
                    { new Guid("ec074f24-591a-41e4-ac6a-4b186c6043b0"), "Assigned", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "تم التعيين", "Assigned", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "TestSessionStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("11364bf4-c7bb-4326-a441-238864e53612"), "Draft", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "مسودة", "Draft", null, null },
                    { new Guid("16b3ac19-2ff1-491f-acca-7663c19c226b"), "InProgress", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 4, true, false, "قيد التنفيذ", "In Progress", null, null },
                    { new Guid("3e3d2acc-4a2b-476c-acc1-eac2e9bb5380"), "Ready", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "جاهزة", "Ready", null, null },
                    { new Guid("82b9fbdd-c9ec-46b5-a27b-001b47b22991"), "Closed", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 5, true, false, "مغلقة", "Closed", null, null },
                    { new Guid("a60f9202-8ab6-4c72-a5a4-43de299b46d6"), "Approved", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "معتمدة", "Approved", null, null },
                    { new Guid("e543df74-3311-441e-aa14-928a918cf952"), "Cancelled", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 6, true, false, "ملغاة", "Cancelled", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "TestSlotStaffRole",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("493cd1f0-2573-4140-a5e3-84c239b6a0c4"), "Support", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "دعم", "Support", null, null },
                    { new Guid("7020e361-a374-4ebd-ab69-dd95d1de8789"), "Monitor", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "مراقب", "Monitor", null, null },
                    { new Guid("f8535d6c-a02e-46d8-a124-db986e51d5fc"), "HallSupervisor", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "مشرف القاعة", "Hall Supervisor", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "TestSlotStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("02a5a982-e326-403c-a8da-34b2f33d88f0"), "Draft", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "مسودة", "Draft", null, null },
                    { new Guid("215d0bfd-1ad4-4613-af9e-79d977dfd637"), "Rescheduled", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 6, true, false, "أعيدت جدولته", "Rescheduled", null, null },
                    { new Guid("46ae567b-7f97-47ef-a960-9fd9efd32a53"), "Closed", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 5, true, false, "مغلق", "Closed", null, null },
                    { new Guid("4ff15b36-d258-48f4-ab1c-2380dfc0d1b0"), "Cancelled", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 7, true, false, "ملغي", "Cancelled", null, null },
                    { new Guid("7995eaf8-baaf-4dcc-a30d-0becc5a8e4d8"), "Approved", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "معتمد", "Approved", null, null },
                    { new Guid("cc71a094-b1b0-4c44-ab10-f48a8cc12140"), "Started", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 4, true, false, "بدأ", "Started", null, null },
                    { new Guid("fd397696-6596-4482-ae56-4c187813a65e"), "Ready", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "جاهز", "Ready", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "CandidateTypeProviderLogin",
                columns: new[] { "CandidateTypeId", "ProviderLoginId" },
                values: new object[,]
                {
                    { new Guid("f650aa0c-ab45-422c-9c9e-3b2246409ef2"), new Guid("b8854959-1e46-4595-b51f-de3c09e3ed85") },
                    { new Guid("f650aa0c-ab45-422c-9c9e-3b2246409ef2"), new Guid("fc379bb9-39f7-458a-85f8-6e54d1780178") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DifficultyLevel_BackendName",
                schema: "lkp",
                table: "DifficultyLevel",
                column: "BackendName",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_DifficultyLevel_CreatedById",
                schema: "lkp",
                table: "DifficultyLevel",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DifficultyLevel_CreatedDate",
                schema: "lkp",
                table: "DifficultyLevel",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_DifficultyLevel_DeletedById",
                schema: "lkp",
                table: "DifficultyLevel",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_DifficultyLevel_IsDeleted",
                schema: "lkp",
                table: "DifficultyLevel",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_DifficultyLevel_UpdatedById",
                schema: "lkp",
                table: "DifficultyLevel",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_ApprovedById",
                schema: "hr",
                table: "Exam",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_CreatedById",
                schema: "hr",
                table: "Exam",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_CreatedDate",
                schema: "hr",
                table: "Exam",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_DeletedById",
                schema: "hr",
                table: "Exam",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_InterruptionPolicyId",
                schema: "hr",
                table: "Exam",
                column: "InterruptionPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_IsDeleted",
                schema: "hr",
                table: "Exam",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_JobId",
                schema: "hr",
                table: "Exam",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_StatusId",
                schema: "hr",
                table: "Exam",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_UpdatedById",
                schema: "hr",
                table: "Exam",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamCategory_CategoryId",
                schema: "hr",
                table: "ExamCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamCategory_CreatedById",
                schema: "hr",
                table: "ExamCategory",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamCategory_CreatedDate",
                schema: "hr",
                table: "ExamCategory",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ExamCategory_DeletedById",
                schema: "hr",
                table: "ExamCategory",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamCategory_ExamPartId",
                schema: "hr",
                table: "ExamCategory",
                column: "ExamPartId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamCategory_IsDeleted",
                schema: "hr",
                table: "ExamCategory",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ExamCategory_UpdatedById",
                schema: "hr",
                table: "ExamCategory",
                column: "UpdatedById");

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
                name: "IX_ExamExemptionDecision_ApprovedById",
                schema: "hr",
                table: "ExamExemptionDecision",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamExemptionDecision_CreatedById",
                schema: "hr",
                table: "ExamExemptionDecision",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamExemptionDecision_CreatedDate",
                schema: "hr",
                table: "ExamExemptionDecision",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ExamExemptionDecision_DeletedById",
                schema: "hr",
                table: "ExamExemptionDecision",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamExemptionDecision_InvitationId",
                schema: "hr",
                table: "ExamExemptionDecision",
                column: "InvitationId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamExemptionDecision_IsDeleted",
                schema: "hr",
                table: "ExamExemptionDecision",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ExamExemptionDecision_StatusId",
                schema: "hr",
                table: "ExamExemptionDecision",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamExemptionDecision_UpdatedById",
                schema: "hr",
                table: "ExamExemptionDecision",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamExemptionDecisionStatus_BackendName",
                schema: "lkp",
                table: "ExamExemptionDecisionStatus",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamExemptionDecisionStatus_CreatedById",
                schema: "lkp",
                table: "ExamExemptionDecisionStatus",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamExemptionDecisionStatus_CreatedDate",
                schema: "lkp",
                table: "ExamExemptionDecisionStatus",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ExamExemptionDecisionStatus_DeletedById",
                schema: "lkp",
                table: "ExamExemptionDecisionStatus",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamExemptionDecisionStatus_DisplayOrder",
                schema: "lkp",
                table: "ExamExemptionDecisionStatus",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ExamExemptionDecisionStatus_IsDeleted",
                schema: "lkp",
                table: "ExamExemptionDecisionStatus",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ExamExemptionDecisionStatus_UpdatedById",
                schema: "lkp",
                table: "ExamExemptionDecisionStatus",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamInterruptionPolicy_BackendName",
                schema: "lkp",
                table: "ExamInterruptionPolicy",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamInterruptionPolicy_CreatedById",
                schema: "lkp",
                table: "ExamInterruptionPolicy",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamInterruptionPolicy_CreatedDate",
                schema: "lkp",
                table: "ExamInterruptionPolicy",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ExamInterruptionPolicy_DeletedById",
                schema: "lkp",
                table: "ExamInterruptionPolicy",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamInterruptionPolicy_DisplayOrder",
                schema: "lkp",
                table: "ExamInterruptionPolicy",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ExamInterruptionPolicy_IsDeleted",
                schema: "lkp",
                table: "ExamInterruptionPolicy",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ExamInterruptionPolicy_UpdatedById",
                schema: "lkp",
                table: "ExamInterruptionPolicy",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamPart_CreatedById",
                schema: "hr",
                table: "ExamPart",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamPart_CreatedDate",
                schema: "hr",
                table: "ExamPart",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ExamPart_DeletedById",
                schema: "hr",
                table: "ExamPart",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamPart_ExamId",
                schema: "hr",
                table: "ExamPart",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamPart_IsDeleted",
                schema: "hr",
                table: "ExamPart",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ExamPart_UpdatedById",
                schema: "hr",
                table: "ExamPart",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultCandidate_CreatedById",
                schema: "hr",
                table: "ExamResultCandidate",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultCandidate_CreatedDate",
                schema: "hr",
                table: "ExamResultCandidate",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultCandidate_DeletedById",
                schema: "hr",
                table: "ExamResultCandidate",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultCandidate_ExamResultReportId",
                schema: "hr",
                table: "ExamResultCandidate",
                column: "ExamResultReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultCandidate_InvitationId",
                schema: "hr",
                table: "ExamResultCandidate",
                column: "InvitationId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultCandidate_IsDeleted",
                schema: "hr",
                table: "ExamResultCandidate",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultCandidate_ResultStatusId",
                schema: "hr",
                table: "ExamResultCandidate",
                column: "ResultStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultCandidate_TestAttemptId",
                schema: "hr",
                table: "ExamResultCandidate",
                column: "TestAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultCandidate_UpdatedById",
                schema: "hr",
                table: "ExamResultCandidate",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultCandidateResultStatus_BackendName",
                schema: "lkp",
                table: "ExamResultCandidateResultStatus",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultCandidateResultStatus_CreatedById",
                schema: "lkp",
                table: "ExamResultCandidateResultStatus",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultCandidateResultStatus_CreatedDate",
                schema: "lkp",
                table: "ExamResultCandidateResultStatus",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultCandidateResultStatus_DeletedById",
                schema: "lkp",
                table: "ExamResultCandidateResultStatus",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultCandidateResultStatus_DisplayOrder",
                schema: "lkp",
                table: "ExamResultCandidateResultStatus",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultCandidateResultStatus_IsDeleted",
                schema: "lkp",
                table: "ExamResultCandidateResultStatus",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultCandidateResultStatus_UpdatedById",
                schema: "lkp",
                table: "ExamResultCandidateResultStatus",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultReport_ApprovedById",
                schema: "hr",
                table: "ExamResultReport",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultReport_CreatedById",
                schema: "hr",
                table: "ExamResultReport",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultReport_CreatedDate",
                schema: "hr",
                table: "ExamResultReport",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultReport_DeletedById",
                schema: "hr",
                table: "ExamResultReport",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultReport_ExamId",
                schema: "hr",
                table: "ExamResultReport",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultReport_IsDeleted",
                schema: "hr",
                table: "ExamResultReport",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultReport_StatusId",
                schema: "hr",
                table: "ExamResultReport",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultReport_UpdatedById",
                schema: "hr",
                table: "ExamResultReport",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultReportStatus_BackendName",
                schema: "lkp",
                table: "ExamResultReportStatus",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultReportStatus_CreatedById",
                schema: "lkp",
                table: "ExamResultReportStatus",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultReportStatus_CreatedDate",
                schema: "lkp",
                table: "ExamResultReportStatus",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultReportStatus_DeletedById",
                schema: "lkp",
                table: "ExamResultReportStatus",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultReportStatus_DisplayOrder",
                schema: "lkp",
                table: "ExamResultReportStatus",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultReportStatus_IsDeleted",
                schema: "lkp",
                table: "ExamResultReportStatus",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultReportStatus_UpdatedById",
                schema: "lkp",
                table: "ExamResultReportStatus",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamStatus_BackendName",
                schema: "lkp",
                table: "ExamStatus",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamStatus_CreatedById",
                schema: "lkp",
                table: "ExamStatus",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamStatus_CreatedDate",
                schema: "lkp",
                table: "ExamStatus",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ExamStatus_DeletedById",
                schema: "lkp",
                table: "ExamStatus",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExamStatus_DisplayOrder",
                schema: "lkp",
                table: "ExamStatus",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ExamStatus_IsDeleted",
                schema: "lkp",
                table: "ExamStatus",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ExamStatus_UpdatedById",
                schema: "lkp",
                table: "ExamStatus",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Location_CreatedById",
                schema: "hr",
                table: "Location",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Location_CreatedDate",
                schema: "hr",
                table: "Location",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Location_DeletedById",
                schema: "hr",
                table: "Location",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Location_IsDeleted",
                schema: "hr",
                table: "Location",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Location_UpdatedById",
                schema: "hr",
                table: "Location",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Question_CreatedById",
                schema: "hr",
                table: "Question",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Question_CreatedDate",
                schema: "hr",
                table: "Question",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Question_DeletedById",
                schema: "hr",
                table: "Question",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Question_IsDeleted",
                schema: "hr",
                table: "Question",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Question_QuestionCode",
                schema: "hr",
                table: "Question",
                column: "QuestionCode",
                unique: true,
                filter: "[QuestionCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Question_UpdatedById",
                schema: "hr",
                table: "Question",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBank_CreatedById",
                schema: "hr",
                table: "QuestionBank",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBank_CreatedDate",
                schema: "hr",
                table: "QuestionBank",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBank_CurrentApprovedVersionId",
                schema: "hr",
                table: "QuestionBank",
                column: "CurrentApprovedVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBank_DeletedById",
                schema: "hr",
                table: "QuestionBank",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBank_IsDeleted",
                schema: "hr",
                table: "QuestionBank",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBank_JobTitleId",
                schema: "hr",
                table: "QuestionBank",
                column: "JobTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBank_ManagementId",
                schema: "hr",
                table: "QuestionBank",
                column: "ManagementId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBank_QuestionBankTypeId",
                schema: "hr",
                table: "QuestionBank",
                column: "QuestionBankTypeId",
                unique: true,
                filter: "[IsDeleted] = 0 AND [QuestionBankTypeId] IN ('686928db-b630-4689-b265-ae17eded73cf', '0ef8f0d9-02d7-4863-9bfe-e63355f3686e')");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBank_QuestionBankTypeId_ManagementId_JobTitleId",
                schema: "hr",
                table: "QuestionBank",
                columns: new[] { "QuestionBankTypeId", "ManagementId", "JobTitleId" },
                unique: true,
                filter: "[IsDeleted] = 0 AND [QuestionBankTypeId] = 'a7f9646b-fcc3-4a00-944c-ceed82acd557'");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBank_StageId",
                schema: "hr",
                table: "QuestionBank",
                column: "StageId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBank_UpdatedById",
                schema: "hr",
                table: "QuestionBank",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankAssignment_AssignedByUserId",
                schema: "hr",
                table: "QuestionBankAssignment",
                column: "AssignedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankAssignment_CreatedById",
                schema: "hr",
                table: "QuestionBankAssignment",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankAssignment_CreatedDate",
                schema: "hr",
                table: "QuestionBankAssignment",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankAssignment_DeletedById",
                schema: "hr",
                table: "QuestionBankAssignment",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankAssignment_EmployeeId",
                schema: "hr",
                table: "QuestionBankAssignment",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankAssignment_IsDeleted",
                schema: "hr",
                table: "QuestionBankAssignment",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankAssignment_QuestionBankRequestId_EmployeeId",
                schema: "hr",
                table: "QuestionBankAssignment",
                columns: new[] { "QuestionBankRequestId", "EmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankAssignment_StatusId",
                schema: "hr",
                table: "QuestionBankAssignment",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankAssignment_UpdatedById",
                schema: "hr",
                table: "QuestionBankAssignment",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankAssignmentStatus_Code",
                schema: "lkp",
                table: "QuestionBankAssignmentStatus",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequest_BaseVersionId",
                schema: "hr",
                table: "QuestionBankRequest",
                column: "BaseVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequest_CreatedById",
                schema: "hr",
                table: "QuestionBankRequest",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequest_CreatedDate",
                schema: "hr",
                table: "QuestionBankRequest",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequest_DeletedById",
                schema: "hr",
                table: "QuestionBankRequest",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequest_FinalDecisionById",
                schema: "hr",
                table: "QuestionBankRequest",
                column: "FinalDecisionById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequest_IsDeleted",
                schema: "hr",
                table: "QuestionBankRequest",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequest_QuestionBankId",
                schema: "hr",
                table: "QuestionBankRequest",
                column: "QuestionBankId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequest_RequestTypeId",
                schema: "hr",
                table: "QuestionBankRequest",
                column: "RequestTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequest_StatusId",
                schema: "hr",
                table: "QuestionBankRequest",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequest_SubmittedById",
                schema: "hr",
                table: "QuestionBankRequest",
                column: "SubmittedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequest_UpdatedById",
                schema: "hr",
                table: "QuestionBankRequest",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestHistory_CreatedById",
                schema: "hr",
                table: "QuestionBankRequestHistory",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestHistory_CreatedDate",
                schema: "hr",
                table: "QuestionBankRequestHistory",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestHistory_DeletedById",
                schema: "hr",
                table: "QuestionBankRequestHistory",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestHistory_FromStatusId",
                schema: "hr",
                table: "QuestionBankRequestHistory",
                column: "FromStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestHistory_IsDeleted",
                schema: "hr",
                table: "QuestionBankRequestHistory",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestHistory_PerformedById",
                schema: "hr",
                table: "QuestionBankRequestHistory",
                column: "PerformedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestHistory_RequestId_PerformedAt",
                schema: "hr",
                table: "QuestionBankRequestHistory",
                columns: new[] { "RequestId", "PerformedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestHistory_ToStatusId",
                schema: "hr",
                table: "QuestionBankRequestHistory",
                column: "ToStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestHistory_UpdatedById",
                schema: "hr",
                table: "QuestionBankRequestHistory",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItem_ChangeTypeId",
                schema: "hr",
                table: "QuestionBankRequestItem",
                column: "ChangeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItem_CreatedById",
                schema: "hr",
                table: "QuestionBankRequestItem",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItem_CreatedDate",
                schema: "hr",
                table: "QuestionBankRequestItem",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItem_CurrentProposedRevisionId",
                schema: "hr",
                table: "QuestionBankRequestItem",
                column: "CurrentProposedRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItem_DeletedById",
                schema: "hr",
                table: "QuestionBankRequestItem",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItem_IsDeleted",
                schema: "hr",
                table: "QuestionBankRequestItem",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItem_OriginalRevisionId",
                schema: "hr",
                table: "QuestionBankRequestItem",
                column: "OriginalRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItem_QuestionBankAssignmentId",
                schema: "hr",
                table: "QuestionBankRequestItem",
                column: "QuestionBankAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItem_QuestionId",
                schema: "hr",
                table: "QuestionBankRequestItem",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItem_RemovedById",
                schema: "hr",
                table: "QuestionBankRequestItem",
                column: "RemovedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItem_RequestId_QuestionId",
                schema: "hr",
                table: "QuestionBankRequestItem",
                columns: new[] { "RequestId", "QuestionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItem_StatusId",
                schema: "hr",
                table: "QuestionBankRequestItem",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItem_UpdatedById",
                schema: "hr",
                table: "QuestionBankRequestItem",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItemReview_CreatedById",
                schema: "hr",
                table: "QuestionBankRequestItemReview",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItemReview_CreatedDate",
                schema: "hr",
                table: "QuestionBankRequestItemReview",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItemReview_DecisionId",
                schema: "hr",
                table: "QuestionBankRequestItemReview",
                column: "DecisionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItemReview_DeletedById",
                schema: "hr",
                table: "QuestionBankRequestItemReview",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItemReview_IsDeleted",
                schema: "hr",
                table: "QuestionBankRequestItemReview",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItemReview_RequestItemId_ReviewRound",
                schema: "hr",
                table: "QuestionBankRequestItemReview",
                columns: new[] { "RequestItemId", "ReviewRound" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItemReview_ReviewedById",
                schema: "hr",
                table: "QuestionBankRequestItemReview",
                column: "ReviewedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItemReview_ReviewedRevisionId",
                schema: "hr",
                table: "QuestionBankRequestItemReview",
                column: "ReviewedRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItemReview_UpdatedById",
                schema: "hr",
                table: "QuestionBankRequestItemReview",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItemStatus_BackendName",
                schema: "lkp",
                table: "QuestionBankRequestItemStatus",
                column: "BackendName",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItemStatus_CreatedById",
                schema: "lkp",
                table: "QuestionBankRequestItemStatus",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItemStatus_CreatedDate",
                schema: "lkp",
                table: "QuestionBankRequestItemStatus",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItemStatus_DeletedById",
                schema: "lkp",
                table: "QuestionBankRequestItemStatus",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItemStatus_IsDeleted",
                schema: "lkp",
                table: "QuestionBankRequestItemStatus",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestItemStatus_UpdatedById",
                schema: "lkp",
                table: "QuestionBankRequestItemStatus",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestReview_CreatedById",
                schema: "hr",
                table: "QuestionBankRequestReview",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestReview_CreatedDate",
                schema: "hr",
                table: "QuestionBankRequestReview",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestReview_DeletedById",
                schema: "hr",
                table: "QuestionBankRequestReview",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestReview_IsDeleted",
                schema: "hr",
                table: "QuestionBankRequestReview",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestReview_RequestId_ReviewRound",
                schema: "hr",
                table: "QuestionBankRequestReview",
                columns: new[] { "RequestId", "ReviewRound" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestReview_ReviewedById",
                schema: "hr",
                table: "QuestionBankRequestReview",
                column: "ReviewedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestReview_UpdatedById",
                schema: "hr",
                table: "QuestionBankRequestReview",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestStatus_BackendName",
                schema: "lkp",
                table: "QuestionBankRequestStatus",
                column: "BackendName",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestStatus_CreatedById",
                schema: "lkp",
                table: "QuestionBankRequestStatus",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestStatus_CreatedDate",
                schema: "lkp",
                table: "QuestionBankRequestStatus",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestStatus_DeletedById",
                schema: "lkp",
                table: "QuestionBankRequestStatus",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestStatus_IsDeleted",
                schema: "lkp",
                table: "QuestionBankRequestStatus",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestStatus_UpdatedById",
                schema: "lkp",
                table: "QuestionBankRequestStatus",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestType_BackendName",
                schema: "lkp",
                table: "QuestionBankRequestType",
                column: "BackendName",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestType_CreatedById",
                schema: "lkp",
                table: "QuestionBankRequestType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestType_CreatedDate",
                schema: "lkp",
                table: "QuestionBankRequestType",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestType_DeletedById",
                schema: "lkp",
                table: "QuestionBankRequestType",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestType_IsDeleted",
                schema: "lkp",
                table: "QuestionBankRequestType",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankRequestType_UpdatedById",
                schema: "lkp",
                table: "QuestionBankRequestType",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankType_BackendName",
                schema: "lkp",
                table: "QuestionBankType",
                column: "BackendName",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankType_CreatedById",
                schema: "lkp",
                table: "QuestionBankType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankType_CreatedDate",
                schema: "lkp",
                table: "QuestionBankType",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankType_DeletedById",
                schema: "lkp",
                table: "QuestionBankType",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankType_IsDeleted",
                schema: "lkp",
                table: "QuestionBankType",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankType_UpdatedById",
                schema: "lkp",
                table: "QuestionBankType",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersion_ApprovedById",
                schema: "hr",
                table: "QuestionBankVersion",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersion_CreatedById",
                schema: "hr",
                table: "QuestionBankVersion",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersion_CreatedDate",
                schema: "hr",
                table: "QuestionBankVersion",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersion_CreatedFromRequestId",
                schema: "hr",
                table: "QuestionBankVersion",
                column: "CreatedFromRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersion_DeletedById",
                schema: "hr",
                table: "QuestionBankVersion",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersion_IsDeleted",
                schema: "hr",
                table: "QuestionBankVersion",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersion_PreviousVersionId",
                schema: "hr",
                table: "QuestionBankVersion",
                column: "PreviousVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersion_QuestionBankId_VersionNo",
                schema: "hr",
                table: "QuestionBankVersion",
                columns: new[] { "QuestionBankId", "VersionNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersion_UpdatedById",
                schema: "hr",
                table: "QuestionBankVersion",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionChange_ChangedById",
                schema: "hr",
                table: "QuestionBankVersionChange",
                column: "ChangedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionChange_ChangeRequestId",
                schema: "hr",
                table: "QuestionBankVersionChange",
                column: "ChangeRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionChange_ChangeTypeId",
                schema: "hr",
                table: "QuestionBankVersionChange",
                column: "ChangeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionChange_CreatedById",
                schema: "hr",
                table: "QuestionBankVersionChange",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionChange_CreatedDate",
                schema: "hr",
                table: "QuestionBankVersionChange",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionChange_DeletedById",
                schema: "hr",
                table: "QuestionBankVersionChange",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionChange_FromBankVersionId",
                schema: "hr",
                table: "QuestionBankVersionChange",
                column: "FromBankVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionChange_IsDeleted",
                schema: "hr",
                table: "QuestionBankVersionChange",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionChange_NewQuestionRevisionId",
                schema: "hr",
                table: "QuestionBankVersionChange",
                column: "NewQuestionRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionChange_OldQuestionRevisionId",
                schema: "hr",
                table: "QuestionBankVersionChange",
                column: "OldQuestionRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionChange_QuestionBankId_ChangedAt",
                schema: "hr",
                table: "QuestionBankVersionChange",
                columns: new[] { "QuestionBankId", "ChangedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionChange_QuestionId",
                schema: "hr",
                table: "QuestionBankVersionChange",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionChange_ToBankVersionId_QuestionId",
                schema: "hr",
                table: "QuestionBankVersionChange",
                columns: new[] { "ToBankVersionId", "QuestionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionChange_UpdatedById",
                schema: "hr",
                table: "QuestionBankVersionChange",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionChangeDetail_CreatedById",
                schema: "hr",
                table: "QuestionBankVersionChangeDetail",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionChangeDetail_CreatedDate",
                schema: "hr",
                table: "QuestionBankVersionChangeDetail",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionChangeDetail_DeletedById",
                schema: "hr",
                table: "QuestionBankVersionChangeDetail",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionChangeDetail_IsDeleted",
                schema: "hr",
                table: "QuestionBankVersionChangeDetail",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionChangeDetail_UpdatedById",
                schema: "hr",
                table: "QuestionBankVersionChangeDetail",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionChangeDetail_VersionChangeId",
                schema: "hr",
                table: "QuestionBankVersionChangeDetail",
                column: "VersionChangeId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionQuestion_CreatedById",
                schema: "hr",
                table: "QuestionBankVersionQuestion",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionQuestion_CreatedDate",
                schema: "hr",
                table: "QuestionBankVersionQuestion",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionQuestion_DeletedById",
                schema: "hr",
                table: "QuestionBankVersionQuestion",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionQuestion_IsDeleted",
                schema: "hr",
                table: "QuestionBankVersionQuestion",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionQuestion_QuestionBankVersionId_QuestionId",
                schema: "hr",
                table: "QuestionBankVersionQuestion",
                columns: new[] { "QuestionBankVersionId", "QuestionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionQuestion_QuestionId",
                schema: "hr",
                table: "QuestionBankVersionQuestion",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionQuestion_QuestionRevisionId",
                schema: "hr",
                table: "QuestionBankVersionQuestion",
                column: "QuestionRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionQuestion_SourceRequestItemId",
                schema: "hr",
                table: "QuestionBankVersionQuestion",
                column: "SourceRequestItemId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankVersionQuestion_UpdatedById",
                schema: "hr",
                table: "QuestionBankVersionQuestion",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionChangeType_BackendName",
                schema: "lkp",
                table: "QuestionChangeType",
                column: "BackendName",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionChangeType_CreatedById",
                schema: "lkp",
                table: "QuestionChangeType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionChangeType_CreatedDate",
                schema: "lkp",
                table: "QuestionChangeType",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionChangeType_DeletedById",
                schema: "lkp",
                table: "QuestionChangeType",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionChangeType_IsDeleted",
                schema: "lkp",
                table: "QuestionChangeType",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionChangeType_UpdatedById",
                schema: "lkp",
                table: "QuestionChangeType",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionReviewDecision_BackendName",
                schema: "lkp",
                table: "QuestionReviewDecision",
                column: "BackendName",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionReviewDecision_CreatedById",
                schema: "lkp",
                table: "QuestionReviewDecision",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionReviewDecision_CreatedDate",
                schema: "lkp",
                table: "QuestionReviewDecision",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionReviewDecision_DeletedById",
                schema: "lkp",
                table: "QuestionReviewDecision",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionReviewDecision_IsDeleted",
                schema: "lkp",
                table: "QuestionReviewDecision",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionReviewDecision_UpdatedById",
                schema: "lkp",
                table: "QuestionReviewDecision",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionRevision_CreatedById",
                schema: "hr",
                table: "QuestionRevision",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionRevision_CreatedDate",
                schema: "hr",
                table: "QuestionRevision",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionRevision_DeletedById",
                schema: "hr",
                table: "QuestionRevision",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionRevision_DifficultyLevelId",
                schema: "hr",
                table: "QuestionRevision",
                column: "DifficultyLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionRevision_IsDeleted",
                schema: "hr",
                table: "QuestionRevision",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionRevision_QuestionId_RevisionNo",
                schema: "hr",
                table: "QuestionRevision",
                columns: new[] { "QuestionId", "RevisionNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionRevision_QuestionTypeId",
                schema: "hr",
                table: "QuestionRevision",
                column: "QuestionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionRevision_ResourceId",
                schema: "hr",
                table: "QuestionRevision",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionRevision_SourceRequestItemId",
                schema: "hr",
                table: "QuestionRevision",
                column: "SourceRequestItemId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionRevision_UpdatedById",
                schema: "hr",
                table: "QuestionRevision",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionRevisionOption_CreatedById",
                schema: "hr",
                table: "QuestionRevisionOption",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionRevisionOption_CreatedDate",
                schema: "hr",
                table: "QuestionRevisionOption",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionRevisionOption_DeletedById",
                schema: "hr",
                table: "QuestionRevisionOption",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionRevisionOption_IsDeleted",
                schema: "hr",
                table: "QuestionRevisionOption",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionRevisionOption_QuestionRevisionId_DisplayOrder",
                schema: "hr",
                table: "QuestionRevisionOption",
                columns: new[] { "QuestionRevisionId", "DisplayOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionRevisionOption_ResourceId",
                schema: "hr",
                table: "QuestionRevisionOption",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionRevisionOption_UpdatedById",
                schema: "hr",
                table: "QuestionRevisionOption",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionType_BackendName",
                schema: "lkp",
                table: "QuestionType",
                column: "BackendName",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionType_CreatedById",
                schema: "lkp",
                table: "QuestionType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionType_CreatedDate",
                schema: "lkp",
                table: "QuestionType",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionType_DeletedById",
                schema: "lkp",
                table: "QuestionType",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionType_IsDeleted",
                schema: "lkp",
                table: "QuestionType",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionType_UpdatedById",
                schema: "lkp",
                table: "QuestionType",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Room_CreatedById",
                schema: "hr",
                table: "Room",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Room_CreatedDate",
                schema: "hr",
                table: "Room",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Room_DeletedById",
                schema: "hr",
                table: "Room",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Room_IsDeleted",
                schema: "hr",
                table: "Room",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Room_LocationId",
                schema: "hr",
                table: "Room",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_RoomTypeId",
                schema: "hr",
                table: "Room",
                column: "RoomTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_StatusId",
                schema: "hr",
                table: "Room",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_UpdatedById",
                schema: "hr",
                table: "Room",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RoomStatus_BackendName",
                schema: "lkp",
                table: "RoomStatus",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomStatus_CreatedById",
                schema: "lkp",
                table: "RoomStatus",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RoomStatus_CreatedDate",
                schema: "lkp",
                table: "RoomStatus",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_RoomStatus_DeletedById",
                schema: "lkp",
                table: "RoomStatus",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_RoomStatus_DisplayOrder",
                schema: "lkp",
                table: "RoomStatus",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_RoomStatus_IsDeleted",
                schema: "lkp",
                table: "RoomStatus",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_RoomStatus_UpdatedById",
                schema: "lkp",
                table: "RoomStatus",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RoomType_BackendName",
                schema: "lkp",
                table: "RoomType",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomType_CreatedById",
                schema: "lkp",
                table: "RoomType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RoomType_CreatedDate",
                schema: "lkp",
                table: "RoomType",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_RoomType_DeletedById",
                schema: "lkp",
                table: "RoomType",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_RoomType_DisplayOrder",
                schema: "lkp",
                table: "RoomType",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_RoomType_IsDeleted",
                schema: "lkp",
                table: "RoomType",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_RoomType_UpdatedById",
                schema: "lkp",
                table: "RoomType",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Stage_BackendName",
                schema: "lkp",
                table: "Stage",
                column: "BackendName",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Stage_CreatedById",
                schema: "lkp",
                table: "Stage",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Stage_CreatedDate",
                schema: "lkp",
                table: "Stage",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Stage_DeletedById",
                schema: "lkp",
                table: "Stage",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Stage_IsDeleted",
                schema: "lkp",
                table: "Stage",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Stage_UpdatedById",
                schema: "lkp",
                table: "Stage",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttempt_CreatedById",
                schema: "hr",
                table: "TestAttempt",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttempt_CreatedDate",
                schema: "hr",
                table: "TestAttempt",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttempt_DeletedById",
                schema: "hr",
                table: "TestAttempt",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttempt_IsDeleted",
                schema: "hr",
                table: "TestAttempt",
                column: "IsDeleted");

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
                name: "IX_TestAttempt_UpdatedById",
                schema: "hr",
                table: "TestAttempt",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptInterruption_CreatedById",
                schema: "hr",
                table: "TestAttemptInterruption",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptInterruption_CreatedDate",
                schema: "hr",
                table: "TestAttemptInterruption",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptInterruption_DeletedById",
                schema: "hr",
                table: "TestAttemptInterruption",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptInterruption_IsDeleted",
                schema: "hr",
                table: "TestAttemptInterruption",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptInterruption_ResolutionActionId",
                schema: "hr",
                table: "TestAttemptInterruption",
                column: "ResolutionActionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptInterruption_ResolvedById",
                schema: "hr",
                table: "TestAttemptInterruption",
                column: "ResolvedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptInterruption_StatusId",
                schema: "hr",
                table: "TestAttemptInterruption",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptInterruption_TestAttemptId",
                schema: "hr",
                table: "TestAttemptInterruption",
                column: "TestAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptInterruption_TestAttemptPartId",
                schema: "hr",
                table: "TestAttemptInterruption",
                column: "TestAttemptPartId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptInterruption_UpdatedById",
                schema: "hr",
                table: "TestAttemptInterruption",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptInterruptionResolutionAction_BackendName",
                schema: "lkp",
                table: "TestAttemptInterruptionResolutionAction",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptInterruptionResolutionAction_CreatedById",
                schema: "lkp",
                table: "TestAttemptInterruptionResolutionAction",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptInterruptionResolutionAction_CreatedDate",
                schema: "lkp",
                table: "TestAttemptInterruptionResolutionAction",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptInterruptionResolutionAction_DeletedById",
                schema: "lkp",
                table: "TestAttemptInterruptionResolutionAction",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptInterruptionResolutionAction_DisplayOrder",
                schema: "lkp",
                table: "TestAttemptInterruptionResolutionAction",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptInterruptionResolutionAction_IsDeleted",
                schema: "lkp",
                table: "TestAttemptInterruptionResolutionAction",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptInterruptionResolutionAction_UpdatedById",
                schema: "lkp",
                table: "TestAttemptInterruptionResolutionAction",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptInterruptionStatus_BackendName",
                schema: "lkp",
                table: "TestAttemptInterruptionStatus",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptInterruptionStatus_CreatedById",
                schema: "lkp",
                table: "TestAttemptInterruptionStatus",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptInterruptionStatus_CreatedDate",
                schema: "lkp",
                table: "TestAttemptInterruptionStatus",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptInterruptionStatus_DeletedById",
                schema: "lkp",
                table: "TestAttemptInterruptionStatus",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptInterruptionStatus_DisplayOrder",
                schema: "lkp",
                table: "TestAttemptInterruptionStatus",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptInterruptionStatus_IsDeleted",
                schema: "lkp",
                table: "TestAttemptInterruptionStatus",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptInterruptionStatus_UpdatedById",
                schema: "lkp",
                table: "TestAttemptInterruptionStatus",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptPart_CreatedById",
                schema: "hr",
                table: "TestAttemptPart",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptPart_CreatedDate",
                schema: "hr",
                table: "TestAttemptPart",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptPart_DeletedById",
                schema: "hr",
                table: "TestAttemptPart",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptPart_ExamPartId",
                schema: "hr",
                table: "TestAttemptPart",
                column: "ExamPartId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptPart_IsDeleted",
                schema: "hr",
                table: "TestAttemptPart",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptPart_StatusId",
                schema: "hr",
                table: "TestAttemptPart",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptPart_TestAttemptId",
                schema: "hr",
                table: "TestAttemptPart",
                column: "TestAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptPart_UpdatedById",
                schema: "hr",
                table: "TestAttemptPart",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptPartStatus_BackendName",
                schema: "lkp",
                table: "TestAttemptPartStatus",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptPartStatus_CreatedById",
                schema: "lkp",
                table: "TestAttemptPartStatus",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptPartStatus_CreatedDate",
                schema: "lkp",
                table: "TestAttemptPartStatus",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptPartStatus_DeletedById",
                schema: "lkp",
                table: "TestAttemptPartStatus",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptPartStatus_DisplayOrder",
                schema: "lkp",
                table: "TestAttemptPartStatus",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptPartStatus_IsDeleted",
                schema: "lkp",
                table: "TestAttemptPartStatus",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptPartStatus_UpdatedById",
                schema: "lkp",
                table: "TestAttemptPartStatus",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptQuestion_CreatedById",
                schema: "hr",
                table: "TestAttemptQuestion",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptQuestion_CreatedDate",
                schema: "hr",
                table: "TestAttemptQuestion",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptQuestion_DeletedById",
                schema: "hr",
                table: "TestAttemptQuestion",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptQuestion_ExamCategoryId",
                schema: "hr",
                table: "TestAttemptQuestion",
                column: "ExamCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptQuestion_IsDeleted",
                schema: "hr",
                table: "TestAttemptQuestion",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptQuestion_TestAttemptId",
                schema: "hr",
                table: "TestAttemptQuestion",
                column: "TestAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptQuestion_UpdatedById",
                schema: "hr",
                table: "TestAttemptQuestion",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptStatus_BackendName",
                schema: "lkp",
                table: "TestAttemptStatus",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptStatus_CreatedById",
                schema: "lkp",
                table: "TestAttemptStatus",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptStatus_CreatedDate",
                schema: "lkp",
                table: "TestAttemptStatus",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptStatus_DeletedById",
                schema: "lkp",
                table: "TestAttemptStatus",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptStatus_DisplayOrder",
                schema: "lkp",
                table: "TestAttemptStatus",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptStatus_IsDeleted",
                schema: "lkp",
                table: "TestAttemptStatus",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptStatus_UpdatedById",
                schema: "lkp",
                table: "TestAttemptStatus",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSession_CreatedById",
                schema: "hr",
                table: "TestSession",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSession_CreatedDate",
                schema: "hr",
                table: "TestSession",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TestSession_DeletedById",
                schema: "hr",
                table: "TestSession",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSession_ExamId",
                schema: "hr",
                table: "TestSession",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_TestSession_IsDeleted",
                schema: "hr",
                table: "TestSession",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TestSession_StatusId",
                schema: "hr",
                table: "TestSession",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TestSession_TestSlotId",
                schema: "hr",
                table: "TestSession",
                column: "TestSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_TestSession_UpdatedById",
                schema: "hr",
                table: "TestSession",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidate_AttendanceStatusId",
                schema: "hr",
                table: "TestSessionCandidate",
                column: "AttendanceStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidate_CreatedById",
                schema: "hr",
                table: "TestSessionCandidate",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidate_CreatedDate",
                schema: "hr",
                table: "TestSessionCandidate",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidate_DeletedById",
                schema: "hr",
                table: "TestSessionCandidate",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidate_IdentityVerificationStatusId",
                schema: "hr",
                table: "TestSessionCandidate",
                column: "IdentityVerificationStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidate_InvitationId",
                schema: "hr",
                table: "TestSessionCandidate",
                column: "InvitationId");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidate_IsDeleted",
                schema: "hr",
                table: "TestSessionCandidate",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidate_StatusId",
                schema: "hr",
                table: "TestSessionCandidate",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidate_TestSessionId",
                schema: "hr",
                table: "TestSessionCandidate",
                column: "TestSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidate_UpdatedById",
                schema: "hr",
                table: "TestSessionCandidate",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidateAttendanceStatus_BackendName",
                schema: "lkp",
                table: "TestSessionCandidateAttendanceStatus",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidateAttendanceStatus_CreatedById",
                schema: "lkp",
                table: "TestSessionCandidateAttendanceStatus",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidateAttendanceStatus_CreatedDate",
                schema: "lkp",
                table: "TestSessionCandidateAttendanceStatus",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidateAttendanceStatus_DeletedById",
                schema: "lkp",
                table: "TestSessionCandidateAttendanceStatus",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidateAttendanceStatus_DisplayOrder",
                schema: "lkp",
                table: "TestSessionCandidateAttendanceStatus",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidateAttendanceStatus_IsDeleted",
                schema: "lkp",
                table: "TestSessionCandidateAttendanceStatus",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidateAttendanceStatus_UpdatedById",
                schema: "lkp",
                table: "TestSessionCandidateAttendanceStatus",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidateIdentityVerificationStatus_BackendName",
                schema: "lkp",
                table: "TestSessionCandidateIdentityVerificationStatus",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidateIdentityVerificationStatus_CreatedById",
                schema: "lkp",
                table: "TestSessionCandidateIdentityVerificationStatus",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidateIdentityVerificationStatus_CreatedDate",
                schema: "lkp",
                table: "TestSessionCandidateIdentityVerificationStatus",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidateIdentityVerificationStatus_DeletedById",
                schema: "lkp",
                table: "TestSessionCandidateIdentityVerificationStatus",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidateIdentityVerificationStatus_DisplayOrder",
                schema: "lkp",
                table: "TestSessionCandidateIdentityVerificationStatus",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidateIdentityVerificationStatus_IsDeleted",
                schema: "lkp",
                table: "TestSessionCandidateIdentityVerificationStatus",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidateIdentityVerificationStatus_UpdatedById",
                schema: "lkp",
                table: "TestSessionCandidateIdentityVerificationStatus",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidateStatus_BackendName",
                schema: "lkp",
                table: "TestSessionCandidateStatus",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidateStatus_CreatedById",
                schema: "lkp",
                table: "TestSessionCandidateStatus",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidateStatus_CreatedDate",
                schema: "lkp",
                table: "TestSessionCandidateStatus",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidateStatus_DeletedById",
                schema: "lkp",
                table: "TestSessionCandidateStatus",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidateStatus_DisplayOrder",
                schema: "lkp",
                table: "TestSessionCandidateStatus",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidateStatus_IsDeleted",
                schema: "lkp",
                table: "TestSessionCandidateStatus",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionCandidateStatus_UpdatedById",
                schema: "lkp",
                table: "TestSessionCandidateStatus",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionStatus_BackendName",
                schema: "lkp",
                table: "TestSessionStatus",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionStatus_CreatedById",
                schema: "lkp",
                table: "TestSessionStatus",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionStatus_CreatedDate",
                schema: "lkp",
                table: "TestSessionStatus",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionStatus_DeletedById",
                schema: "lkp",
                table: "TestSessionStatus",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionStatus_DisplayOrder",
                schema: "lkp",
                table: "TestSessionStatus",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionStatus_IsDeleted",
                schema: "lkp",
                table: "TestSessionStatus",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessionStatus_UpdatedById",
                schema: "lkp",
                table: "TestSessionStatus",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlot_CreatedById",
                schema: "hr",
                table: "TestSlot",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlot_CreatedDate",
                schema: "hr",
                table: "TestSlot",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlot_DeletedById",
                schema: "hr",
                table: "TestSlot",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlot_IsDeleted",
                schema: "hr",
                table: "TestSlot",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlot_RoomId",
                schema: "hr",
                table: "TestSlot",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlot_StartedById",
                schema: "hr",
                table: "TestSlot",
                column: "StartedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlot_StatusId",
                schema: "hr",
                table: "TestSlot",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlot_UpdatedById",
                schema: "hr",
                table: "TestSlot",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlotStaff_CreatedById",
                schema: "hr",
                table: "TestSlotStaff",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlotStaff_CreatedDate",
                schema: "hr",
                table: "TestSlotStaff",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlotStaff_DeletedById",
                schema: "hr",
                table: "TestSlotStaff",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlotStaff_IsDeleted",
                schema: "hr",
                table: "TestSlotStaff",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlotStaff_RoleId",
                schema: "hr",
                table: "TestSlotStaff",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlotStaff_StaffUserId",
                schema: "hr",
                table: "TestSlotStaff",
                column: "StaffUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlotStaff_TestSlotId",
                schema: "hr",
                table: "TestSlotStaff",
                column: "TestSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlotStaff_UpdatedById",
                schema: "hr",
                table: "TestSlotStaff",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlotStaffRole_BackendName",
                schema: "lkp",
                table: "TestSlotStaffRole",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestSlotStaffRole_CreatedById",
                schema: "lkp",
                table: "TestSlotStaffRole",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlotStaffRole_CreatedDate",
                schema: "lkp",
                table: "TestSlotStaffRole",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlotStaffRole_DeletedById",
                schema: "lkp",
                table: "TestSlotStaffRole",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlotStaffRole_DisplayOrder",
                schema: "lkp",
                table: "TestSlotStaffRole",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlotStaffRole_IsDeleted",
                schema: "lkp",
                table: "TestSlotStaffRole",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlotStaffRole_UpdatedById",
                schema: "lkp",
                table: "TestSlotStaffRole",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlotStatus_BackendName",
                schema: "lkp",
                table: "TestSlotStatus",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestSlotStatus_CreatedById",
                schema: "lkp",
                table: "TestSlotStatus",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlotStatus_CreatedDate",
                schema: "lkp",
                table: "TestSlotStatus",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlotStatus_DeletedById",
                schema: "lkp",
                table: "TestSlotStatus",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlotStatus_DisplayOrder",
                schema: "lkp",
                table: "TestSlotStatus",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlotStatus_IsDeleted",
                schema: "lkp",
                table: "TestSlotStatus",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TestSlotStatus_UpdatedById",
                schema: "lkp",
                table: "TestSlotStatus",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionBank_QuestionBankVersion_CurrentApprovedVersionId",
                schema: "hr",
                table: "QuestionBank",
                column: "CurrentApprovedVersionId",
                principalSchema: "hr",
                principalTable: "QuestionBankVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionBankAssignment_QuestionBankRequest_QuestionBankRequestId",
                schema: "hr",
                table: "QuestionBankAssignment",
                column: "QuestionBankRequestId",
                principalSchema: "hr",
                principalTable: "QuestionBankRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionBankRequest_QuestionBankVersion_BaseVersionId",
                schema: "hr",
                table: "QuestionBankRequest",
                column: "BaseVersionId",
                principalSchema: "hr",
                principalTable: "QuestionBankVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionBankRequestItem_QuestionRevision_CurrentProposedRevisionId",
                schema: "hr",
                table: "QuestionBankRequestItem",
                column: "CurrentProposedRevisionId",
                principalSchema: "hr",
                principalTable: "QuestionRevision",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionBankRequestItem_QuestionRevision_OriginalRevisionId",
                schema: "hr",
                table: "QuestionBankRequestItem",
                column: "OriginalRevisionId",
                principalSchema: "hr",
                principalTable: "QuestionRevision",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBank_QuestionBankType_QuestionBankTypeId",
                schema: "hr",
                table: "QuestionBank");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBank_QuestionBankVersion_CurrentApprovedVersionId",
                schema: "hr",
                table: "QuestionBank");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBankRequest_QuestionBankVersion_BaseVersionId",
                schema: "hr",
                table: "QuestionBankRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBank_Stage_StageId",
                schema: "hr",
                table: "QuestionBank");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBankAssignment_QuestionBankAssignmentStatus_StatusId",
                schema: "hr",
                table: "QuestionBankAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBankAssignment_QuestionBankRequest_QuestionBankRequestId",
                schema: "hr",
                table: "QuestionBankAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBankRequestItem_QuestionBankRequest_RequestId",
                schema: "hr",
                table: "QuestionBankRequestItem");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBankRequestItem_QuestionBankAssignment_QuestionBankAssignmentId",
                schema: "hr",
                table: "QuestionBankRequestItem");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBankRequestItem_QuestionBankRequestItemStatus_StatusId",
                schema: "hr",
                table: "QuestionBankRequestItem");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBankRequestItem_QuestionChangeType_ChangeTypeId",
                schema: "hr",
                table: "QuestionBankRequestItem");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBankRequestItem_QuestionRevision_CurrentProposedRevisionId",
                schema: "hr",
                table: "QuestionBankRequestItem");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBankRequestItem_QuestionRevision_OriginalRevisionId",
                schema: "hr",
                table: "QuestionBankRequestItem");

            migrationBuilder.DropTable(
                name: "ExamExemptionDecision",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "ExamResultCandidate",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "QuestionBankRequestHistory",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "QuestionBankRequestItemReview",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "QuestionBankRequestReview",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "QuestionBankVersionChangeDetail",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "QuestionBankVersionQuestion",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "QuestionRevisionOption",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "TestAttemptInterruption",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "TestAttemptQuestion",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "TestSlotStaff",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "ExamExemptionDecisionStatus",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "ExamResultCandidateResultStatus",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "ExamResultReport",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "QuestionReviewDecision",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "QuestionBankVersionChange",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "TestAttemptInterruptionResolutionAction",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "TestAttemptInterruptionStatus",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "TestAttemptPart",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "ExamCategory",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "TestSlotStaffRole",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "ExamResultReportStatus",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "TestAttemptPartStatus",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "TestAttempt",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "ExamCategoryType",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "ExamPart",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "TestAttemptStatus",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "TestSessionCandidate",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "TestSessionCandidateAttendanceStatus",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "TestSessionCandidateIdentityVerificationStatus",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "TestSessionCandidateStatus",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "TestSession",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "Exam",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "TestSessionStatus",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "TestSlot",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "ExamInterruptionPolicy",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "ExamStatus",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "Room",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "TestSlotStatus",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "Location",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "RoomStatus",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "RoomType",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "QuestionBankType",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "QuestionBankVersion",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "Stage",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "QuestionBankAssignmentStatus",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "QuestionBankRequest",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "QuestionBankRequestStatus",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "QuestionBankRequestType",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "QuestionBank",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "QuestionBankAssignment",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "QuestionBankRequestItemStatus",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "QuestionChangeType",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "QuestionRevision",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "DifficultyLevel",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "QuestionBankRequestItem",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "QuestionType",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "Question",
                schema: "hr");

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1900294407);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -853440182);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -721093517);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -688729387);

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "CandidateTypeProviderLogin",
                keyColumns: new[] { "CandidateTypeId", "ProviderLoginId" },
                keyValues: new object[] { new Guid("f650aa0c-ab45-422c-9c9e-3b2246409ef2"), new Guid("b8854959-1e46-4595-b51f-de3c09e3ed85") });

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "CandidateTypeProviderLogin",
                keyColumns: new[] { "CandidateTypeId", "ProviderLoginId" },
                keyValues: new object[] { new Guid("f650aa0c-ab45-422c-9c9e-3b2246409ef2"), new Guid("fc379bb9-39f7-458a-85f8-6e54d1780178") });

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("0eb0193c-0ace-ac59-b02d-08e1679a4347"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("6d31ec9d-8e04-005e-b326-7221d0fa2438"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("a86ab789-83ea-2152-891b-382c2e970edc"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("ba0e8dfa-d8a2-5158-b2b9-2272331ea692"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "CandidateType",
                keyColumn: "Id",
                keyValue: new Guid("f650aa0c-ab45-422c-9c9e-3b2246409ef2"));
        }
    }
}
