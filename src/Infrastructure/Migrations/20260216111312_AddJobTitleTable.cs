using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJobTitleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Job_Unique_Title_Department_Category_SubMajor",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "TitleAr",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "TitleEn",
                schema: "hr",
                table: "Job");

            migrationBuilder.AddColumn<Guid>(
                name: "JobTitleId",
                schema: "hr",
                table: "Job",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "JobTitle",
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
                    JobNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    JobNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    JobNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTitle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobTitle_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobTitle_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobTitle_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Job_Unique_JobTitle_Department_Category_SubMajor",
                schema: "hr",
                table: "Job",
                columns: new[] { "JobTitleId", "DepartmentId", "JobCategoryId", "SubMajorId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_JobTitle_CreatedById",
                schema: "lkp",
                table: "JobTitle",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobTitle_DeletedById",
                schema: "lkp",
                table: "JobTitle",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobTitle_JobNumber",
                schema: "lkp",
                table: "JobTitle",
                column: "JobNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobTitle_UpdatedById",
                schema: "lkp",
                table: "JobTitle",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Job_JobTitle_JobTitleId",
                schema: "hr",
                table: "Job",
                column: "JobTitleId",
                principalSchema: "lkp",
                principalTable: "JobTitle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Job_JobTitle_JobTitleId",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropTable(
                name: "JobTitle",
                schema: "lkp");

            migrationBuilder.DropIndex(
                name: "IX_Job_Unique_JobTitle_Department_Category_SubMajor",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "JobTitleId",
                schema: "hr",
                table: "Job");

            migrationBuilder.AddColumn<string>(
                name: "TitleAr",
                schema: "hr",
                table: "Job",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TitleEn",
                schema: "hr",
                table: "Job",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Unique_Title_Department_Category_SubMajor",
                schema: "hr",
                table: "Job",
                columns: new[] { "TitleAr", "DepartmentId", "JobCategoryId", "SubMajorId" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }
    }
}
