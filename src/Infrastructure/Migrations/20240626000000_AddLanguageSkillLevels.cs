using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLanguageSkillLevels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReadingLevelId",
                schema: "pro",
                table: "ProfileLanguage",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SpeakingLevelId",
                schema: "pro",
                table: "ProfileLanguage",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WritingLevelId",
                schema: "pro",
                table: "ProfileLanguage",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.Sql("UPDATE pro.ProfileLanguage SET ReadingLevelId = LevelId, WritingLevelId = LevelId, SpeakingLevelId = LevelId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReadingLevelId",
                schema: "pro",
                table: "ProfileLanguage",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SpeakingLevelId",
                schema: "pro",
                table: "ProfileLanguage",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "WritingLevelId",
                schema: "pro",
                table: "ProfileLanguage",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileLanguage_ReadingLevelId",
                schema: "pro",
                table: "ProfileLanguage",
                column: "ReadingLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileLanguage_SpeakingLevelId",
                schema: "pro",
                table: "ProfileLanguage",
                column: "SpeakingLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileLanguage_WritingLevelId",
                schema: "pro",
                table: "ProfileLanguage",
                column: "WritingLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileLanguage_LanguageLevel_ReadingLevelId",
                schema: "pro",
                table: "ProfileLanguage",
                column: "ReadingLevelId",
                principalSchema: "lkp",
                principalTable: "LanguageLevel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileLanguage_LanguageLevel_SpeakingLevelId",
                schema: "pro",
                table: "ProfileLanguage",
                column: "SpeakingLevelId",
                principalSchema: "lkp",
                principalTable: "LanguageLevel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileLanguage_LanguageLevel_WritingLevelId",
                schema: "pro",
                table: "ProfileLanguage",
                column: "WritingLevelId",
                principalSchema: "lkp",
                principalTable: "LanguageLevel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfileLanguage_LanguageLevel_ReadingLevelId",
                schema: "pro",
                table: "ProfileLanguage");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfileLanguage_LanguageLevel_SpeakingLevelId",
                schema: "pro",
                table: "ProfileLanguage");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfileLanguage_LanguageLevel_WritingLevelId",
                schema: "pro",
                table: "ProfileLanguage");

            migrationBuilder.DropIndex(
                name: "IX_ProfileLanguage_ReadingLevelId",
                schema: "pro",
                table: "ProfileLanguage");

            migrationBuilder.DropIndex(
                name: "IX_ProfileLanguage_SpeakingLevelId",
                schema: "pro",
                table: "ProfileLanguage");

            migrationBuilder.DropIndex(
                name: "IX_ProfileLanguage_WritingLevelId",
                schema: "pro",
                table: "ProfileLanguage");

            migrationBuilder.DropColumn(
                name: "ReadingLevelId",
                schema: "pro",
                table: "ProfileLanguage");

            migrationBuilder.DropColumn(
                name: "SpeakingLevelId",
                schema: "pro",
                table: "ProfileLanguage");

            migrationBuilder.DropColumn(
                name: "WritingLevelId",
                schema: "pro",
                table: "ProfileLanguage");
        }
    }
}
