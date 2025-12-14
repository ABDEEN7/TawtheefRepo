using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCertificateColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_Resources_ResidenceAddressCertificateId",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_ResidenceAddressCertificateId",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "ResidenceAddressCertificateId",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.AddColumn<Guid>(
                name: "CertificateId",
                schema: "pro",
                table: "ResidenceAddress",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "NameEn",
                table: "AspNetRoles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                table: "AspNetRoles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionEn",
                table: "AspNetRoles",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionAr",
                table: "AspNetRoles",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResidenceAddress_CertificateId",
                schema: "pro",
                table: "ResidenceAddress",
                column: "CertificateId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResidenceAddress_Resources_CertificateId",
                schema: "pro",
                table: "ResidenceAddress",
                column: "CertificateId",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResidenceAddress_Resources_CertificateId",
                schema: "pro",
                table: "ResidenceAddress");

            migrationBuilder.DropIndex(
                name: "IX_ResidenceAddress_CertificateId",
                schema: "pro",
                table: "ResidenceAddress");

            migrationBuilder.DropColumn(
                name: "CertificateId",
                schema: "pro",
                table: "ResidenceAddress");

            migrationBuilder.AddColumn<Guid>(
                name: "ResidenceAddressCertificateId",
                schema: "app",
                table: "UserProfile",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameEn",
                table: "AspNetRoles",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                table: "AspNetRoles",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionEn",
                table: "AspNetRoles",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionAr",
                table: "AspNetRoles",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_ResidenceAddressCertificateId",
                schema: "app",
                table: "UserProfile",
                column: "ResidenceAddressCertificateId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_Resources_ResidenceAddressCertificateId",
                schema: "app",
                table: "UserProfile",
                column: "ResidenceAddressCertificateId",
                principalTable: "Resources",
                principalColumn: "Id");
        }
    }
}
