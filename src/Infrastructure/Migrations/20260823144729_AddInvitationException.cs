using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvitationException : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Source",
                schema: "hr",
                table: "Invitation",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Normal");

            migrationBuilder.CreateTable(
                name: "InvitationException",
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
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvitationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CancellationReason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ProofResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "ReadyToSend")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvitationException", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvitationException_AspNetUsers_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvitationException_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvitationException_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvitationException_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvitationException_Invitation_InvitationId",
                        column: x => x.InvitationId,
                        principalSchema: "hr",
                        principalTable: "Invitation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvitationException_Job_JobId",
                        column: x => x.JobId,
                        principalSchema: "hr",
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvitationException_Resources_ProofResourceId",
                        column: x => x.ProofResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Permission",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsAssignableToRole", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("39e706ac-4288-0053-8d56-9cf1d23b3f8c"), "exceptions.cancel", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 105, true, true, false, "الاستثناءات - إلغاء", "Exceptions - Cancel", null, null },
                    { new Guid("49c0e32d-20fd-7551-b5ab-ec3498123be0"), "exceptions.create", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 103, true, true, false, "الاستثناءات - إضافة", "Exceptions - Create", null, null },
                    { new Guid("8d47f7d5-7ea1-bc51-8c09-eaf6d00d7d90"), "exceptions.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 102, true, true, false, "الاستثناءات - عرض", "Exceptions - View", null, null },
                    { new Guid("affbbe6c-d76a-9b5f-9308-d24d5a0f978b"), "exceptions.send-invitation", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 104, true, true, false, "الاستثناءات - إرسال دعوة", "Exceptions - Send Invitation", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invitation_ApplicantId_JobId_Active",
                schema: "hr",
                table: "Invitation",
                columns: new[] { "ApplicantId", "JobId" },
                unique: true,
                filter: "[IsDeleted] = 0 AND [InvitationStatusId] IN ('F0BC801D-F54C-4A0E-8AAE-00694E4FC80D','64236C6A-167A-4213-B1D6-80C2C8C86DDE','6608F560-4DC0-4F2A-A190-6743A9A8C5CB','9B86FA2C-D295-46D7-9092-23AD8AB7B10C','22EF7E86-28CB-4A30-98BC-7D45F9B44DE3')");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationException_ApplicantId",
                schema: "hr",
                table: "InvitationException",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationException_ApplicantId_JobId_InFlight",
                schema: "hr",
                table: "InvitationException",
                columns: new[] { "ApplicantId", "JobId" },
                unique: true,
                filter: "[Status] IN (N'ReadyToSend', N'InvitationSent') AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationException_CreatedById",
                schema: "hr",
                table: "InvitationException",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationException_CreatedDate",
                schema: "hr",
                table: "InvitationException",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationException_DeletedById",
                schema: "hr",
                table: "InvitationException",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationException_InvitationId",
                schema: "hr",
                table: "InvitationException",
                column: "InvitationId");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationException_IsDeleted",
                schema: "hr",
                table: "InvitationException",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationException_JobId",
                schema: "hr",
                table: "InvitationException",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationException_ProofResourceId",
                schema: "hr",
                table: "InvitationException",
                column: "ProofResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationException_Status",
                schema: "hr",
                table: "InvitationException",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationException_UpdatedById",
                schema: "hr",
                table: "InvitationException",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvitationException",
                schema: "hr");

            migrationBuilder.DropIndex(
                name: "IX_Invitation_ApplicantId_JobId_Active",
                schema: "hr",
                table: "Invitation");

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("39e706ac-4288-0053-8d56-9cf1d23b3f8c"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("49c0e32d-20fd-7551-b5ab-ec3498123be0"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("8d47f7d5-7ea1-bc51-8c09-eaf6d00d7d90"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("affbbe6c-d76a-9b5f-9308-d24d5a0f978b"));

            migrationBuilder.DropColumn(
                name: "Source",
                schema: "hr",
                table: "Invitation");
        }
    }
}
