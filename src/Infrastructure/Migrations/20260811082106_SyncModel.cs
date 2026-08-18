using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "CandidateType",
                keyColumn: "Id",
                keyValue: new Guid("91f86f10-1799-44e8-bc93-83f0c21f50b4"),
                columns: new[] { "DescriptionEn", "NameAr", "NameEn" },
                values: new object[] { "Holders of Qatari Documents", "حملة الوثائق القطرية", "Holders of Qatari Documents" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "CandidateType",
                keyColumn: "Id",
                keyValue: new Guid("91f86f10-1799-44e8-bc93-83f0c21f50b4"),
                columns: new[] { "DescriptionEn", "NameAr", "NameEn" },
                values: new object[] { "Candidate with a QID number", "حامل الوثيقة الشخصية القطرية", "Candidate with a QID number" });
        }
    }
}
