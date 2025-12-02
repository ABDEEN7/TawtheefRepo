using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnInExceprince : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Position",
                schema: "pro",
                table: "TrainingCourse",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Organization",
                schema: "pro",
                table: "TrainingCourse",
                newName: "Provider");

            migrationBuilder.RenameColumn(
                name: "Position",
                schema: "pro",
                table: "Experience",
                newName: "JobTitle");

            migrationBuilder.RenameColumn(
                name: "Organization",
                schema: "pro",
                table: "Experience",
                newName: "EmployerName");

            migrationBuilder.RenameColumn(
                name: "Achievements",
                schema: "pro",
                table: "Experience",
                newName: "Description");

            migrationBuilder.AddColumn<Guid>(
                name: "CountryId",
                schema: "pro",
                table: "TrainingCourse",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "pro",
                table: "TrainingCourse",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CountryId",
                schema: "pro",
                table: "Experience",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TrainingCourse_CountryId",
                schema: "pro",
                table: "TrainingCourse",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Experience_CountryId",
                schema: "pro",
                table: "Experience",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Experience_Country_CountryId",
                schema: "pro",
                table: "Experience",
                column: "CountryId",
                principalSchema: "lkp",
                principalTable: "Country",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingCourse_Country_CountryId",
                schema: "pro",
                table: "TrainingCourse",
                column: "CountryId",
                principalSchema: "lkp",
                principalTable: "Country",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Experience_Country_CountryId",
                schema: "pro",
                table: "Experience");

            migrationBuilder.DropForeignKey(
                name: "FK_TrainingCourse_Country_CountryId",
                schema: "pro",
                table: "TrainingCourse");

            migrationBuilder.DropIndex(
                name: "IX_TrainingCourse_CountryId",
                schema: "pro",
                table: "TrainingCourse");

            migrationBuilder.DropIndex(
                name: "IX_Experience_CountryId",
                schema: "pro",
                table: "Experience");

            migrationBuilder.DropColumn(
                name: "CountryId",
                schema: "pro",
                table: "TrainingCourse");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "pro",
                table: "TrainingCourse");

            migrationBuilder.DropColumn(
                name: "CountryId",
                schema: "pro",
                table: "Experience");

            migrationBuilder.RenameColumn(
                name: "Title",
                schema: "pro",
                table: "TrainingCourse",
                newName: "Position");

            migrationBuilder.RenameColumn(
                name: "Provider",
                schema: "pro",
                table: "TrainingCourse",
                newName: "Organization");

            migrationBuilder.RenameColumn(
                name: "JobTitle",
                schema: "pro",
                table: "Experience",
                newName: "Position");

            migrationBuilder.RenameColumn(
                name: "EmployerName",
                schema: "pro",
                table: "Experience",
                newName: "Organization");

            migrationBuilder.RenameColumn(
                name: "Description",
                schema: "pro",
                table: "Experience",
                newName: "Achievements");
        }
    }
}
