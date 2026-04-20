using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexForJobSpecialization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobSpecialization_JobId",
                schema: "hr",
                table: "JobSpecialization");

            migrationBuilder.CreateIndex(
                name: "IX_JobSpecialization_JobId_MajorId",
                schema: "hr",
                table: "JobSpecialization",
                columns: new[] { "JobId", "MajorId" },
                unique: true,
                filter: "[SubMajorId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_JobSpecialization_JobId_MajorId_SubMajorId",
                schema: "hr",
                table: "JobSpecialization",
                columns: new[] { "JobId", "MajorId", "SubMajorId" },
                unique: true,
                filter: "[SubMajorId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobSpecialization_JobId_MajorId",
                schema: "hr",
                table: "JobSpecialization");

            migrationBuilder.DropIndex(
                name: "IX_JobSpecialization_JobId_MajorId_SubMajorId",
                schema: "hr",
                table: "JobSpecialization");

            migrationBuilder.CreateIndex(
                name: "IX_JobSpecialization_JobId",
                schema: "hr",
                table: "JobSpecialization",
                column: "JobId");
        }
    }
}
