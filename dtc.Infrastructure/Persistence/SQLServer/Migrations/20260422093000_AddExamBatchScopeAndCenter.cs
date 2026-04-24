using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dtc.Infrastructure.Persistence.SQLServer.Migrations
{
    public partial class AddExamBatchScopeAndCenter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CenterId",
                table: "ExamBatches",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScopeType",
                table: "ExamBatches",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Center");

            migrationBuilder.Sql(
                """
                ;WITH BatchCenters AS (
                    SELECT
                        e.ExamBatchId,
                        COUNT(DISTINCT c.CenterId) AS CenterCount,
                        MIN(c.CenterId) AS SingleCenterId
                    FROM Exams e
                    INNER JOIN Courses c ON c.Id = e.CourseId
                    GROUP BY e.ExamBatchId
                )
                UPDATE eb
                SET
                    eb.CenterId = CASE WHEN bc.CenterCount = 1 THEN bc.SingleCenterId ELSE NULL END,
                    eb.ScopeType = CASE WHEN bc.CenterCount = 1 THEN 'Center' ELSE 'National' END
                FROM ExamBatches eb
                INNER JOIN BatchCenters bc ON bc.ExamBatchId = eb.Id;

                UPDATE eb
                SET
                    eb.CenterId = NULL,
                    eb.ScopeType = 'National'
                FROM ExamBatches eb
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM Exams e
                    WHERE e.ExamBatchId = eb.Id
                );
                """);

            migrationBuilder.UpdateData(
                table: "ExamBatches",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888881"),
                columns: new[] { "CenterId", "ScopeType" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333331"), "Center" });

            migrationBuilder.UpdateData(
                table: "ExamBatches",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888882"),
                columns: new[] { "CenterId", "ScopeType" },
                values: new object[] { null, "National" });

            migrationBuilder.CreateIndex(
                name: "IX_ExamBatches_CenterId",
                table: "ExamBatches",
                column: "CenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamBatches_Centers_CenterId",
                table: "ExamBatches",
                column: "CenterId",
                principalTable: "Centers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamBatches_Centers_CenterId",
                table: "ExamBatches");

            migrationBuilder.DropIndex(
                name: "IX_ExamBatches_CenterId",
                table: "ExamBatches");

            migrationBuilder.DropColumn(
                name: "CenterId",
                table: "ExamBatches");

            migrationBuilder.DropColumn(
                name: "ScopeType",
                table: "ExamBatches");
        }
    }
}
