using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PhaseReviewProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewItem_ProfileChange_ProfileChangeId",
                schema: "hr",
                table: "ReviewItem");

            migrationBuilder.DropTable(
                name: "ProfileChange",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "ProfileSubmission",
                schema: "pro");

            migrationBuilder.DropColumn(
                name: "ApprovedAtVersion",
                schema: "hr",
                table: "ReviewItem");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "hr",
                table: "ReviewItem");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ReviewedAtUtc",
                schema: "hr",
                table: "ReviewItem",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ProfileChangeRequest",
                schema: "hr",
                columns: table => new
                {
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Section = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    TargetKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FieldPath = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EntityName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NewResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AttachmentTitle = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewerNote = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CanceledById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CanceledAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileChangeRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileChangeRequest_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileChangeRequest_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileChangeRequest_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileChangeRequest_UserProfile_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "app",
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfileChangeRequest_CreatedById",
                schema: "hr",
                table: "ProfileChangeRequest",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileChangeRequest_DeletedById",
                schema: "hr",
                table: "ProfileChangeRequest",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileChangeRequest_UpdatedById",
                schema: "hr",
                table: "ProfileChangeRequest",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileChangeRequest_UserProfileId_Section_Status",
                schema: "hr",
                table: "ProfileChangeRequest",
                columns: new[] { "UserProfileId", "Section", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ProfileChangeRequest_UserProfileId_TargetKey",
                schema: "hr",
                table: "ProfileChangeRequest",
                columns: new[] { "UserProfileId", "TargetKey" },
                unique: true,
                filter: "[Status] IN (1,2)");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewItem_ProfileChangeRequest_ProfileChangeId",
                schema: "hr",
                table: "ReviewItem",
                column: "ProfileChangeId",
                principalSchema: "hr",
                principalTable: "ProfileChangeRequest",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewItem_ProfileChangeRequest_ProfileChangeId",
                schema: "hr",
                table: "ReviewItem");

            migrationBuilder.DropTable(
                name: "ProfileChangeRequest",
                schema: "hr");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReviewedAtUtc",
                schema: "hr",
                table: "ReviewItem",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprovedAtVersion",
                schema: "hr",
                table: "ReviewItem",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                schema: "hr",
                table: "ReviewItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ProfileChange",
                schema: "hr",
                columns: table => new
                {
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttachmentTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EntityName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FieldPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Section = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TargetType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileChange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileChange_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileChange_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileChange_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileChange_UserProfile_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "app",
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileSubmission",
                schema: "pro",
                columns: table => new
                {
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SnapshotJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileSubmission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileSubmission_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileSubmission_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileSubmission_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfileChange_CreatedById",
                schema: "hr",
                table: "ProfileChange",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileChange_DeletedById",
                schema: "hr",
                table: "ProfileChange",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileChange_UpdatedById",
                schema: "hr",
                table: "ProfileChange",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileChange_UserProfileId",
                schema: "hr",
                table: "ProfileChange",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSubmission_CreatedById",
                schema: "pro",
                table: "ProfileSubmission",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSubmission_DeletedById",
                schema: "pro",
                table: "ProfileSubmission",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSubmission_UpdatedById",
                schema: "pro",
                table: "ProfileSubmission",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewItem_ProfileChange_ProfileChangeId",
                schema: "hr",
                table: "ReviewItem",
                column: "ProfileChangeId",
                principalSchema: "hr",
                principalTable: "ProfileChange",
                principalColumn: "Id");
        }
    }
}
