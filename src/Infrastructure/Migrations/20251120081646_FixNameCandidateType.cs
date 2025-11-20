using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixNameCandidateType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "CandidateType",
                keyColumn: "Id",
                keyValue: new Guid("42b374d1-8032-44d7-95bd-ff5a64abbc95"),
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "مقيم خارج قطر", "Resident outside Qatar" });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "CandidateType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("744256ea-4ee0-4a63-b071-8810895a33dc"), "SonOfQatariMother", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 5, false, "أبناء المرأة القطرية المتزوجة من غير قطري", "Children of a Qatari woman married to a non-Qatari", null, null },
                    { new Guid("ce67465e-6fed-4a83-9161-8eba16a79af3"), "WifeOfQatari", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 6, false, "الزوج غير القطري المتزوج من قطرية أو قطري", "Non-Qatari husband married to a Qatari woman or man", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "CandidateType",
                keyColumn: "Id",
                keyValue: new Guid("744256ea-4ee0-4a63-b071-8810895a33dc"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "CandidateType",
                keyColumn: "Id",
                keyValue: new Guid("ce67465e-6fed-4a83-9161-8eba16a79af3"));

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "CandidateType",
                keyColumn: "Id",
                keyValue: new Guid("42b374d1-8032-44d7-95bd-ff5a64abbc95"),
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "غير قطري", "Non-Qatari" });
        }
    }
}
