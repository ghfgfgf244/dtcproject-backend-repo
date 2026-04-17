using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dtc.Infrastructure.Persistence.SQLServer.Migrations
{
    /// <inheritdoc />
    public partial class AddClassTypeToClasses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClassType",
                table: "Classes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE [Classes]
                SET [ClassType] = CASE
                    WHEN [ClassName] LIKE N'%TH%' THEN N'Theory'
                    WHEN [ClassName] LIKE N'%LT%' THEN N'Theory'
                    WHEN [ClassName] LIKE N'%THUC HANH%' THEN N'Practice'
                    WHEN [ClassName] LIKE N'%Practice%' THEN N'Practice'
                    WHEN [ClassName] LIKE N'%C-%' THEN N'Practice'
                    ELSE N'Theory'
                END
                WHERE [ClassType] IS NULL;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "ClassType",
                table: "Classes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666661"),
                column: "ClassType",
                value: "Theory");

            migrationBuilder.UpdateData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666662"),
                column: "ClassType",
                value: "Practice");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassType",
                table: "Classes");
        }
    }
}
