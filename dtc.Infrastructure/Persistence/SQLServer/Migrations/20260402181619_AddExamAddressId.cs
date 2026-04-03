using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dtc.Infrastructure.Persistence.SQLServer.Migrations
{
    /// <inheritdoc />
    public partial class AddExamAddressId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "Exams",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Exams",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999991"),
                column: "AddressId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Exams",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999992"),
                column: "AddressId",
                value: 2);

            migrationBuilder.Sql(
                """
                UPDATE [Exams]
                SET [AddressId] = 1
                WHERE [AddressId] IS NULL;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "AddressId",
                table: "Exams",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Exams");
        }
    }
}
