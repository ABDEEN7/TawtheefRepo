using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeGCCUseQatarPass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "lkp",
                table: "CandidateTypeProviderLogin",
                columns: new[] { "CandidateTypeId", "ProviderLoginId" },
                values: new object[] { new Guid("50f14c55-d5f0-4bba-930e-ab73b012e6cb"), new Guid("b8854959-1e46-4595-b51f-de3c09e3ed85") });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "ProviderLogin",
                keyColumn: "Id",
                keyValue: new Guid("e0575116-ea2b-4917-965f-214048a4c78b"),
                columns: new[] { "BackendName", "NameEn" },
                values: new object[] { "AzureAD", "AzureAD" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "SkillType",
                keyColumn: "Id",
                keyValue: new Guid("2f1b6ce7-cbc3-2b5c-b264-a02c6a87be1d"),
                columns: new[] { "DescriptionAr", "NameAr" },
                values: new object[] { "مهارات تعليمية تم الحصول عليها من خلال التعليم الرسمي", "تعليمي" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "SkillType",
                keyColumn: "Id",
                keyValue: new Guid("83b504d0-25c1-5ca0-6757-df299869f002"),
                columns: new[] { "DescriptionAr", "NameAr" },
                values: new object[] { "مهارات مهنية ناعمة وكفاءات مكان العمل", "مهني" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "SkillType",
                keyColumn: "Id",
                keyValue: new Guid("d14ac141-c057-16f5-ddd8-97d02f6a7c9b"),
                columns: new[] { "DescriptionAr", "NameAr" },
                values: new object[] { "مهارات تقنية أو صلبة تتعلق بأدوات أو تقنيات أو منهجيات محددة", "تقني" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "SkillType",
                keyColumn: "Id",
                keyValue: new Guid("f91eb9e6-7a3f-76d6-1fe1-4443cecc5b9a"),
                columns: new[] { "DescriptionAr", "NameAr" },
                values: new object[] { "أنواع أخرى من المهارات غير المصنفة أعلاه", "أخرى" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "CandidateTypeProviderLogin",
                keyColumns: new[] { "CandidateTypeId", "ProviderLoginId" },
                keyValues: new object[] { new Guid("50f14c55-d5f0-4bba-930e-ab73b012e6cb"), new Guid("b8854959-1e46-4595-b51f-de3c09e3ed85") });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "ProviderLogin",
                keyColumn: "Id",
                keyValue: new Guid("e0575116-ea2b-4917-965f-214048a4c78b"),
                columns: new[] { "BackendName", "NameEn" },
                values: new object[] { "Azure", "Azure" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "SkillType",
                keyColumn: "Id",
                keyValue: new Guid("2f1b6ce7-cbc3-2b5c-b264-a02c6a87be1d"),
                columns: new[] { "DescriptionAr", "NameAr" },
                values: new object[] { "ãåÇÑÇÊ ÊÚáíãíÉ Êã ÇáÍÕæá ÚáíåÇ ãä ÎáÇá ÇáÊÚáíã ÇáÑÓãí", "ÊÚáíãí" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "SkillType",
                keyColumn: "Id",
                keyValue: new Guid("83b504d0-25c1-5ca0-6757-df299869f002"),
                columns: new[] { "DescriptionAr", "NameAr" },
                values: new object[] { "ãåÇÑÇÊ ãåäíÉ äÇÚãÉ æßÝÇÁÇÊ ãßÇä ÇáÚãá", "ãåäí" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "SkillType",
                keyColumn: "Id",
                keyValue: new Guid("d14ac141-c057-16f5-ddd8-97d02f6a7c9b"),
                columns: new[] { "DescriptionAr", "NameAr" },
                values: new object[] { "ãåÇÑÇÊ ÊÞäíÉ Ãæ ÕáÈÉ ÊÊÚáÞ ÈÃÏæÇÊ Ãæ ÊÞäíÇÊ Ãæ ãäåÌíÇÊ ãÍÏÏÉ", "ÊÞäí" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "SkillType",
                keyColumn: "Id",
                keyValue: new Guid("f91eb9e6-7a3f-76d6-1fe1-4443cecc5b9a"),
                columns: new[] { "DescriptionAr", "NameAr" },
                values: new object[] { "ÃäæÇÚ ÃÎÑì ãä ÇáãåÇÑÇÊ ÛíÑ ÇáãÕäÝÉ ÃÚáÇå", "ÃÎÑì" });
        }
    }
}
