using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAchivementType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NameEn",
                schema: "lkp",
                table: "AchievementType",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                schema: "lkp",
                table: "AchievementType",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "BackendName",
                schema: "lkp",
                table: "AchievementType",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "AchievementType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("3f5f4d1f-b214-44cb-9b9e-2f9ec3c1f7f1"), "Certificate", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "شهادة", "Certificate", 1, false, "شهادة", "Certificate", null, null },
                    { new Guid("6c3b1b1b-0c9c-4e0e-8d1e-4d6c12e91533"), "Award", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "جائزة", "Award", 2, false, "جائزة", "Award", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AchievementType_BackendName",
                schema: "lkp",
                table: "AchievementType",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AchievementType_DisplayOrder",
                schema: "lkp",
                table: "AchievementType",
                column: "DisplayOrder");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AchievementType_BackendName",
                schema: "lkp",
                table: "AchievementType");

            migrationBuilder.DropIndex(
                name: "IX_AchievementType_DisplayOrder",
                schema: "lkp",
                table: "AchievementType");

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "AchievementType",
                keyColumn: "Id",
                keyValue: new Guid("3f5f4d1f-b214-44cb-9b9e-2f9ec3c1f7f1"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "AchievementType",
                keyColumn: "Id",
                keyValue: new Guid("6c3b1b1b-0c9c-4e0e-8d1e-4d6c12e91533"));

            migrationBuilder.AlterColumn<string>(
                name: "NameEn",
                schema: "lkp",
                table: "AchievementType",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                schema: "lkp",
                table: "AchievementType",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "BackendName",
                schema: "lkp",
                table: "AchievementType",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
