using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeLookups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "LanguageLevel",
                keyColumn: "Id",
                keyValue: new Guid("f0c9e84a-258f-4f6b-a469-153a92d68730"),
                column: "NameAr",
                value: "مبتدئ");

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "LanguageLevel",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[] { new Guid("5161f23a-f501-414c-b4fe-81cde9ebcd5d"), "Expert", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 4, false, "خبير", "Expert", null, null });

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

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("69c0943a-f04f-4b6c-9145-1fbfae4b5c2e"),
                columns: new[] { "BackendName", "NameAr", "NameEn" },
                values: new object[] { "Basic", "أساسي", "Basic" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("b8f10518-bf02-4357-a0e3-1f6bfb1746cd"),
                columns: new[] { "BackendName", "NameAr", "NameEn" },
                values: new object[] { "Advanced", "متقدم", "Advanced" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "LanguageLevel",
                keyColumn: "Id",
                keyValue: new Guid("5161f23a-f501-414c-b4fe-81cde9ebcd5d"));

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "LanguageLevel",
                keyColumn: "Id",
                keyValue: new Guid("f0c9e84a-258f-4f6b-a469-153a92d68730"),
                column: "NameAr",
                value: "أساسي");

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("2cf3d4e2-33cd-4671-9667-1b5e6e0cee9a"),
                columns: new[] { "BackendName", "NameAr", "NameEn" },
                values: new object[] { "Good", "جيد", "Good" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("4dbd3381-f57a-4f6c-a718-432970d32276"),
                columns: new[] { "BackendName", "NameAr", "NameEn" },
                values: new object[] { "Excellent", "ممتاز", "Excellent" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("69c0943a-f04f-4b6c-9145-1fbfae4b5c2e"),
                columns: new[] { "BackendName", "NameAr", "NameEn" },
                values: new object[] { "Acceptable", "مقبول", "Acceptable" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("b8f10518-bf02-4357-a0e3-1f6bfb1746cd"),
                columns: new[] { "BackendName", "NameAr", "NameEn" },
                values: new object[] { "VeryGood", "جيد جدًا", "Very Good" });
        }
    }
}
