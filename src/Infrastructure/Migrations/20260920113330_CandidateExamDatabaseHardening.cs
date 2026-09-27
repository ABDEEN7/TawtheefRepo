using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations;

/// <inheritdoc />
public partial class CandidateExamDatabaseHardening : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            IF EXISTS (SELECT 1 FROM [hr].[TestAttempt] GROUP BY [TestSessionCandidateId] HAVING COUNT(*) > 1)
                THROW 51000, 'BE-01 blocked: duplicate TestAttempt.TestSessionCandidateId values exist.', 1;

            IF EXISTS (SELECT 1 FROM [hr].[TestAttemptPart] GROUP BY [TestAttemptId], [ExamPartId] HAVING COUNT(*) > 1)
                THROW 51000, 'BE-01 blocked: duplicate TestAttemptPart attempt/part values exist.', 1;

            IF EXISTS (SELECT 1 FROM [hr].[TestAttemptQuestion])
                THROW 51000, 'BE-01 blocked: existing TestAttemptQuestion rows require explicit part and revision backfill.', 1;

            IF EXISTS (SELECT 1 FROM [hr].[TestSessionCandidate] GROUP BY [TestSessionId], [InvitationId] HAVING COUNT(*) > 1)
                THROW 51000, 'BE-01 blocked: duplicate TestSessionCandidate session/invitation values exist.', 1;
            """);

        migrationBuilder.DropIndex(name: "IX_TestSessionCandidate_TestSessionId", schema: "hr", table: "TestSessionCandidate");
        migrationBuilder.DropIndex(name: "IX_TestAttemptQuestion_TestAttemptId", schema: "hr", table: "TestAttemptQuestion");
        migrationBuilder.DropIndex(name: "IX_TestAttemptPart_TestAttemptId", schema: "hr", table: "TestAttemptPart");
        migrationBuilder.DropIndex(name: "IX_TestAttempt_TestSessionCandidateId", schema: "hr", table: "TestAttempt");

        migrationBuilder.AddColumn<Guid>(name: "QuestionRevisionId", schema: "hr", table: "TestAttemptQuestion", type: "uniqueidentifier", nullable: false);
        migrationBuilder.AddColumn<Guid>(name: "TestAttemptPartId", schema: "hr", table: "TestAttemptQuestion", type: "uniqueidentifier", nullable: false);
        migrationBuilder.AddColumn<int>(name: "DurationMinutesSnapshot", schema: "hr", table: "TestAttemptPart", type: "int", nullable: true);
        migrationBuilder.AddColumn<DateTime>(name: "EffectiveDeadlineAt", schema: "hr", table: "TestAttemptPart", type: "datetime2", nullable: true);

        migrationBuilder.CreateIndex(name: "IX_TestSessionCandidate_TestSessionId_InvitationId", schema: "hr", table: "TestSessionCandidate", columns: new[] { "TestSessionId", "InvitationId" }, unique: true);
        migrationBuilder.CreateIndex(name: "IX_TestAttemptQuestion_QuestionRevisionId", schema: "hr", table: "TestAttemptQuestion", column: "QuestionRevisionId");
        migrationBuilder.CreateIndex(name: "IX_TestAttemptQuestion_TestAttemptId_QuestionId", schema: "hr", table: "TestAttemptQuestion", columns: new[] { "TestAttemptId", "QuestionId" }, unique: true);
        migrationBuilder.CreateIndex(name: "IX_TestAttemptQuestion_TestAttemptPartId_OrderNo", schema: "hr", table: "TestAttemptQuestion", columns: new[] { "TestAttemptPartId", "OrderNo" }, unique: true);
        migrationBuilder.CreateIndex(name: "IX_TestAttemptPart_TestAttemptId_ExamPartId", schema: "hr", table: "TestAttemptPart", columns: new[] { "TestAttemptId", "ExamPartId" }, unique: true);
        migrationBuilder.CreateIndex(name: "IX_TestAttempt_TestSessionCandidateId", schema: "hr", table: "TestAttempt", column: "TestSessionCandidateId", unique: true);

        migrationBuilder.AddForeignKey(name: "FK_TestAttemptQuestion_QuestionRevision_QuestionRevisionId", schema: "hr", table: "TestAttemptQuestion", column: "QuestionRevisionId", principalSchema: "hr", principalTable: "QuestionRevision", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
        migrationBuilder.AddForeignKey(name: "FK_TestAttemptQuestion_TestAttemptPart_TestAttemptPartId", schema: "hr", table: "TestAttemptQuestion", column: "TestAttemptPartId", principalSchema: "hr", principalTable: "TestAttemptPart", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(name: "FK_TestAttemptQuestion_QuestionRevision_QuestionRevisionId", schema: "hr", table: "TestAttemptQuestion");
        migrationBuilder.DropForeignKey(name: "FK_TestAttemptQuestion_TestAttemptPart_TestAttemptPartId", schema: "hr", table: "TestAttemptQuestion");

        migrationBuilder.DropIndex(name: "IX_TestSessionCandidate_TestSessionId_InvitationId", schema: "hr", table: "TestSessionCandidate");
        migrationBuilder.DropIndex(name: "IX_TestAttemptQuestion_QuestionRevisionId", schema: "hr", table: "TestAttemptQuestion");
        migrationBuilder.DropIndex(name: "IX_TestAttemptQuestion_TestAttemptId_QuestionId", schema: "hr", table: "TestAttemptQuestion");
        migrationBuilder.DropIndex(name: "IX_TestAttemptQuestion_TestAttemptPartId_OrderNo", schema: "hr", table: "TestAttemptQuestion");
        migrationBuilder.DropIndex(name: "IX_TestAttemptPart_TestAttemptId_ExamPartId", schema: "hr", table: "TestAttemptPart");
        migrationBuilder.DropIndex(name: "IX_TestAttempt_TestSessionCandidateId", schema: "hr", table: "TestAttempt");

        migrationBuilder.DropColumn(name: "QuestionRevisionId", schema: "hr", table: "TestAttemptQuestion");
        migrationBuilder.DropColumn(name: "TestAttemptPartId", schema: "hr", table: "TestAttemptQuestion");
        migrationBuilder.DropColumn(name: "DurationMinutesSnapshot", schema: "hr", table: "TestAttemptPart");
        migrationBuilder.DropColumn(name: "EffectiveDeadlineAt", schema: "hr", table: "TestAttemptPart");

        migrationBuilder.CreateIndex(name: "IX_TestSessionCandidate_TestSessionId", schema: "hr", table: "TestSessionCandidate", column: "TestSessionId");
        migrationBuilder.CreateIndex(name: "IX_TestAttemptQuestion_TestAttemptId", schema: "hr", table: "TestAttemptQuestion", column: "TestAttemptId");
        migrationBuilder.CreateIndex(name: "IX_TestAttemptPart_TestAttemptId", schema: "hr", table: "TestAttemptPart", column: "TestAttemptId");
        migrationBuilder.CreateIndex(name: "IX_TestAttempt_TestSessionCandidateId", schema: "hr", table: "TestAttempt", column: "TestSessionCandidateId");
    }
}
