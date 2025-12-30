#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Tawtheef.Infrastructure.Migrations.Stage
{
    /// <inheritdoc />
    public partial class AddMajorSkillTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MajorSkill",
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
                    MajorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsSkillRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MajorSkill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MajorSkill_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MajorSkill_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MajorSkill_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MajorSkill_Major_MajorId",
                        column: x => x.MajorId,
                        principalSchema: "lkp",
                        principalTable: "Major",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MajorSkill_Skill_SkillId",
                        column: x => x.SkillId,
                        principalSchema: "lkp",
                        principalTable: "Skill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("12f5805d-6970-4a9e-a275-2b7cf3db3bb8"),
                column: "ConcurrencyStamp",
                value: "01a9fb2f-6cf4-4017-af5c-5bae382b88f9");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("1361d691-53c5-4a84-aea1-64ff134cf082"),
                column: "ConcurrencyStamp",
                value: "e6479b8c-3a14-42b6-895c-7436573f8315");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd"),
                column: "ConcurrencyStamp",
                value: "c08e01e7-d615-46ac-afd3-b1413a5aba75");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca"),
                column: "ConcurrencyStamp",
                value: "b134981f-130d-4e1a-915f-bab3a856ca08");

            migrationBuilder.CreateIndex(
                name: "IX_MajorSkill_CreatedById",
                schema: "hr",
                table: "MajorSkill",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MajorSkill_DeletedById",
                schema: "hr",
                table: "MajorSkill",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_MajorSkill_MajorId",
                schema: "hr",
                table: "MajorSkill",
                column: "MajorId");

            migrationBuilder.CreateIndex(
                name: "IX_MajorSkill_SkillId",
                schema: "hr",
                table: "MajorSkill",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_MajorSkill_UpdatedById",
                schema: "hr",
                table: "MajorSkill",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MajorSkill",
                schema: "hr");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("12f5805d-6970-4a9e-a275-2b7cf3db3bb8"),
                column: "ConcurrencyStamp",
                value: "4f13f535-5bdd-43d2-91f3-8ce4c447aeac");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("1361d691-53c5-4a84-aea1-64ff134cf082"),
                column: "ConcurrencyStamp",
                value: "88e83fe5-963c-4fad-9d76-bb1a23c4c994");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd"),
                column: "ConcurrencyStamp",
                value: "d25a1f34-a3b4-47d0-9272-4c01951520be");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca"),
                column: "ConcurrencyStamp",
                value: "c0ae9539-292e-489a-96d5-fc0b8149aab4");
        }
    }
}
