using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvitationExpiryConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "ExpiresOn",
                schema: "hr",
                table: "Invitation",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.CreateTable(
                name: "InvitationExpiryConfiguration",
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
                    ExpiryDays = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvitationExpiryConfiguration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvitationExpiryConfiguration_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvitationExpiryConfiguration_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvitationExpiryConfiguration_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { -1711943202, "permission", "invitation-expiry-configuration.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -651629113, "permission", "invitation-expiry-configuration.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -134168132, "permission", "invitation-expiry-configuration.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -83519686, "permission", "invitation-expiry-configuration.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") }
                });

            migrationBuilder.InsertData(
                schema: "hr",
                table: "InvitationExpiryConfiguration",
                columns: new[] { "Id", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "ExpiryDays", "IsDeleted", "UpdatedById", "UpdatedDate" },
                values: new object[] { new Guid("d3ecf2b4-8ddf-48d7-859d-88b7fe32ef13"), null, new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, 7, false, null, null });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "InvitationStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[] { new Guid("c0a9d348-2175-4fa2-a1fb-fa6a6a729e64"), "Expired", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "انتهت صلاحية الدعوة لأن المرشح لم يستجب خلال الوقت المحدد.", "The invitation expired because the candidate did not respond within the specified time.", 9, true, false, "منتهي الصلاحية", "Expired", null, null });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Permission",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsAssignableToRole", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0342b09b-1b90-335f-ba63-c0c9a7bf0f3b"), "invitation-expiry-configuration.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 100, true, true, false, "Invitation Expiry Configuration - View", "Invitation Expiry Configuration - View", null, null },
                    { new Guid("30b1b4f9-70da-af5d-888b-f434086b5af0"), "invitation-expiry-configuration.manage", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 101, true, true, false, "Invitation Expiry Configuration - Manage", "Invitation Expiry Configuration - Manage", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvitationExpiryConfiguration_CreatedById",
                schema: "hr",
                table: "InvitationExpiryConfiguration",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationExpiryConfiguration_CreatedDate",
                schema: "hr",
                table: "InvitationExpiryConfiguration",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationExpiryConfiguration_DeletedById",
                schema: "hr",
                table: "InvitationExpiryConfiguration",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationExpiryConfiguration_IsDeleted",
                schema: "hr",
                table: "InvitationExpiryConfiguration",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationExpiryConfiguration_UpdatedById",
                schema: "hr",
                table: "InvitationExpiryConfiguration",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvitationExpiryConfiguration",
                schema: "hr");

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1711943202);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -651629113);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -134168132);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -83519686);

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("c0a9d348-2175-4fa2-a1fb-fa6a6a729e64"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("0342b09b-1b90-335f-ba63-c0c9a7bf0f3b"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("30b1b4f9-70da-af5d-888b-f434086b5af0"));

            migrationBuilder.DropColumn(
                name: "ExpiresOn",
                schema: "hr",
                table: "Invitation");
        }
    }
}
