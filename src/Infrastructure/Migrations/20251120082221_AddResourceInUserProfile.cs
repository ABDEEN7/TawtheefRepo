using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddResourceInUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BirthdayCertificateId",
                schema: "app",
                table: "UserProfile",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MarriageCertificateId",
                schema: "app",
                table: "UserProfile",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_BirthdayCertificateId",
                schema: "app",
                table: "UserProfile",
                column: "BirthdayCertificateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_MarriageCertificateId",
                schema: "app",
                table: "UserProfile",
                column: "MarriageCertificateId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_Resources_BirthdayCertificateId",
                schema: "app",
                table: "UserProfile",
                column: "BirthdayCertificateId",
                principalTable: "Resources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_Resources_MarriageCertificateId",
                schema: "app",
                table: "UserProfile",
                column: "MarriageCertificateId",
                principalTable: "Resources",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_Resources_BirthdayCertificateId",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_Resources_MarriageCertificateId",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_BirthdayCertificateId",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_MarriageCertificateId",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "BirthdayCertificateId",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "MarriageCertificateId",
                schema: "app",
                table: "UserProfile");
        }
    }
}
