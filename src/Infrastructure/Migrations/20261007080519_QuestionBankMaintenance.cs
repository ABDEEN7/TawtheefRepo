using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class QuestionBankMaintenance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ_ResultReport",
                schema: "itv",
                table: "InterviewResultReport");

            migrationBuilder.DropIndex(
                name: "UQ_ResultCandidate",
                schema: "itv",
                table: "InterviewResultCandidate");

            migrationBuilder.CreateSequence<int>(
                name: "InterviewResultReportNumber",
                schema: "itv");

            migrationBuilder.AddColumn<DateTime>(
                name: "CommitteeReviewedAt",
                schema: "itv",
                table: "InterviewResultReport",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CommitteeReviewedById",
                schema: "itv",
                table: "InterviewResultReport",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Number",
                schema: "itv",
                table: "InterviewResultReport",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR [itv].[InterviewResultReportNumber]");

            migrationBuilder.AddColumn<string>(
                name: "ChairRecommendationReason",
                schema: "itv",
                table: "InterviewResultCandidate",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChairRecommendedDecision",
                schema: "itv",
                table: "InterviewResultCandidate",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RecommendedSchoolStageId",
                schema: "itv",
                table: "InterviewResultCandidate",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                schema: "itv",
                table: "InterviewResultReport",
                type: "nvarchar(24)",
                maxLength: 24,
                nullable: false,
                computedColumnSql: "'REP-INT-' + CAST(YEAR([CreatedDate]) AS varchar(4)) + '-' + CASE WHEN [Number] < 10000 THEN RIGHT('0000' + CAST([Number] AS varchar(10)), 4) ELSE CAST([Number] AS varchar(10)) END",
                stored: true);

            migrationBuilder.CreateTable(
                name: "SchoolStage",
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
                    table.PrimaryKey("PK_SchoolStage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchoolStage_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SchoolStage_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SchoolStage_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { -2097377010, "permission", "interview-committee-review.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -1646454079, "permission", "question-bank-requests.maintenance", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -1333939796, "permission", "interview-committee-review.override-suggestion", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -1308318742, "permission", "interview-committee-review.override-suggestion", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -783036742, "permission", "interview-committee-review.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Permission",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsAssignableToRole", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("3e54df06-ed8b-6050-8753-060af580365c"), "interview-committee-review.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 114, true, true, false, "مراجعة نتائج اللجنة - عرض", "Committee Head Review - View", null, null },
                    { new Guid("68381294-9393-df53-8a1a-3e2fc242045e"), "question-bank-requests.maintenance", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 116, true, true, false, "طلبات بنوك الأسئلة - صيانة", "Question Bank Requests - Maintenance", null, null },
                    { new Guid("746ea540-2fce-cc55-a1fa-3f4d75ea984e"), "interview-committee-review.override-suggestion", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 115, true, true, false, "مراجعة نتائج اللجنة - تعديل المقترح", "Committee Head Review - Override Suggestion", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "SchoolStage",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("5c1a7e10-3b2d-4f6a-9c01-7d2e4b8a0001"), "Kindergarten", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "رياض أطفال", "Kindergarten", null, null },
                    { new Guid("5c1a7e10-3b2d-4f6a-9c01-7d2e4b8a0002"), "Primary", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "ابتدائي", "Primary", null, null },
                    { new Guid("5c1a7e10-3b2d-4f6a-9c01-7d2e4b8a0003"), "Preparatory", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "إعدادي", "Preparatory", null, null },
                    { new Guid("5c1a7e10-3b2d-4f6a-9c01-7d2e4b8a0004"), "Secondary", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 4, true, false, "ثانوي", "Secondary", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewResultReport_CommitteeReviewedById",
                schema: "itv",
                table: "InterviewResultReport",
                column: "CommitteeReviewedById");

            migrationBuilder.CreateIndex(
                name: "UQ_ResultReport",
                schema: "itv",
                table: "InterviewResultReport",
                column: "InterviewScheduleId",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewResultCandidate_RecommendedSchoolStageId",
                schema: "itv",
                table: "InterviewResultCandidate",
                column: "RecommendedSchoolStageId");

            migrationBuilder.CreateIndex(
                name: "UQ_ResultCandidate",
                schema: "itv",
                table: "InterviewResultCandidate",
                column: "InterviewAppointmentId",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolStage_BackendName",
                schema: "lkp",
                table: "SchoolStage",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchoolStage_CreatedById",
                schema: "lkp",
                table: "SchoolStage",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolStage_CreatedDate",
                schema: "lkp",
                table: "SchoolStage",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolStage_DeletedById",
                schema: "lkp",
                table: "SchoolStage",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolStage_DisplayOrder",
                schema: "lkp",
                table: "SchoolStage",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolStage_IsDeleted",
                schema: "lkp",
                table: "SchoolStage",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolStage_UpdatedById",
                schema: "lkp",
                table: "SchoolStage",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_InterviewResultCandidate_SchoolStage_RecommendedSchoolStageId",
                schema: "itv",
                table: "InterviewResultCandidate",
                column: "RecommendedSchoolStageId",
                principalSchema: "lkp",
                principalTable: "SchoolStage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterviewResultReport_AspNetUsers_CommitteeReviewedById",
                schema: "itv",
                table: "InterviewResultReport",
                column: "CommitteeReviewedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterviewResultCandidate_SchoolStage_RecommendedSchoolStageId",
                schema: "itv",
                table: "InterviewResultCandidate");

            migrationBuilder.DropForeignKey(
                name: "FK_InterviewResultReport_AspNetUsers_CommitteeReviewedById",
                schema: "itv",
                table: "InterviewResultReport");

            migrationBuilder.DropTable(
                name: "SchoolStage",
                schema: "lkp");

            migrationBuilder.DropIndex(
                name: "IX_InterviewResultReport_CommitteeReviewedById",
                schema: "itv",
                table: "InterviewResultReport");

            migrationBuilder.DropIndex(
                name: "UQ_ResultReport",
                schema: "itv",
                table: "InterviewResultReport");

            migrationBuilder.DropIndex(
                name: "IX_InterviewResultCandidate_RecommendedSchoolStageId",
                schema: "itv",
                table: "InterviewResultCandidate");

            migrationBuilder.DropIndex(
                name: "UQ_ResultCandidate",
                schema: "itv",
                table: "InterviewResultCandidate");

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -2097377010);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1646454079);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1333939796);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1308318742);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -783036742);

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("3e54df06-ed8b-6050-8753-060af580365c"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("68381294-9393-df53-8a1a-3e2fc242045e"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("746ea540-2fce-cc55-a1fa-3f4d75ea984e"));

            migrationBuilder.DropColumn(
                name: "Code",
                schema: "itv",
                table: "InterviewResultReport");

            migrationBuilder.DropColumn(
                name: "CommitteeReviewedAt",
                schema: "itv",
                table: "InterviewResultReport");

            migrationBuilder.DropColumn(
                name: "CommitteeReviewedById",
                schema: "itv",
                table: "InterviewResultReport");

            migrationBuilder.DropColumn(
                name: "Number",
                schema: "itv",
                table: "InterviewResultReport");

            migrationBuilder.DropColumn(
                name: "ChairRecommendationReason",
                schema: "itv",
                table: "InterviewResultCandidate");

            migrationBuilder.DropColumn(
                name: "ChairRecommendedDecision",
                schema: "itv",
                table: "InterviewResultCandidate");

            migrationBuilder.DropColumn(
                name: "RecommendedSchoolStageId",
                schema: "itv",
                table: "InterviewResultCandidate");

            migrationBuilder.DropSequence(
                name: "InterviewResultReportNumber",
                schema: "itv");

            migrationBuilder.CreateIndex(
                name: "UQ_ResultReport",
                schema: "itv",
                table: "InterviewResultReport",
                column: "InterviewScheduleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_ResultCandidate",
                schema: "itv",
                table: "InterviewResultCandidate",
                column: "InterviewAppointmentId",
                unique: true);
        }
    }
}
