using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJobCandidateFilterSpecialization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobSpecialization_JobId_MajorId",
                schema: "hr",
                table: "JobSpecialization");

            migrationBuilder.DropIndex(
                name: "IX_JobSpecialization_JobId_MajorId_SubMajorId",
                schema: "hr",
                table: "JobSpecialization");

            migrationBuilder.DropIndex(
                name: "IX_JobSpecialization_SubMajorId",
                schema: "hr",
                table: "JobSpecialization");
            
            migrationBuilder.AlterColumn<Guid>(
                name: "SubMajorId",
                schema: "hr",
                table: "JobSpecialization",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
            
            migrationBuilder.CreateIndex(
                name: "IX_JobSpecialization_JobId_MajorId",
                schema: "hr",
                table: "JobSpecialization",
                columns: new[] { "JobId", "MajorId" },
                unique: true,
                filter: "[SubMajorId] = '00000000-0000-0000-0000-000000000000' AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_JobSpecialization_JobId_MajorId_SubMajorId",
                schema: "hr",
                table: "JobSpecialization",
                columns: new[] { "JobId", "MajorId", "SubMajorId" },
                unique: true,
                filter: "[SubMajorId] <> '00000000-0000-0000-0000-000000000000' AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_JobSpecialization_SubMajorId",
                schema: "hr",
                table: "JobSpecialization",
                column: "SubMajorId");
            
            migrationBuilder.CreateTable(
                name: "JobCandidateFilterSpecialization",
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
                    JobCandidateFilterSettingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MajorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubMajorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobCandidateFilterSpecialization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobCandidateFilterSpecialization_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobCandidateFilterSpecialization_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobCandidateFilterSpecialization_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobCandidateFilterSpecialization_JobCandidateFilterSetting_JobCandidateFilterSettingId",
                        column: x => x.JobCandidateFilterSettingId,
                        principalSchema: "hr",
                        principalTable: "JobCandidateFilterSetting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobCandidateFilterSpecialization_Major_MajorId",
                        column: x => x.MajorId,
                        principalSchema: "lkp",
                        principalTable: "Major",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobCandidateFilterSpecialization_Major_SubMajorId",
                        column: x => x.SubMajorId,
                        principalSchema: "lkp",
                        principalTable: "Major",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateFilterSpecialization_CreatedById",
                schema: "hr",
                table: "JobCandidateFilterSpecialization",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateFilterSpecialization_CreatedDate",
                schema: "hr",
                table: "JobCandidateFilterSpecialization",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateFilterSpecialization_DeletedById",
                schema: "hr",
                table: "JobCandidateFilterSpecialization",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateFilterSpecialization_IsDeleted",
                schema: "hr",
                table: "JobCandidateFilterSpecialization",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateFilterSpecialization_JobCandidateFilterSettingId",
                schema: "hr",
                table: "JobCandidateFilterSpecialization",
                column: "JobCandidateFilterSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateFilterSpecialization_MajorId",
                schema: "hr",
                table: "JobCandidateFilterSpecialization",
                column: "MajorId");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateFilterSpecialization_SubMajorId",
                schema: "hr",
                table: "JobCandidateFilterSpecialization",
                column: "SubMajorId");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateFilterSpecialization_UpdatedById",
                schema: "hr",
                table: "JobCandidateFilterSpecialization",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobCandidateFilterSpecialization",
                schema: "hr");

            migrationBuilder.AlterColumn<Guid>(
                name: "SubMajorId",
                schema: "hr",
                table: "JobSpecialization",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }
    }
}
