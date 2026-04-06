using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMinisterOfficeEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MinisterOfficeCandidate",
                schema: "hr",
                columns: table => new
                {
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Qid = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    QidExpiryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsFollowUpActive = table.Column<bool>(type: "bit", nullable: false),
                    FullNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FullNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NationalityEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NationalityAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NationalityCode = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinisterOfficeCandidate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MinisterOfficeCandidate_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MinisterOfficeCandidate_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MinisterOfficeCandidate_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MinisterOfficeCandidateAuditLog",
                schema: "hr",
                columns: table => new
                {
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Qid = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinisterOfficeCandidateAuditLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MinisterOfficeCandidateAuditLog_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MinisterOfficeCandidateAuditLog_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MinisterOfficeCandidateAuditLog_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MinisterOfficeCandidateAuditLog_MinisterOfficeCandidate_CandidateId",
                        column: x => x.CandidateId,
                        principalSchema: "hr",
                        principalTable: "MinisterOfficeCandidate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MinisterOfficeCandidate_CreatedById",
                schema: "hr",
                table: "MinisterOfficeCandidate",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MinisterOfficeCandidate_CreatedDate",
                schema: "hr",
                table: "MinisterOfficeCandidate",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_MinisterOfficeCandidate_DeletedById",
                schema: "hr",
                table: "MinisterOfficeCandidate",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_MinisterOfficeCandidate_IsDeleted",
                schema: "hr",
                table: "MinisterOfficeCandidate",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_MinisterOfficeCandidate_Qid",
                schema: "hr",
                table: "MinisterOfficeCandidate",
                column: "Qid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MinisterOfficeCandidate_UpdatedById",
                schema: "hr",
                table: "MinisterOfficeCandidate",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MinisterOfficeCandidateAuditLog_Action",
                schema: "hr",
                table: "MinisterOfficeCandidateAuditLog",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_MinisterOfficeCandidateAuditLog_CandidateId",
                schema: "hr",
                table: "MinisterOfficeCandidateAuditLog",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_MinisterOfficeCandidateAuditLog_CreatedById",
                schema: "hr",
                table: "MinisterOfficeCandidateAuditLog",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MinisterOfficeCandidateAuditLog_CreatedDate",
                schema: "hr",
                table: "MinisterOfficeCandidateAuditLog",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_MinisterOfficeCandidateAuditLog_DeletedById",
                schema: "hr",
                table: "MinisterOfficeCandidateAuditLog",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_MinisterOfficeCandidateAuditLog_IsDeleted",
                schema: "hr",
                table: "MinisterOfficeCandidateAuditLog",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_MinisterOfficeCandidateAuditLog_UpdatedById",
                schema: "hr",
                table: "MinisterOfficeCandidateAuditLog",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MinisterOfficeCandidateAuditLog",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "MinisterOfficeCandidate",
                schema: "hr");
        }
    }
}
