using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileChangeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProfileChangeId",
                schema: "hr",
                table: "ReviewItem",
                type: "uniqueidentifier",
                nullable: true);

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
                    Section = table.Column<int>(type: "int", nullable: false),
                    TargetType = table.Column<int>(type: "int", nullable: false),
                    FieldPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AttachmentTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_ReviewItem_ProfileChangeId",
                schema: "hr",
                table: "ReviewItem",
                column: "ProfileChangeId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewItem_ProfileChange_ProfileChangeId",
                schema: "hr",
                table: "ReviewItem",
                column: "ProfileChangeId",
                principalSchema: "hr",
                principalTable: "ProfileChange",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewItem_ProfileChange_ProfileChangeId",
                schema: "hr",
                table: "ReviewItem");

            migrationBuilder.DropTable(
                name: "ProfileChange",
                schema: "hr");

            migrationBuilder.DropIndex(
                name: "IX_ReviewItem_ProfileChangeId",
                schema: "hr",
                table: "ReviewItem");

            migrationBuilder.DropColumn(
                name: "ProfileChangeId",
                schema: "hr",
                table: "ReviewItem");
        }
    }
}
