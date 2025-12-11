using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSkilllevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfileSkill_RatingGrade_LevelId",
                schema: "pro",
                table: "ProfileSkill");

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("69c0943a-f04f-4b6c-9145-1fbfae4b5c2e"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("b8f10518-bf02-4357-a0e3-1f6bfb1746cd"));

            migrationBuilder.CreateTable(
                name: "SkillLevel",
                schema: "lkp",
                columns: table => new
                {
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillLevel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillLevel_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SkillLevel_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SkillLevel_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("2cf3d4e2-33cd-4671-9667-1b5e6e0cee9a"),
                columns: new[] { "BackendName", "NameAr", "NameEn" },
                values: new object[] { "VeryGood", "جيد جدا", "Very Good" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("4dbd3381-f57a-4f6c-a718-432970d32276"),
                columns: new[] { "BackendName", "NameAr", "NameEn" },
                values: new object[] { "AboveExcellent", "امتياز", "Above Excellent" });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "RatingGrade",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("08e6f782-458e-4334-9bf1-f599c53b437a"), "Acceptable", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 5, false, "مقبول", "Acceptable", null, null },
                    { new Guid("71a78988-825a-4a0f-94a3-f59089dbe33d"), "Good", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 4, false, "جيد", "Good", null, null },
                    { new Guid("76bd9c52-9c81-4d8f-afb4-fdd924744082"), "Excellent", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, false, "ممتاز", "Excellent", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "SkillLevel",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("2cf3d4e2-33cd-4671-9667-1b5e6e0cee9a"), "Intermediate", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 3, false, "متوسط", "Intermediate", null, null },
                    { new Guid("4dbd3381-f57a-4f6c-a718-432970d32276"), "Expert", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, false, "خبير", "Expert", null, null },
                    { new Guid("69c0943a-f04f-4b6c-9145-1fbfae4b5c2e"), "Basic", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 4, false, "أساسي", "Basic", null, null },
                    { new Guid("b8f10518-bf02-4357-a0e3-1f6bfb1746cd"), "Advanced", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, false, "متقدم", "Advanced", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SkillLevel_BackendName",
                schema: "lkp",
                table: "SkillLevel",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkillLevel_CreatedById",
                schema: "lkp",
                table: "SkillLevel",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SkillLevel_DeletedById",
                schema: "lkp",
                table: "SkillLevel",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_SkillLevel_DisplayOrder",
                schema: "lkp",
                table: "SkillLevel",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_SkillLevel_UpdatedById",
                schema: "lkp",
                table: "SkillLevel",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileSkill_SkillLevel_LevelId",
                schema: "pro",
                table: "ProfileSkill",
                column: "LevelId",
                principalSchema: "lkp",
                principalTable: "SkillLevel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfileSkill_SkillLevel_LevelId",
                schema: "pro",
                table: "ProfileSkill");

            migrationBuilder.DropTable(
                name: "SkillLevel",
                schema: "lkp");

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("08e6f782-458e-4334-9bf1-f599c53b437a"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("71a78988-825a-4a0f-94a3-f59089dbe33d"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("76bd9c52-9c81-4d8f-afb4-fdd924744082"));

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("2cf3d4e2-33cd-4671-9667-1b5e6e0cee9a"),
                columns: new[] { "BackendName", "NameAr", "NameEn" },
                values: new object[] { "Intermediate", "متوسط", "Intermediate" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("4dbd3381-f57a-4f6c-a718-432970d32276"),
                columns: new[] { "BackendName", "NameAr", "NameEn" },
                values: new object[] { "Expert", "خبير", "Expert" });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "RatingGrade",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("69c0943a-f04f-4b6c-9145-1fbfae4b5c2e"), "Basic", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 4, false, "أساسي", "Basic", null, null },
                    { new Guid("b8f10518-bf02-4357-a0e3-1f6bfb1746cd"), "Advanced", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, false, "متقدم", "Advanced", null, null }
                });

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
    }
}
