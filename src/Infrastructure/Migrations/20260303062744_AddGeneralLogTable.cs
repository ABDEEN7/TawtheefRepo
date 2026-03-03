using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGeneralLogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActionLog",
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
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ActionType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LogType = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    Section = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActionLog_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActionLog_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActionLog_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "SkillType",
                keyColumn: "Id",
                keyValue: new Guid("2f1b6ce7-cbc3-2b5c-b264-a02c6a87be1d"),
                column: "DescriptionAr",
                value: "مهارات تعليمية تم اكتسابها من خلال التعليم الرسمي");

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "SkillType",
                keyColumn: "Id",
                keyValue: new Guid("83b504d0-25c1-5ca0-6757-df299869f002"),
                column: "DescriptionAr",
                value: "مهارات مهنية وسلوكية مرتبطة ببيئة العمل");

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "SkillType",
                keyColumn: "Id",
                keyValue: new Guid("d14ac141-c057-16f5-ddd8-97d02f6a7c9b"),
                column: "DescriptionAr",
                value: "مهارات تقنية أو عملية مرتبطة بأدوات أو تقنيات أو منهجيات محددة");

            migrationBuilder.CreateIndex(
                name: "IX_ActionLog_CreatedById",
                schema: "hr",
                table: "ActionLog",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ActionLog_DeletedById",
                schema: "hr",
                table: "ActionLog",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ActionLog_UpdatedById",
                schema: "hr",
                table: "ActionLog",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionLog",
                schema: "hr");

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "SkillType",
                keyColumn: "Id",
                keyValue: new Guid("2f1b6ce7-cbc3-2b5c-b264-a02c6a87be1d"),
                column: "DescriptionAr",
                value: "مهارات تعليمية تم الحصول عليها من خلال التعليم الرسمي");

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "SkillType",
                keyColumn: "Id",
                keyValue: new Guid("83b504d0-25c1-5ca0-6757-df299869f002"),
                column: "DescriptionAr",
                value: "مهارات مهنية ناعمة وكفاءات مكان العمل");

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "SkillType",
                keyColumn: "Id",
                keyValue: new Guid("d14ac141-c057-16f5-ddd8-97d02f6a7c9b"),
                column: "DescriptionAr",
                value: "مهارات تقنية أو صلبة تتعلق بأدوات أو تقنيات أو منهجيات محددة");
        }
    }
}
