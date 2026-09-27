using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCandidateExamConnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "CandidateExamConnection",
                schema: "hr",
                columns: table => new
                {
                    ConnectionId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    TestAttemptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastSeenAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateExamConnection", x => x.ConnectionId);
                    table.ForeignKey(
                        name: "FK_CandidateExamConnection_TestAttempt_TestAttemptId",
                        column: x => x.TestAttemptId,
                        principalSchema: "hr",
                        principalTable: "TestAttempt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CandidateExamConnection_TestAttemptId_ExpiresAt",
                schema: "hr",
                table: "CandidateExamConnection",
                columns: new[] { "TestAttemptId", "ExpiresAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidateExamConnection",
                schema: "hr");
        }
    }
}
