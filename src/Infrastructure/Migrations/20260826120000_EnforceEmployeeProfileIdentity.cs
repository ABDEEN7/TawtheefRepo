using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Tawtheef.Infrastructure.Data;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations;

[DbContext(typeof(TawtheefDbContext))]
[Migration("20260826120000_EnforceEmployeeProfileIdentity")]
public sealed class EnforceEmployeeProfileIdentity : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {

        migrationBuilder.CreateIndex(
            name: "IX_EmployeeProfile_Qid",
            table: "EmployeeProfile",
            column: "Qid",
            unique: true,
            filter: "[Qid] IS NOT NULL AND [Qid] <> N''");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_EmployeeProfile_Qid",
            table: "EmployeeProfile");
    }
}
