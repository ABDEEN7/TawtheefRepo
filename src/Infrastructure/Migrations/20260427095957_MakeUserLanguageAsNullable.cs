using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeUserLanguageAsNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PreferredLanguage",
                table: "AspNetUsers",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("200c5018-fa8c-4ee7-a088-9077200b125c"),
                column: "PreferredLanguage",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("207bd05b-ebd8-4cea-80ad-38fe2479eac6"),
                column: "PreferredLanguage",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("26058720-5808-435a-abbf-d7a4e751f51e"),
                column: "PreferredLanguage",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("781561c3-0175-4165-80c1-7c6a79130b25"),
                column: "PreferredLanguage",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("9fd109fb-a2ad-4637-86f2-2be13ccfa6d4"),
                column: "PreferredLanguage",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a8e0f354-2e23-41e1-9e1b-a1501b72dc4b"),
                column: "PreferredLanguage",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PreferredLanguage",
                table: "AspNetUsers",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("200c5018-fa8c-4ee7-a088-9077200b125c"),
                column: "PreferredLanguage",
                value: "ar");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("207bd05b-ebd8-4cea-80ad-38fe2479eac6"),
                column: "PreferredLanguage",
                value: "ar");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("26058720-5808-435a-abbf-d7a4e751f51e"),
                column: "PreferredLanguage",
                value: "ar");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("781561c3-0175-4165-80c1-7c6a79130b25"),
                column: "PreferredLanguage",
                value: "ar");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("9fd109fb-a2ad-4637-86f2-2be13ccfa6d4"),
                column: "PreferredLanguage",
                value: "ar");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a8e0f354-2e23-41e1-9e1b-a1501b72dc4b"),
                column: "PreferredLanguage",
                value: "ar");
        }
    }
}
