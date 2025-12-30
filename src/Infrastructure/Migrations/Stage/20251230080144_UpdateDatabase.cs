#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations.Development
{
    /// <inheritdoc />
    public partial class UpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skill_Major_MajorId",
                schema: "lkp",
                table: "Skill");

            migrationBuilder.DropForeignKey(
                name: "FK_Skill_SkillRequirementType_SkillRequirementTypeId",
                schema: "lkp",
                table: "Skill");

            migrationBuilder.DropTable(
                name: "SkillRequirementType",
                schema: "lkp");

            migrationBuilder.DropIndex(
                name: "IX_Skill_MajorId",
                schema: "lkp",
                table: "Skill");

            migrationBuilder.DropIndex(
                name: "IX_Skill_SkillRequirementTypeId",
                schema: "lkp",
                table: "Skill");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "lkp",
                table: "Skill");

            migrationBuilder.DropColumn(
                name: "MajorId",
                schema: "lkp",
                table: "Skill");

            migrationBuilder.DropColumn(
                name: "SkillRequirementTypeId",
                schema: "lkp",
                table: "Skill");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "WorkType",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "UserType",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "University",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "TargetEntity",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "StudyType",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "SponsorType",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "SkillType",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "SkillLevel",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameEn",
                schema: "lkp",
                table: "Skill",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                schema: "lkp",
                table: "Skill",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "BackendName",
                schema: "lkp",
                table: "Skill",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "Skill",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "Sector",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "Religion",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "RatingGrade",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "ProviderLogin",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "Office",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "MaritalStatus",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "Management",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "Major",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "LanguageLevel",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "Language",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "JobStatus",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "JobCategory",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "InvitationStatus",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "Gender",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "Department",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "Degree",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "Country",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "City",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "CandidateType",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "lkp",
                table: "AchievementType",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "AchievementType",
                keyColumn: "Id",
                keyValue: new Guid("3f5f4d1f-b214-44cb-9b9e-2f9ec3c1f7f1"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "AchievementType",
                keyColumn: "Id",
                keyValue: new Guid("6c3b1b1b-0c9c-4e0e-8d1e-4d6c12e91533"),
                column: "IsActive",
                value: true);

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

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "CandidateType",
                keyColumn: "Id",
                keyValue: new Guid("268d49f6-0dda-4dd6-8346-be355c553496"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "CandidateType",
                keyColumn: "Id",
                keyValue: new Guid("42b374d1-8032-44d7-95bd-ff5a64abbc95"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "CandidateType",
                keyColumn: "Id",
                keyValue: new Guid("50f14c55-d5f0-4bba-930e-ab73b012e6cb"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "CandidateType",
                keyColumn: "Id",
                keyValue: new Guid("744256ea-4ee0-4a63-b071-8810895a33dc"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "CandidateType",
                keyColumn: "Id",
                keyValue: new Guid("7b06bc91-88b3-46ec-b6cc-ef2338864d41"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "CandidateType",
                keyColumn: "Id",
                keyValue: new Guid("ce67465e-6fed-4a83-9161-8eba16a79af3"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Degree",
                keyColumn: "Id",
                keyValue: new Guid("1af8c855-975f-4a49-bcf0-343a6d7fd8df"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Degree",
                keyColumn: "Id",
                keyValue: new Guid("6e453f48-5f2f-4f98-8b76-f416cdd4811b"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Degree",
                keyColumn: "Id",
                keyValue: new Guid("8ca14478-019d-4d3e-89cd-90cb89baf463"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Degree",
                keyColumn: "Id",
                keyValue: new Guid("d60cb9b1-f0ce-4c0a-a147-51e8d3947ef4"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Degree",
                keyColumn: "Id",
                keyValue: new Guid("de5901db-60dd-49f0-953c-daf923cf9f4a"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Degree",
                keyColumn: "Id",
                keyValue: new Guid("ebf2faa1-5ce6-4a04-9472-2746bfbbd252"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Degree",
                keyColumn: "Id",
                keyValue: new Guid("f1a31fe6-ba80-46cb-b24b-f402bcb4fdec"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Degree",
                keyColumn: "Id",
                keyValue: new Guid("f6249ce2-fa02-4e18-9c89-151ba3be0c12"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Department",
                keyColumn: "Id",
                keyValue: new Guid("0bfb044d-9f85-4f46-a1eb-920a1f9f519a"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Department",
                keyColumn: "Id",
                keyValue: new Guid("1b01eb93-b6e3-447a-8d1c-a9ce59bf5ca7"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Department",
                keyColumn: "Id",
                keyValue: new Guid("2a65e4a6-ca1b-4da3-8375-299ec39a50f9"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Department",
                keyColumn: "Id",
                keyValue: new Guid("48d2a180-30dc-4314-b222-92750a9d0afe"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Department",
                keyColumn: "Id",
                keyValue: new Guid("6881d9fd-7281-457a-a2e1-3cbe827622e8"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Department",
                keyColumn: "Id",
                keyValue: new Guid("7e225d86-c5e5-4225-b9fc-d255a975f5d1"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Department",
                keyColumn: "Id",
                keyValue: new Guid("8acd1c68-9b65-40e1-8776-ce6d7afcf542"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Department",
                keyColumn: "Id",
                keyValue: new Guid("8fd80cb6-794a-4211-b8a4-139e8e026e5b"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Department",
                keyColumn: "Id",
                keyValue: new Guid("91a511fb-9b43-47fd-9cd5-fc2a9ecbc32e"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Department",
                keyColumn: "Id",
                keyValue: new Guid("b858ce40-a3ac-4d27-aba9-86ef62fab4fe"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Gender",
                keyColumn: "Id",
                keyValue: new Guid("03cbe4e3-dc47-4d0c-8a07-87917af1d2dd"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Gender",
                keyColumn: "Id",
                keyValue: new Guid("4436a8ce-3f1e-4581-be3d-839c67127a9f"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Gender",
                keyColumn: "Id",
                keyValue: new Guid("6720e352-b360-41f0-8d3b-fa5116b7a0b4"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("22ef7e86-28cb-4a30-98bc-7d45f9b44de3"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("43ad4950-46a9-4b37-b4e2-4a2d8ac4a7c4"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("5602dbce-00a3-488b-9156-981a0fb18a01"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("64236c6a-167a-4213-b1d6-80c2c8c86dde"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("7103ba49-ad43-4751-b1a5-9084aca69676"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("bced01d8-3784-4e2c-9312-930951cc59d8"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("ca054592-8617-406d-8e8f-3a773b3d0d5e"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("e7b28f10-fb23-466e-a9b2-aaa3c9afdeac"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("f0bc801d-f54c-4a0e-8aae-00694e4fc80d"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "JobCategory",
                keyColumn: "Id",
                keyValue: new Guid("3d31e01b-9c57-4b47-8584-9fa8949838e8"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "JobCategory",
                keyColumn: "Id",
                keyValue: new Guid("4e7c8fe7-475b-4e4d-99f0-cfef85020d5b"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "JobCategory",
                keyColumn: "Id",
                keyValue: new Guid("62ecfbcc-7ae7-45c0-9db4-fd50751a336e"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("0d21e063-48d3-d320-4078-d85a7c2bf622"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("114dae76-bde9-3efa-2a2b-803ebd92e109"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("1e3ecad5-63fa-a11c-7acb-dd4c62ef74fd"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("5c360b07-157c-630a-254a-9c01587d80a8"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("c0d95787-8505-b84f-6f50-0b461ece500d"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("c64916a6-4bd2-a4b6-afbe-c5c3b4926530"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("e07bd71f-c466-0eea-49cc-2b13d1d9403f"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("e0cd7b22-8948-0c37-9b15-2e5217f00065"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("e0cd7b22-8948-0c37-9b15-2e5217f0c565"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("f2e7748a-12fe-53ac-fbdf-e989f8aa498a"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("8672c4c2-f635-4d26-843a-223fba3a6322"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Language",
                keyColumn: "Id",
                keyValue: new Guid("9843b692-ef7c-44a7-b1e8-1dcf2b6d87dd"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "LanguageLevel",
                keyColumn: "Id",
                keyValue: new Guid("5161f23a-f501-414c-b4fe-81cde9ebcd5d"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "LanguageLevel",
                keyColumn: "Id",
                keyValue: new Guid("9aa2c1bc-2040-40e1-86a2-72cac90928e1"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "LanguageLevel",
                keyColumn: "Id",
                keyValue: new Guid("afd6d7b4-9e20-40bc-a571-0df1670761b0"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "LanguageLevel",
                keyColumn: "Id",
                keyValue: new Guid("c7d8b159-c9dc-48a1-be6d-26493423f8a9"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "LanguageLevel",
                keyColumn: "Id",
                keyValue: new Guid("f0c9e84a-258f-4f6b-a469-153a92d68730"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Management",
                keyColumn: "Id",
                keyValue: new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Management",
                keyColumn: "Id",
                keyValue: new Guid("4575068a-4f0d-b6cd-8ea8-be680d8dc992"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "MaritalStatus",
                keyColumn: "Id",
                keyValue: new Guid("18e83653-978c-44c8-8691-43f95d9a5b7d"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "MaritalStatus",
                keyColumn: "Id",
                keyValue: new Guid("7113eb36-44b8-457a-96e9-61bfe6a06f05"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "MaritalStatus",
                keyColumn: "Id",
                keyValue: new Guid("8f22e74f-672b-47f4-8f32-93f9dbcb15da"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "MaritalStatus",
                keyColumn: "Id",
                keyValue: new Guid("c28ab4a0-59b0-4a64-82c4-ba1bd89efaba"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "ProviderLogin",
                keyColumn: "Id",
                keyValue: new Guid("0d1ab8a4-2b89-4dcc-aa6f-6ec92ccb887c"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "ProviderLogin",
                keyColumn: "Id",
                keyValue: new Guid("b8854959-1e46-4595-b51f-de3c09e3ed85"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "ProviderLogin",
                keyColumn: "Id",
                keyValue: new Guid("e0575116-ea2b-4917-965f-214048a4c78b"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("08e6f782-458e-4334-9bf1-f599c53b437a"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("2cf3d4e2-33cd-4671-9667-1b5e6e0cee9a"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("4dbd3381-f57a-4f6c-a718-432970d32276"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("71a78988-825a-4a0f-94a3-f59089dbe33d"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("76bd9c52-9c81-4d8f-afb4-fdd924744082"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Religion",
                keyColumn: "Id",
                keyValue: new Guid("185cbc18-2a59-40b0-a8d9-64b451f91ddf"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Religion",
                keyColumn: "Id",
                keyValue: new Guid("51588ec8-2d50-4365-aa8c-84efaec02e09"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Religion",
                keyColumn: "Id",
                keyValue: new Guid("5970a291-e636-4fd4-ab27-2af22dd77e9a"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Religion",
                keyColumn: "Id",
                keyValue: new Guid("6af60f76-d289-42d1-a3de-a3fa89056678"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Religion",
                keyColumn: "Id",
                keyValue: new Guid("7d76cd4c-915a-4ce2-a2b0-decddf0f7490"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Religion",
                keyColumn: "Id",
                keyValue: new Guid("c419dbdf-d0fc-4555-af2a-c5ed46847f8f"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Sector",
                keyColumn: "Id",
                keyValue: new Guid("a3c9f0eb-5d6e-6c4f-0a7b-8c9d0e1a2b3c"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Sector",
                keyColumn: "Id",
                keyValue: new Guid("b4da01fc-6e7f-7d50-1b8c-9d0e1a2b3c4d"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Sector",
                keyColumn: "Id",
                keyValue: new Guid("c5eb12fd-7f80-8e61-2c9d-0e1a2b3c4d5e"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Sector",
                keyColumn: "Id",
                keyValue: new Guid("e1a7f8d9-3b4c-4a2d-8e5f-6a7b8c9d0e1f"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Sector",
                keyColumn: "Id",
                keyValue: new Guid("f2b8e9fa-4c5d-5b3e-9f6a-7b8c9d0e1a2b"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "SkillLevel",
                keyColumn: "Id",
                keyValue: new Guid("2cf3d4e2-33cd-4671-9667-1b5e6e0cee9a"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "SkillLevel",
                keyColumn: "Id",
                keyValue: new Guid("4dbd3381-f57a-4f6c-a718-432970d32276"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "SkillLevel",
                keyColumn: "Id",
                keyValue: new Guid("69c0943a-f04f-4b6c-9145-1fbfae4b5c2e"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "SkillLevel",
                keyColumn: "Id",
                keyValue: new Guid("b8f10518-bf02-4357-a0e3-1f6bfb1746cd"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "SkillType",
                keyColumn: "Id",
                keyValue: new Guid("2f1b6ce7-cbc3-2b5c-b264-a02c6a87be1d"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "SkillType",
                keyColumn: "Id",
                keyValue: new Guid("83b504d0-25c1-5ca0-6757-df299869f002"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "SkillType",
                keyColumn: "Id",
                keyValue: new Guid("d14ac141-c057-16f5-ddd8-97d02f6a7c9b"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "SkillType",
                keyColumn: "Id",
                keyValue: new Guid("f91eb9e6-7a3f-76d6-1fe1-4443cecc5b9a"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "SponsorType",
                keyColumn: "Id",
                keyValue: new Guid("51588ec8-2d50-4365-aa8c-84efaec02e09"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "SponsorType",
                keyColumn: "Id",
                keyValue: new Guid("5970a291-e636-4fd4-ab27-2af22dd77e9a"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "StudyType",
                keyColumn: "Id",
                keyValue: new Guid("0a09774f-fa61-4896-b808-c8576cd0bf6b"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "StudyType",
                keyColumn: "Id",
                keyValue: new Guid("29754e1d-9125-4582-a998-68a72b8f8443"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "StudyType",
                keyColumn: "Id",
                keyValue: new Guid("b28f6a3a-e3ec-401e-9bc7-c4156b4bf76f"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "TargetEntity",
                keyColumn: "Id",
                keyValue: new Guid("1548322c-6c05-4754-a89c-19e9d2443d62"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "TargetEntity",
                keyColumn: "Id",
                keyValue: new Guid("8fadf8df-ae7e-4d22-8cb8-f79ed9b14375"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "UserType",
                keyColumn: "Id",
                keyValue: new Guid("5d1970fe-8398-44c0-88ea-560521933912"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "UserType",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-4879-8a3b-5c6d7e8f9a0b"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "UserType",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f6a7-5984-9b2c-6d7e8f9a0b1c"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "UserType",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e5f6-a7b8-6a95-0c3d-7e8f9a0b1c2d"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "WorkType",
                keyColumn: "Id",
                keyValue: new Guid("0fdfadbf-e5a0-41d3-a74d-d86354fed434"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "WorkType",
                keyColumn: "Id",
                keyValue: new Guid("e30074cd-52c6-41b5-a692-57626d109c73"),
                column: "IsActive",
                value: true);

            migrationBuilder.CreateIndex(
                name: "IX_Skill_BackendName",
                schema: "lkp",
                table: "Skill",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Skill_DisplayOrder",
                schema: "lkp",
                table: "Skill",
                column: "DisplayOrder");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Skill_BackendName",
                schema: "lkp",
                table: "Skill");

            migrationBuilder.DropIndex(
                name: "IX_Skill_DisplayOrder",
                schema: "lkp",
                table: "Skill");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "WorkType");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "UserType");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "University");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "TargetEntity");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "StudyType");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "SponsorType");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "SkillType");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "SkillLevel");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "Skill");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "Sector");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "Religion");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "RatingGrade");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "ProviderLogin");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "Office");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "MaritalStatus");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "Management");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "Major");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "LanguageLevel");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "Language");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "JobStatus");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "JobCategory");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "InvitationStatus");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "Gender");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "Degree");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "City");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "CandidateType");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "lkp",
                table: "AchievementType");

            migrationBuilder.AlterColumn<string>(
                name: "NameEn",
                schema: "lkp",
                table: "Skill",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                schema: "lkp",
                table: "Skill",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "BackendName",
                schema: "lkp",
                table: "Skill",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "lkp",
                table: "Skill",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MajorId",
                schema: "lkp",
                table: "Skill",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SkillRequirementTypeId",
                schema: "lkp",
                table: "Skill",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "SkillRequirementType",
                schema: "lkp",
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
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillRequirementType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillRequirementType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SkillRequirementType_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SkillRequirementType_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("12f5805d-6970-4a9e-a275-2b7cf3db3bb8"),
                column: "ConcurrencyStamp",
                value: "db98add5-8e90-47db-96db-f291baa280ed");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("1361d691-53c5-4a84-aea1-64ff134cf082"),
                column: "ConcurrencyStamp",
                value: "e01c6405-9903-4c59-a0ca-ac117ce2a93b");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd"),
                column: "ConcurrencyStamp",
                value: "06d0913c-071b-46ef-a4e5-185cf422d706");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca"),
                column: "ConcurrencyStamp",
                value: "dd7c8bfb-1aeb-426b-a51f-5affe6bd1b96");

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "SkillRequirementType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("a014fa55-b3ed-14e4-b4aa-15354a5d1cc3"), "Optional", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "متطلب مهارة اختياري", "Optional skill requirement", 2, false, "اختياري", "Optional", null, null },
                    { new Guid("b0f1f4dd-95ad-3d0b-e74c-684d2d288329"), "Essential", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "متطلب مهارة أساسي", "Essential skill requirement", 1, false, "أساسي", "Essential", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Skill_MajorId",
                schema: "lkp",
                table: "Skill",
                column: "MajorId");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_SkillRequirementTypeId",
                schema: "lkp",
                table: "Skill",
                column: "SkillRequirementTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillRequirementType_BackendName",
                schema: "lkp",
                table: "SkillRequirementType",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkillRequirementType_CreatedById",
                schema: "lkp",
                table: "SkillRequirementType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SkillRequirementType_DeletedById",
                schema: "lkp",
                table: "SkillRequirementType",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_SkillRequirementType_DisplayOrder",
                schema: "lkp",
                table: "SkillRequirementType",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_SkillRequirementType_UpdatedById",
                schema: "lkp",
                table: "SkillRequirementType",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Skill_Major_MajorId",
                schema: "lkp",
                table: "Skill",
                column: "MajorId",
                principalSchema: "lkp",
                principalTable: "Major",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Skill_SkillRequirementType_SkillRequirementTypeId",
                schema: "lkp",
                table: "Skill",
                column: "SkillRequirementTypeId",
                principalSchema: "lkp",
                principalTable: "SkillRequirementType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
