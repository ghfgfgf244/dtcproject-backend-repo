using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dtc.Infrastructure.Persistence.SQLServer.Migrations
{
    /// <inheritdoc />
    public partial class FinalModelSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SampleExamResults_Users_StudentId",
                table: "SampleExamResults");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentEvaluations_Classes_ClassId",
                table: "StudentEvaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentEvaluations_Users_InstructorId",
                table: "StudentEvaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentEvaluations_Users_StudentId",
                table: "StudentEvaluations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentEvaluations",
                table: "StudentEvaluations");

            migrationBuilder.DropIndex(
                name: "IX_StudentEvaluations_ClassId",
                table: "StudentEvaluations");

            migrationBuilder.DropIndex(
                name: "IX_StudentEvaluations_InstructorId",
                table: "StudentEvaluations");

            migrationBuilder.DropIndex(
                name: "IX_StudentEvaluations_StudentId",
                table: "StudentEvaluations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SampleExamResults",
                table: "SampleExamResults");

            migrationBuilder.DropIndex(
                name: "IX_SampleExamResults_StudentId",
                table: "SampleExamResults");

            migrationBuilder.RenameTable(
                name: "StudentEvaluations",
                newName: "StudentEvaluation");

            migrationBuilder.RenameTable(
                name: "SampleExamResults",
                newName: "SampleExamResult");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentEvaluation",
                table: "StudentEvaluation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SampleExamResult",
                table: "SampleExamResult",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentEvaluation",
                table: "StudentEvaluation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SampleExamResult",
                table: "SampleExamResult");

            migrationBuilder.RenameTable(
                name: "StudentEvaluation",
                newName: "StudentEvaluations");

            migrationBuilder.RenameTable(
                name: "SampleExamResult",
                newName: "SampleExamResults");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentEvaluations",
                table: "StudentEvaluations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SampleExamResults",
                table: "SampleExamResults",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEvaluations_ClassId",
                table: "StudentEvaluations",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEvaluations_InstructorId",
                table: "StudentEvaluations",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEvaluations_StudentId",
                table: "StudentEvaluations",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_SampleExamResults_StudentId",
                table: "SampleExamResults",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_SampleExamResults_Users_StudentId",
                table: "SampleExamResults",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentEvaluations_Classes_ClassId",
                table: "StudentEvaluations",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentEvaluations_Users_InstructorId",
                table: "StudentEvaluations",
                column: "InstructorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentEvaluations_Users_StudentId",
                table: "StudentEvaluations",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
