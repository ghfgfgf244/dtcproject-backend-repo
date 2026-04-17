using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dtc.Infrastructure.Persistence.SQLServer.Migrations
{
    /// <inheritdoc />
    public partial class AddClassScheduleAddressId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "ClassSchedules",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE [ClassSchedules]
                SET [AddressId] = CASE
                    WHEN [Location] LIKE N'%Quan 1%' THEN 1
                    WHEN [Location] LIKE N'%Thu Duc%' THEN 2
                    ELSE 1
                END
                WHERE [AddressId] IS NULL;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "AddressId",
                table: "ClassSchedules",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "ClassSchedules",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777771"),
                column: "AddressId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "ClassSchedules",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777772"),
                column: "AddressId",
                value: 2);

            migrationBuilder.DropColumn(
                name: "Location",
                table: "ClassSchedules");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "ClassSchedules");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "ClassSchedules",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "ClassSchedules",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777771"),
                column: "Location",
                value: "San tap Trung tam Quan 1");

            migrationBuilder.UpdateData(
                table: "ClassSchedules",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777772"),
                column: "Location",
                value: "Bai tap Thu Duc");
        }
    }
}
