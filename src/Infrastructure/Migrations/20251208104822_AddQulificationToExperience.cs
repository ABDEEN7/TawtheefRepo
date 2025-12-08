using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQulificationToExperience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "QualificationId",
                schema: "pro",
                table: "Experience",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RelatedToSpecialization",
                schema: "pro",
                table: "Achievement",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Experience_QualificationId",
                schema: "pro",
                table: "Experience",
                column: "QualificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Experience_Qualification_QualificationId",
                schema: "pro",
                table: "Experience",
                column: "QualificationId",
                principalSchema: "pro",
                principalTable: "Qualification",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Experience_Qualification_QualificationId",
                schema: "pro",
                table: "Experience");

            migrationBuilder.DropIndex(
                name: "IX_Experience_QualificationId",
                schema: "pro",
                table: "Experience");

            migrationBuilder.DropColumn(
                name: "QualificationId",
                schema: "pro",
                table: "Experience");

            migrationBuilder.DropColumn(
                name: "RelatedToSpecialization",
                schema: "pro",
                table: "Achievement");
        }
    }
}
