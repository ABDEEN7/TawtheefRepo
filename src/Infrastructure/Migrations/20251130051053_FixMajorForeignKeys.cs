using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixMajorForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Major_Major_ParentId",
                schema: "lkp",
                table: "Major");

            migrationBuilder.DropForeignKey(
                name: "FK_Qualification_Major_MajorId",
                schema: "pro",
                table: "Qualification");

            migrationBuilder.DropForeignKey(
                name: "FK_Qualification_Major_SubMajorId",
                schema: "pro",
                table: "Qualification");

            migrationBuilder.AddForeignKey(
                name: "FK_Major_Major_ParentId",
                schema: "lkp",
                table: "Major",
                column: "ParentId",
                principalSchema: "lkp",
                principalTable: "Major",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Qualification_Major_MajorId",
                schema: "pro",
                table: "Qualification",
                column: "MajorId",
                principalSchema: "lkp",
                principalTable: "Major",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Qualification_Major_SubMajorId",
                schema: "pro",
                table: "Qualification",
                column: "SubMajorId",
                principalSchema: "lkp",
                principalTable: "Major",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Major_Major_ParentId",
                schema: "lkp",
                table: "Major");

            migrationBuilder.DropForeignKey(
                name: "FK_Qualification_Major_MajorId",
                schema: "pro",
                table: "Qualification");

            migrationBuilder.DropForeignKey(
                name: "FK_Qualification_Major_SubMajorId",
                schema: "pro",
                table: "Qualification");

            migrationBuilder.AddForeignKey(
                name: "FK_Major_Major_ParentId",
                schema: "lkp",
                table: "Major",
                column: "ParentId",
                principalSchema: "lkp",
                principalTable: "Major",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Qualification_Major_MajorId",
                schema: "pro",
                table: "Qualification",
                column: "MajorId",
                principalSchema: "lkp",
                principalTable: "Major",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Qualification_Major_SubMajorId",
                schema: "pro",
                table: "Qualification",
                column: "SubMajorId",
                principalSchema: "lkp",
                principalTable: "Major",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
