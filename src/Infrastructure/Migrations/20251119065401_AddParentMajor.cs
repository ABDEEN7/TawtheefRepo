using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddParentMajor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                schema: "lkp",
                table: "Major",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Degree",
                keyColumn: "Id",
                keyValue: new Guid("6e453f48-5f2f-4f98-8b76-f416cdd4811b"),
                columns: new[] { "BackendName", "DescriptionEn", "NameEn" },
                values: new object[] { "Primary", "Primary", "Primary" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Degree",
                keyColumn: "Id",
                keyValue: new Guid("f6249ce2-fa02-4e18-9c89-151ba3be0c12"),
                columns: new[] { "BackendName", "DescriptionAr", "DescriptionEn", "NameAr", "NameEn" },
                values: new object[] { "IntermediateDiploma", "دبلوم متوسط", "Intermediate Diploma", "دبلوم متوسط", "Intermediate Diploma" });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Degree",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("1af8c855-975f-4a49-bcf0-343a6d7fd8df"), "PostgraduateDiploma", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "دبلوم دراسات عليا", "Postgraduate Diploma", 0, false, "دبلوم دراسات عليا", "Postgraduate Diploma", null, null },
                    { new Guid("b4e889bb-a35c-46b6-8219-7e0600c71ce1"), "NoQualifications", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "بدون مؤهل", "No Qualifications", 0, false, "بدون مؤهل", "No Qualifications", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Major_ParentId",
                schema: "lkp",
                table: "Major",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Major_Major_ParentId",
                schema: "lkp",
                table: "Major",
                column: "ParentId",
                principalSchema: "lkp",
                principalTable: "Major",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Major_Major_ParentId",
                schema: "lkp",
                table: "Major");

            migrationBuilder.DropIndex(
                name: "IX_Major_ParentId",
                schema: "lkp",
                table: "Major");

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Degree",
                keyColumn: "Id",
                keyValue: new Guid("1af8c855-975f-4a49-bcf0-343a6d7fd8df"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Degree",
                keyColumn: "Id",
                keyValue: new Guid("b4e889bb-a35c-46b6-8219-7e0600c71ce1"));

            migrationBuilder.DropColumn(
                name: "ParentId",
                schema: "lkp",
                table: "Major");

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Degree",
                keyColumn: "Id",
                keyValue: new Guid("6e453f48-5f2f-4f98-8b76-f416cdd4811b"),
                columns: new[] { "BackendName", "DescriptionEn", "NameEn" },
                values: new object[] { "Elementary", "Elementary", "Elementary" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Degree",
                keyColumn: "Id",
                keyValue: new Guid("f6249ce2-fa02-4e18-9c89-151ba3be0c12"),
                columns: new[] { "BackendName", "DescriptionAr", "DescriptionEn", "NameAr", "NameEn" },
                values: new object[] { "Diploma", "دبلوم", "Diploma", "دبلوم", "Diploma" });
        }
    }
}
