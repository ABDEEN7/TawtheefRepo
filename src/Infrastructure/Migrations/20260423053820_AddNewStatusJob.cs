using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewStatusJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "lkp",
                table: "JobStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[] { new Guid("3f2a1c9b-8b5d-4e1f-a3c9-1d7b4e5f6a2c"), "NeedPointUpdate", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "نقاط الوظيفة تحتاج للتعديل.", "Job points need to be updated.", 6, true, false, "نقاط تحتاج للتعديل", "Need Point Update", null, null });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("0148d1fe-79a7-0d5d-8391-7baf026f65fd"),
                column: "DisplayOrder",
                value: 42);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("06a9ef54-2bd0-8b55-b746-16d7bce6274c"),
                column: "DisplayOrder",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("1502d704-7619-0255-ade1-8eab23a079a3"),
                column: "DisplayOrder",
                value: 11);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("241a2e7e-4868-3e57-9f4b-a2e1d3bd2423"),
                column: "DisplayOrder",
                value: 17);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("35dea045-9dab-125d-ab51-c820bf59525a"),
                column: "DisplayOrder",
                value: 16);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("36030234-5820-8756-89b9-dd1038173a45"),
                column: "DisplayOrder",
                value: 37);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("3b9b386e-13a1-f959-a87e-a9d598b7ecf8"),
                column: "DisplayOrder",
                value: 19);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("42ea0acc-9bac-b15a-aef3-2ca7c7b9dbed"),
                column: "DisplayOrder",
                value: 22);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("48571726-6c3c-0d55-9e84-5827bb52c9a4"),
                column: "DisplayOrder",
                value: 6);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("50642bb5-f557-6954-99f3-6d6732446624"),
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "نقاط الوظائف - إدارة", "Jobs Points - Manage" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("5adc185c-aabf-a55f-9675-769a482552da"),
                column: "DisplayOrder",
                value: 12);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("6917c852-e9e4-6758-8e80-5a4797d8e7b9"),
                column: "DisplayOrder",
                value: 35);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("6fb9d7e1-c7e8-b05b-a2c4-16c5da50e1b0"),
                column: "DisplayOrder",
                value: 36);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("829e83f6-214b-e755-b6e8-01ccc773d5fe"),
                column: "DisplayOrder",
                value: 9);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("83ff0b70-5423-b05f-a5aa-dd69f5155d6c"),
                column: "DisplayOrder",
                value: 7);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("931e09d8-30f2-455a-b832-aad017c97cf8"),
                column: "DisplayOrder",
                value: 8);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("9e6683e2-9aab-d85c-9039-8e83d57085a0"),
                column: "DisplayOrder",
                value: 33);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("a2d2c7dc-d2a2-fd51-af47-34a24b77a14b"),
                column: "DisplayOrder",
                value: 14);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("a601244f-898e-f95c-9e8b-a2ff34797f75"),
                column: "DisplayOrder",
                value: 18);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("b9d42f3d-ab20-f85f-8a3f-448db7e727f2"),
                column: "DisplayOrder",
                value: 38);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("d14507aa-e61e-ec5f-abf8-f85b50bcaa9a"),
                column: "DisplayOrder",
                value: 45);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("d714523d-6aac-a255-bab5-f1b74ae0bd30"),
                column: "DisplayOrder",
                value: 34);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("e95eaa09-b74d-1955-b325-8896bc574523"),
                column: "DisplayOrder",
                value: 43);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("f523d40b-2395-e454-a2d3-22e96edf347d"),
                column: "DisplayOrder",
                value: 13);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("fe4eb16e-25b4-7950-be57-a79c2c212fa3"),
                column: "DisplayOrder",
                value: 44);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("ffd0887b-67c6-a35f-82e8-1a973da7448a"),
                column: "DisplayOrder",
                value: 15);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("3f2a1c9b-8b5d-4e1f-a3c9-1d7b4e5f6a2c"));

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("0148d1fe-79a7-0d5d-8391-7baf026f65fd"),
                column: "DisplayOrder",
                value: 33);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("06a9ef54-2bd0-8b55-b746-16d7bce6274c"),
                column: "DisplayOrder",
                value: 8);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("1502d704-7619-0255-ade1-8eab23a079a3"),
                column: "DisplayOrder",
                value: 9);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("241a2e7e-4868-3e57-9f4b-a2e1d3bd2423"),
                column: "DisplayOrder",
                value: 15);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("35dea045-9dab-125d-ab51-c820bf59525a"),
                column: "DisplayOrder",
                value: 14);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("36030234-5820-8756-89b9-dd1038173a45"),
                column: "DisplayOrder",
                value: 36);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("3b9b386e-13a1-f959-a87e-a9d598b7ecf8"),
                column: "DisplayOrder",
                value: 17);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("42ea0acc-9bac-b15a-aef3-2ca7c7b9dbed"),
                column: "DisplayOrder",
                value: 14);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("48571726-6c3c-0d55-9e84-5827bb52c9a4"),
                column: "DisplayOrder",
                value: 18);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("50642bb5-f557-6954-99f3-6d6732446624"),
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "نقاط الوظائف - تعديل", "Jobs - Points Edit" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("5adc185c-aabf-a55f-9675-769a482552da"),
                column: "DisplayOrder",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("6917c852-e9e4-6758-8e80-5a4797d8e7b9"),
                column: "DisplayOrder",
                value: 34);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("6fb9d7e1-c7e8-b05b-a2c4-16c5da50e1b0"),
                column: "DisplayOrder",
                value: 35);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("829e83f6-214b-e755-b6e8-01ccc773d5fe"),
                column: "DisplayOrder",
                value: 7);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("83ff0b70-5423-b05f-a5aa-dd69f5155d6c"),
                column: "DisplayOrder",
                value: 19);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("931e09d8-30f2-455a-b832-aad017c97cf8"),
                column: "DisplayOrder",
                value: 6);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("9e6683e2-9aab-d85c-9039-8e83d57085a0"),
                column: "DisplayOrder",
                value: 32);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("a2d2c7dc-d2a2-fd51-af47-34a24b77a14b"),
                column: "DisplayOrder",
                value: 12);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("a601244f-898e-f95c-9e8b-a2ff34797f75"),
                column: "DisplayOrder",
                value: 16);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("b9d42f3d-ab20-f85f-8a3f-448db7e727f2"),
                column: "DisplayOrder",
                value: 37);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("d14507aa-e61e-ec5f-abf8-f85b50bcaa9a"),
                column: "DisplayOrder",
                value: 34);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("d714523d-6aac-a255-bab5-f1b74ae0bd30"),
                column: "DisplayOrder",
                value: 33);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("e95eaa09-b74d-1955-b325-8896bc574523"),
                column: "DisplayOrder",
                value: 33);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("f523d40b-2395-e454-a2d3-22e96edf347d"),
                column: "DisplayOrder",
                value: 11);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("fe4eb16e-25b4-7950-be57-a79c2c212fa3"),
                column: "DisplayOrder",
                value: 34);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("ffd0887b-67c6-a35f-82e8-1a973da7448a"),
                column: "DisplayOrder",
                value: 13);
        }
    }
}
