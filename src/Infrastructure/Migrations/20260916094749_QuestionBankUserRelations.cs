using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    public partial class QuestionBankUserRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBankAssignment_EmployeeProfile_AssignedByUserId",
                schema: "hr",
                table: "QuestionBankAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBankAssignment_EmployeeProfile_EmployeeId",
                schema: "hr",
                table: "QuestionBankAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBankRequest_EmployeeProfile_FinalDecisionById",
                schema: "hr",
                table: "QuestionBankRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBankRequest_EmployeeProfile_SubmittedById",
                schema: "hr",
                table: "QuestionBankRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBankRequestItem_EmployeeProfile_RemovedById",
                schema: "hr",
                table: "QuestionBankRequestItem");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBankRequestItemReview_EmployeeProfile_ReviewedById",
                schema: "hr",
                table: "QuestionBankRequestItemReview");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBankRequestReview_EmployeeProfile_ReviewedById",
                schema: "hr",
                table: "QuestionBankRequestReview");

            RemapActorIds(migrationBuilder, toUsers: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionBankAssignment_AspNetUsers_AssignedByUserId",
                schema: "hr",
                table: "QuestionBankAssignment",
                column: "AssignedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionBankAssignment_AspNetUsers_EmployeeId",
                schema: "hr",
                table: "QuestionBankAssignment",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionBankRequest_AspNetUsers_FinalDecisionById",
                schema: "hr",
                table: "QuestionBankRequest",
                column: "FinalDecisionById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionBankRequest_AspNetUsers_SubmittedById",
                schema: "hr",
                table: "QuestionBankRequest",
                column: "SubmittedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionBankRequestItem_AspNetUsers_RemovedById",
                schema: "hr",
                table: "QuestionBankRequestItem",
                column: "RemovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionBankRequestItemReview_AspNetUsers_ReviewedById",
                schema: "hr",
                table: "QuestionBankRequestItemReview",
                column: "ReviewedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionBankRequestReview_AspNetUsers_ReviewedById",
                schema: "hr",
                table: "QuestionBankRequestReview",
                column: "ReviewedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBankAssignment_AspNetUsers_AssignedByUserId",
                schema: "hr",
                table: "QuestionBankAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBankAssignment_AspNetUsers_EmployeeId",
                schema: "hr",
                table: "QuestionBankAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBankRequest_AspNetUsers_FinalDecisionById",
                schema: "hr",
                table: "QuestionBankRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBankRequest_AspNetUsers_SubmittedById",
                schema: "hr",
                table: "QuestionBankRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBankRequestItem_AspNetUsers_RemovedById",
                schema: "hr",
                table: "QuestionBankRequestItem");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBankRequestItemReview_AspNetUsers_ReviewedById",
                schema: "hr",
                table: "QuestionBankRequestItemReview");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBankRequestReview_AspNetUsers_ReviewedById",
                schema: "hr",
                table: "QuestionBankRequestReview");

            RemapActorIds(migrationBuilder, toUsers: false);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionBankAssignment_EmployeeProfile_AssignedByUserId",
                schema: "hr",
                table: "QuestionBankAssignment",
                column: "AssignedByUserId",
                principalTable: "EmployeeProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionBankAssignment_EmployeeProfile_EmployeeId",
                schema: "hr",
                table: "QuestionBankAssignment",
                column: "EmployeeId",
                principalTable: "EmployeeProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionBankRequest_EmployeeProfile_FinalDecisionById",
                schema: "hr",
                table: "QuestionBankRequest",
                column: "FinalDecisionById",
                principalTable: "EmployeeProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionBankRequest_EmployeeProfile_SubmittedById",
                schema: "hr",
                table: "QuestionBankRequest",
                column: "SubmittedById",
                principalTable: "EmployeeProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionBankRequestItem_EmployeeProfile_RemovedById",
                schema: "hr",
                table: "QuestionBankRequestItem",
                column: "RemovedById",
                principalTable: "EmployeeProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionBankRequestItemReview_EmployeeProfile_ReviewedById",
                schema: "hr",
                table: "QuestionBankRequestItemReview",
                column: "ReviewedById",
                principalTable: "EmployeeProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionBankRequestReview_EmployeeProfile_ReviewedById",
                schema: "hr",
                table: "QuestionBankRequestReview",
                column: "ReviewedById",
                principalTable: "EmployeeProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
        private static void RemapActorIds(MigrationBuilder migrationBuilder, bool toUsers)
        {
            var relations = new (string Table, string Column)[]
            {
                ("QuestionBankAssignment", "AssignedByUserId"),
                ("QuestionBankAssignment", "EmployeeId"),
                ("QuestionBankRequest", "FinalDecisionById"),
                ("QuestionBankRequest", "SubmittedById"),
                ("QuestionBankRequestItem", "RemovedById"),
                ("QuestionBankRequestItemReview", "ReviewedById"),
                ("QuestionBankRequestReview", "ReviewedById")
            };
            var sourceKey = toUsers ? "EmployeeProfileId" : "Id";
            var targetKey = toUsers ? "Id" : "EmployeeProfileId";

            foreach (var (table, column) in relations)
            {
                // Never guess an actor when a profile is unlinked or shared by multiple users.
                // Downgrades likewise require every referenced user to still have a profile.
                migrationBuilder.Sql($$"""
                    IF EXISTS (
                        SELECT 1
                        FROM [hr].[{{table}}] AS actor
                        WHERE actor.[{{column}}] IS NOT NULL
                          AND (SELECT COUNT(*)
                               FROM [AspNetUsers] AS u
                               WHERE u.[{{sourceKey}}] = actor.[{{column}}]
                                 AND u.[{{targetKey}}] IS NOT NULL) <> 1
                    )
                    BEGIN
                        THROW 51000, 'Cannot migrate {{table}}.{{column}}: each actor must have exactly one linked {{targetKey}} in AspNetUsers.', 1;
                    END;

                    UPDATE actor
                    SET actor.[{{column}}] = u.[{{targetKey}}]
                    FROM [hr].[{{table}}] AS actor
                    INNER JOIN [AspNetUsers] AS u ON u.[{{sourceKey}}] = actor.[{{column}}];
                    """);
            }
        }
    }
}
