using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddActiveSessionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ActiveSessionId",
                schema: "hr",
                table: "TestAttempt",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestAttempt_ActiveSessionId",
                schema: "hr",
                table: "TestAttempt",
                column: "ActiveSessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TestAttempt_ActiveSessionId",
                schema: "hr",
                table: "TestAttempt");

            migrationBuilder.DropColumn(
                name: "ActiveSessionId",
                schema: "hr",
                table: "TestAttempt");
        }
    }
}
