using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsRequiredFromMajorSkill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("e0cd7b22-8948-0c37-9b15-2e5217f0c565"));

            migrationBuilder.DropColumn(
                name: "IsSkillRequired",
                schema: "hr",
                table: "MajorSkill");

            migrationBuilder.AddColumn<bool>(
                name: "IsRequired",
                schema: "hr",
                table: "JobSkill",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRequired",
                schema: "hr",
                table: "JobSkill");

            migrationBuilder.AddColumn<bool>(
                name: "IsSkillRequired",
                schema: "hr",
                table: "MajorSkill",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "JobStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[] { new Guid("e0cd7b22-8948-0c37-9b15-2e5217f0c565"), "Active", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "الوظيفة جاهزة للتقديم ويمكن إرسالها للجمهور المطلوب.", "Job is open for applications and can be published to the target audience.", 2, true, false, "نشطة", "Active", null, null });
        }
    }
}
