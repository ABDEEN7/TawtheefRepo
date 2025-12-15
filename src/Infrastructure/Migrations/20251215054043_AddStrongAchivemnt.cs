using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStrongAchivemnt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpecializationRelation",
                schema: "pro",
                table: "TrainingCourse",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SpecializationRelation",
                schema: "pro",
                table: "Experience",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpecializationRelation",
                schema: "pro",
                table: "TrainingCourse");

            migrationBuilder.DropColumn(
                name: "SpecializationRelation",
                schema: "pro",
                table: "Experience");
        }
    }
}
