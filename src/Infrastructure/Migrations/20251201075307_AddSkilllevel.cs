using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSkilllevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LevelId",
                schema: "pro",
                table: "ProfileSkill",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSkill_LevelId",
                schema: "pro",
                table: "ProfileSkill",
                column: "LevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileSkill_RatingGrade_LevelId",
                schema: "pro",
                table: "ProfileSkill",
                column: "LevelId",
                principalSchema: "lkp",
                principalTable: "RatingGrade",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfileSkill_RatingGrade_LevelId",
                schema: "pro",
                table: "ProfileSkill");

            migrationBuilder.DropIndex(
                name: "IX_ProfileSkill_LevelId",
                schema: "pro",
                table: "ProfileSkill");

            migrationBuilder.DropColumn(
                name: "LevelId",
                schema: "pro",
                table: "ProfileSkill");
        }
    }
}
