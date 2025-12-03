using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQIDExpiryDateField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "CandidateTypeProviderLogin",
                keyColumns: new[] { "CandidateTypeId", "ProviderLoginId" },
                keyValues: new object[] { new Guid("268d49f6-0dda-4dd6-8346-be355c553496"), new Guid("b8854959-1e46-4595-b51f-de3c09e3ed85") });

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Degree",
                keyColumn: "Id",
                keyValue: new Guid("b4e889bb-a35c-46b6-8219-7e0600c71ce1"));

            migrationBuilder.AddColumn<DateOnly>(
                name: "QIDExpiry",
                schema: "app",
                table: "UserProfile",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "QIDExpiry",
                schema: "app",
                table: "SponsorProfile",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "CandidateType",
                keyColumn: "Id",
                keyValue: new Guid("268d49f6-0dda-4dd6-8346-be355c553496"),
                columns: new[] { "DescriptionAr", "DescriptionEn" },
                values: new object[] { "قطري الجنسية", "Qatari National" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "CandidateType",
                keyColumn: "Id",
                keyValue: new Guid("42b374d1-8032-44d7-95bd-ff5a64abbc95"),
                columns: new[] { "DescriptionAr", "DescriptionEn" },
                values: new object[] { "مقيم خارج دولة قطر", "Resident outside Qatar" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "CandidateType",
                keyColumn: "Id",
                keyValue: new Guid("50f14c55-d5f0-4bba-930e-ab73b012e6cb"),
                columns: new[] { "DescriptionAr", "DescriptionEn", "NameAr", "NameEn" },
                values: new object[] { "مواطن دول مجلس التعاون الخليجي", "Citizen of a GCC country", "مواطن دول مجلس التعاون الخليجي", "Citizen of a GCC country" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "CandidateType",
                keyColumn: "Id",
                keyValue: new Guid("744256ea-4ee0-4a63-b071-8810895a33dc"),
                columns: new[] { "DescriptionAr", "DescriptionEn" },
                values: new object[] { "أبناء المرأة القطرية المتزوجة من غير قطري", "Children of a Qatari woman married to a non-Qatari" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "CandidateType",
                keyColumn: "Id",
                keyValue: new Guid("7b06bc91-88b3-46ec-b6cc-ef2338864d41"),
                columns: new[] { "DescriptionAr", "DescriptionEn" },
                values: new object[] { "مقيم داخل دولة قطر", "Resident in Qatar" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "CandidateType",
                keyColumn: "Id",
                keyValue: new Guid("ce67465e-6fed-4a83-9161-8eba16a79af3"),
                columns: new[] { "DescriptionAr", "DescriptionEn", "NameAr", "NameEn" },
                values: new object[] { "الزوج أو الزوجة غير قطري/ة المتزوج/ة من قطري/ة", "Non-Qatari spouse married to a Qatari", "الزوج أو الزوجة غير قطري/ة المتزوج/ة من قطري/ة", "Non-Qatari spouse married to a Qatari" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QIDExpiry",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "QIDExpiry",
                schema: "app",
                table: "SponsorProfile");

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "CandidateType",
                keyColumn: "Id",
                keyValue: new Guid("268d49f6-0dda-4dd6-8346-be355c553496"),
                columns: new[] { "DescriptionAr", "DescriptionEn" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "CandidateType",
                keyColumn: "Id",
                keyValue: new Guid("42b374d1-8032-44d7-95bd-ff5a64abbc95"),
                columns: new[] { "DescriptionAr", "DescriptionEn" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "CandidateType",
                keyColumn: "Id",
                keyValue: new Guid("50f14c55-d5f0-4bba-930e-ab73b012e6cb"),
                columns: new[] { "DescriptionAr", "DescriptionEn", "NameAr", "NameEn" },
                values: new object[] { null, null, "مجلس تعاون الخليج", "GCC National" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "CandidateType",
                keyColumn: "Id",
                keyValue: new Guid("744256ea-4ee0-4a63-b071-8810895a33dc"),
                columns: new[] { "DescriptionAr", "DescriptionEn" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "CandidateType",
                keyColumn: "Id",
                keyValue: new Guid("7b06bc91-88b3-46ec-b6cc-ef2338864d41"),
                columns: new[] { "DescriptionAr", "DescriptionEn" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "CandidateType",
                keyColumn: "Id",
                keyValue: new Guid("ce67465e-6fed-4a83-9161-8eba16a79af3"),
                columns: new[] { "DescriptionAr", "DescriptionEn", "NameAr", "NameEn" },
                values: new object[] { null, null, "الزوج غير القطري المتزوج من قطرية أو قطري", "Non-Qatari husband married to a Qatari woman or man" });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "CandidateTypeProviderLogin",
                columns: new[] { "CandidateTypeId", "ProviderLoginId" },
                values: new object[] { new Guid("268d49f6-0dda-4dd6-8346-be355c553496"), new Guid("b8854959-1e46-4595-b51f-de3c09e3ed85") });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Degree",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[] { new Guid("b4e889bb-a35c-46b6-8219-7e0600c71ce1"), "NoQualifications", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "بدون مؤهل", "No Qualifications", 0, false, "بدون مؤهل", "No Qualifications", null, null });
        }
    }
}
