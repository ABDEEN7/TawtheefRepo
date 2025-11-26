using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeAllFiledInUserProfileOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_Country_NationalityId",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_MaritalStatus_MaritalStatusId",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_ResidenceAddress_ResidenceAddressId",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResidenceCountryId",
                schema: "app",
                table: "UserProfile",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResidenceAddressId",
                schema: "app",
                table: "UserProfile",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "NationalityId",
                schema: "app",
                table: "UserProfile",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "MaritalStatusId",
                schema: "app",
                table: "UserProfile",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_Country_NationalityId",
                schema: "app",
                table: "UserProfile",
                column: "NationalityId",
                principalSchema: "lkp",
                principalTable: "Country",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_MaritalStatus_MaritalStatusId",
                schema: "app",
                table: "UserProfile",
                column: "MaritalStatusId",
                principalSchema: "lkp",
                principalTable: "MaritalStatus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_ResidenceAddress_ResidenceAddressId",
                schema: "app",
                table: "UserProfile",
                column: "ResidenceAddressId",
                principalSchema: "pro",
                principalTable: "ResidenceAddress",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_Country_NationalityId",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_MaritalStatus_MaritalStatusId",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_ResidenceAddress_ResidenceAddressId",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResidenceCountryId",
                schema: "app",
                table: "UserProfile",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ResidenceAddressId",
                schema: "app",
                table: "UserProfile",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "NationalityId",
                schema: "app",
                table: "UserProfile",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "MaritalStatusId",
                schema: "app",
                table: "UserProfile",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_Country_NationalityId",
                schema: "app",
                table: "UserProfile",
                column: "NationalityId",
                principalSchema: "lkp",
                principalTable: "Country",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_MaritalStatus_MaritalStatusId",
                schema: "app",
                table: "UserProfile",
                column: "MaritalStatusId",
                principalSchema: "lkp",
                principalTable: "MaritalStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_ResidenceAddress_ResidenceAddressId",
                schema: "app",
                table: "UserProfile",
                column: "ResidenceAddressId",
                principalSchema: "pro",
                principalTable: "ResidenceAddress",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
