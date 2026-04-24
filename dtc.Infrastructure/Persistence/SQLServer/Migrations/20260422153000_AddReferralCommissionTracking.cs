using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dtc.Infrastructure.Persistence.SQLServer.Migrations
{
    public partial class AddReferralCommissionTracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OriginalFee",
                table: "CourseRegistrations",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql(
                """
                UPDATE CourseRegistrations
                SET OriginalFee = TotalFee
                WHERE OriginalFee = 0;
                """);

            migrationBuilder.AddColumn<Guid>(
                name: "CourseRegistrationId",
                table: "ReferralRegistrations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReferralRegistrationId",
                table: "CollaboratorCommissions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReferralRegistrations_CourseRegistrationId",
                table: "ReferralRegistrations",
                column: "CourseRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_CollaboratorCommissions_ReferralRegistrationId",
                table: "CollaboratorCommissions",
                column: "ReferralRegistrationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReferralRegistrations_CourseRegistrations_CourseRegistrationId",
                table: "ReferralRegistrations",
                column: "CourseRegistrationId",
                principalTable: "CourseRegistrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CollaboratorCommissions_ReferralRegistrations_ReferralRegistrationId",
                table: "CollaboratorCommissions",
                column: "ReferralRegistrationId",
                principalTable: "ReferralRegistrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReferralRegistrations_CourseRegistrations_CourseRegistrationId",
                table: "ReferralRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_CollaboratorCommissions_ReferralRegistrations_ReferralRegistrationId",
                table: "CollaboratorCommissions");

            migrationBuilder.DropIndex(
                name: "IX_ReferralRegistrations_CourseRegistrationId",
                table: "ReferralRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_CollaboratorCommissions_ReferralRegistrationId",
                table: "CollaboratorCommissions");

            migrationBuilder.DropColumn(
                name: "OriginalFee",
                table: "CourseRegistrations");

            migrationBuilder.DropColumn(
                name: "CourseRegistrationId",
                table: "ReferralRegistrations");

            migrationBuilder.DropColumn(
                name: "ReferralRegistrationId",
                table: "CollaboratorCommissions");
        }
    }
}
