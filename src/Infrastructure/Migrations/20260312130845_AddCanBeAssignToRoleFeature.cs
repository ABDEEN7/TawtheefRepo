using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCanBeAssignToRoleFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAssignableToRole",
                schema: "lkp",
                table: "Permission",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("0148d1fe-79a7-0d5d-8391-7baf026f65fd"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("06a9ef54-2bd0-8b55-b746-16d7bce6274c"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("116b0925-93cf-6157-ba24-c12a87667dae"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("1502d704-7619-0255-ade1-8eab23a079a3"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("1e01a537-dae3-a85b-942c-0cf3541a2196"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("241a2e7e-4868-3e57-9f4b-a2e1d3bd2423"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("27a99574-bb73-425f-a1b3-0853b5234580"),
                column: "IsAssignableToRole",
                value: false);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("35dea045-9dab-125d-ab51-c820bf59525a"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("377a116d-4709-d65d-9565-c01e8908d25a"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("3b9b386e-13a1-f959-a87e-a9d598b7ecf8"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("42ba2b73-02d4-e156-803a-bfed80608058"),
                column: "IsAssignableToRole",
                value: false);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("42ea0acc-9bac-b15a-aef3-2ca7c7b9dbed"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("48571726-6c3c-0d55-9e84-5827bb52c9a4"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("49bcd54d-251e-b557-9d0f-67f8de39c326"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("5adc185c-aabf-a55f-9675-769a482552da"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("5b3122dc-edce-e253-a406-f4f384f3d20b"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("5cffb2e6-4548-0f5e-a045-79765647de0a"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("5d68fab4-cd5e-1958-a5f0-19d93f0db665"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("60271cb4-38b6-f752-98a3-a62a493f3486"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("64270af1-5bd1-9351-b23f-a993bc4ee8ed"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("64b8bb0d-a959-9555-ad77-3c3ab94bd7a6"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("6748978c-f5d7-7155-bba0-d992051bfa0f"),
                column: "IsAssignableToRole",
                value: false);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("6a437b3a-a459-c659-bf8c-f8035a7fed65"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("73bccdc8-d7e8-8d58-b71e-ce853d1cbca7"),
                column: "IsAssignableToRole",
                value: false);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("829e83f6-214b-e755-b6e8-01ccc773d5fe"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("83ff0b70-5423-b05f-a5aa-dd69f5155d6c"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("931e09d8-30f2-455a-b832-aad017c97cf8"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("9539bc9c-9df3-355c-870f-1664738a9c83"),
                column: "IsAssignableToRole",
                value: false);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("a2d2c7dc-d2a2-fd51-af47-34a24b77a14b"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("a601244f-898e-f95c-9e8b-a2ff34797f75"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("a6f1dfe0-8158-a65a-af51-860dc4f2b853"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("a8a3c689-062d-a457-ab7f-56a89ce5ba37"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("ab24906c-15de-7251-bbc0-d278fda72ae0"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("b99f006a-c1ca-e556-b8cc-c9608c643306"),
                column: "IsAssignableToRole",
                value: false);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("ca31d283-6388-325a-8dd6-9b26844c45fa"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("d1b8ab75-6b28-fd55-920b-c0d90ba46060"),
                column: "IsAssignableToRole",
                value: false);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("e6d9fe7e-1262-2859-b005-f999321c87d1"),
                column: "IsAssignableToRole",
                value: false);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("e8075b8b-8c77-ae57-a91e-530d5bdc07a9"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("e95eaa09-b74d-1955-b325-8896bc574523"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("ec49162a-0178-965a-807a-7752d369bbc2"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("f523d40b-2395-e454-a2d3-22e96edf347d"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("fe4eb16e-25b4-7950-be57-a79c2c212fa3"),
                column: "IsAssignableToRole",
                value: true);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("ffd0887b-67c6-a35f-82e8-1a973da7448a"),
                column: "IsAssignableToRole",
                value: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAssignableToRole",
                schema: "lkp",
                table: "Permission");
        }
    }
}
