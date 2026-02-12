using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class KawaderCandidateTypeFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_CandidateType_CandidateTypeId",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_TargetEntity_TargetEntityId",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.AlterColumn<Guid>(
                name: "TargetEntityId",
                schema: "app",
                table: "UserProfile",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "CandidateTypeId",
                schema: "app",
                table: "UserProfile",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompletedProfile",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUserKawader",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_CandidateType_CandidateTypeId",
                schema: "app",
                table: "UserProfile",
                column: "CandidateTypeId",
                principalSchema: "lkp",
                principalTable: "CandidateType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_TargetEntity_TargetEntityId",
                schema: "app",
                table: "UserProfile",
                column: "TargetEntityId",
                principalSchema: "lkp",
                principalTable: "TargetEntity",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_CandidateType_CandidateTypeId",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_TargetEntity_TargetEntityId",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "IsCompletedProfile",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsUserKawader",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<Guid>(
                name: "TargetEntityId",
                schema: "app",
                table: "UserProfile",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CandidateTypeId",
                schema: "app",
                table: "UserProfile",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_CandidateType_CandidateTypeId",
                schema: "app",
                table: "UserProfile",
                column: "CandidateTypeId",
                principalSchema: "lkp",
                principalTable: "CandidateType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_TargetEntity_TargetEntityId",
                schema: "app",
                table: "UserProfile",
                column: "TargetEntityId",
                principalSchema: "lkp",
                principalTable: "TargetEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
