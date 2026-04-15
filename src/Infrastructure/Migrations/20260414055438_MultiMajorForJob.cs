using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MultiMajorForJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobSpecialization",
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
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MajorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubMajorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSpecialization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobSpecialization_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSpecialization_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSpecialization_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSpecialization_Job_JobId",
                        column: x => x.JobId,
                        principalSchema: "hr",
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSpecialization_Major_MajorId",
                        column: x => x.MajorId,
                        principalSchema: "lkp",
                        principalTable: "Major",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSpecialization_Major_SubMajorId",
                        column: x => x.SubMajorId,
                        principalSchema: "lkp",
                        principalTable: "Major",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobSpecialization_CreatedById",
                schema: "hr",
                table: "JobSpecialization",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobSpecialization_CreatedDate",
                schema: "hr",
                table: "JobSpecialization",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_JobSpecialization_DeletedById",
                schema: "hr",
                table: "JobSpecialization",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobSpecialization_IsDeleted",
                schema: "hr",
                table: "JobSpecialization",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_JobSpecialization_JobId",
                schema: "hr",
                table: "JobSpecialization",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSpecialization_MajorId",
                schema: "hr",
                table: "JobSpecialization",
                column: "MajorId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSpecialization_SubMajorId",
                schema: "hr",
                table: "JobSpecialization",
                column: "SubMajorId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSpecialization_UpdatedById",
                schema: "hr",
                table: "JobSpecialization",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobSpecialization",
                schema: "hr");
        }
    }
}
