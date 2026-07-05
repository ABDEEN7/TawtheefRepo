using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQidHolderCandidateType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "lkp",
                table: "CandidateType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[] { new Guid("91f86f10-1799-44e8-bc93-83f0c21f50b4"), "QidHolder", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "مرشح يمتلك الوثيقة الشخصية القطرية", "Candidate with a QID number", 7, true, false, "حامل الوثيقة الشخصية القطرية", "Candidate with a QID number", null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "CandidateType",
                keyColumn: "Id",
                keyValue: new Guid("91f86f10-1799-44e8-bc93-83f0c21f50b4"));
        }
    }
}
