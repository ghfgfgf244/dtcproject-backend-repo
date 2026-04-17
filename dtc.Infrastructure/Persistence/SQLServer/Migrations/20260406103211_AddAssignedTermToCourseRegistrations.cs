using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dtc.Infrastructure.Persistence.SQLServer.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedTermToCourseRegistrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssignedTermId",
                table: "CourseRegistrations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CourseRegistrations",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-ddddddddddd1"),
                column: "AssignedTermId",
                value: new Guid("55555555-5555-5555-5555-555555555551"));

            migrationBuilder.UpdateData(
                table: "CourseRegistrations",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-ddddddddddd2"),
                column: "AssignedTermId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_CourseRegistrations_AssignedTermId",
                table: "CourseRegistrations",
                column: "AssignedTermId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseRegistrations_Terms_AssignedTermId",
                table: "CourseRegistrations",
                column: "AssignedTermId",
                principalTable: "Terms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseRegistrations_Terms_AssignedTermId",
                table: "CourseRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_CourseRegistrations_AssignedTermId",
                table: "CourseRegistrations");

            migrationBuilder.DropColumn(
                name: "AssignedTermId",
                table: "CourseRegistrations");
        }
    }
}
