using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace dtc.Infrastructure.Persistence.SQLServer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Centers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CenterName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    NumberOfClasses = table.Column<int>(type: "int", nullable: false),
                    MaxStudentPerClass = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Centers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SampleExamResult",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SampleExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalScore = table.Column<double>(type: "float", nullable: false),
                    DurationSeconds = table.Column<int>(type: "int", nullable: false),
                    IsPassed = table.Column<bool>(type: "bit", nullable: false),
                    UserAnswersJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleExamResult", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentEvaluation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstructorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PunctualityScore = table.Column<int>(type: "int", nullable: false),
                    SkillLevel = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EvaluationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentEvaluation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClerkId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LicenseType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DurationInWeeks = table.Column<int>(type: "int", nullable: false),
                    MaxStudents = table.Column<int>(type: "int", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Centers_CenterId",
                        column: x => x.CenterId,
                        principalTable: "Centers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScopeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BatchName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RegistrationStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegistrationEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExamStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentCandidates = table.Column<int>(type: "int", nullable: false),
                    MaxCandidates = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamBatches_Centers_CenterId",
                        column: x => x.CenterId,
                        principalTable: "Centers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderPublicId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ResourceType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Extension = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Size = table.Column<int>(type: "int", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InstructorLeaveRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstructorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorLeaveRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstructorLeaveRequests_Users_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReferralCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CollaboratorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsedCount = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferralCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReferralCodes_Users_CollaboratorId",
                        column: x => x.CollaboratorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentDrivingDistances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MorningDistanceKm = table.Column<double>(type: "float", nullable: false),
                    EveningDistanceKm = table.Column<double>(type: "float", nullable: false),
                    MaxMorningDistanceKm = table.Column<double>(type: "float", nullable: false),
                    MaxEveningDistanceKm = table.Column<double>(type: "float", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentDrivingDistances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentDrivingDistances_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserCenters",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCenters", x => new { x.UserId, x.CenterId });
                    table.ForeignKey(
                        name: "FK_UserCenters_Centers_CenterId",
                        column: x => x.CenterId,
                        principalTable: "Centers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCenters_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Terms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TermName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentStudents = table.Column<int>(type: "int", nullable: false),
                    MaxStudents = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Terms_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamRegistrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamRegistrations_ExamBatches_ExamBatchId",
                        column: x => x.ExamBatchId,
                        principalTable: "ExamBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamRegistrations_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Exams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddressId = table.Column<int>(type: "int", nullable: false),
                    ExamName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ExamDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExamType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    TotalScore = table.Column<int>(type: "int", nullable: false),
                    PassScore = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exams_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Exams_ExamBatches_ExamBatchId",
                        column: x => x.ExamBatchId,
                        principalTable: "ExamBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TermId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstructorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CurrentStudents = table.Column<int>(type: "int", nullable: false),
                    MaxStudents = table.Column<int>(type: "int", nullable: false),
                    ClassType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Classes_Terms_TermId",
                        column: x => x.TermId,
                        principalTable: "Terms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Classes_Users_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseRegistrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedTermId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OriginalFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseRegistrations_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseRegistrations_Terms_AssignedTermId",
                        column: x => x.AssignedTermId,
                        principalTable: "Terms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseRegistrations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttemptNo = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<double>(type: "float", nullable: false),
                    IsPassed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamResults_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamResults_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClassSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstructorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AddressId = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassSchedules_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassSchedules_Users_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClassStudents",
                columns: table => new
                {
                    ClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassStudents", x => new { x.ClassId, x.StudentId });
                    table.ForeignKey(
                        name: "FK_ClassStudents_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassStudents_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReferralRegistrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferralCodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferralRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReferralRegistrations_CourseRegistrations_CourseRegistrationId",
                        column: x => x.CourseRegistrationId,
                        principalTable: "CourseRegistrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReferralRegistrations_ReferralCodes_ReferralCodeId",
                        column: x => x.ReferralCodeId,
                        principalTable: "ReferralCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReferralRegistrations_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Attendance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsPresent = table.Column<bool>(type: "bit", nullable: false),
                    CheckedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendance_ClassSchedules_ClassScheduleId",
                        column: x => x.ClassScheduleId,
                        principalTable: "ClassSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attendance_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CollaboratorCommissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CollaboratorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferralRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollaboratorCommissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollaboratorCommissions_ReferralRegistrations_ReferralRegistrationId",
                        column: x => x.ReferralRegistrationId,
                        principalTable: "ReferralRegistrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CollaboratorCommissions_Users_CollaboratorId",
                        column: x => x.CollaboratorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Centers",
                columns: new[] { "Id", "Address", "CenterName", "CreatedAt", "CreatedBy", "Email", "IsActive", "IsDeleted", "MaxStudentPerClass", "NumberOfClasses", "Phone", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333331"), "12 Nguyen Hue, Quan 1", "Trung Tam Lai Xe Quan 1", new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), "q1-center@dtc.local", true, false, 25, 6, "+842812345678", new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("33333333-3333-3333-3333-333333333332"), "99 Vo Van Ngan, Thu Duc", "Trung Tam Lai Xe Thu Duc", new DateTime(2026, 1, 10, 8, 5, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), "thuduc-center@dtc.local", true, false, 30, 8, "+842876543210", new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") }
                });

            migrationBuilder.InsertData(
                table: "ExamBatches",
                columns: new[] { "Id", "BatchName", "CenterId", "CreatedAt", "CreatedBy", "CurrentCandidates", "ExamStartDate", "IsDeleted", "MaxCandidates", "RegistrationEndDate", "RegistrationStartDate", "ScopeType", "Status", "UpdatedAt", "UpdatedBy" },
                values: new object[] { new Guid("88888888-8888-8888-8888-888888888882"), "Ky thi sat hach thang 06", null, new DateTime(2026, 1, 10, 9, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 1, new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc), false, 80, new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), "National", "Pending", null, null });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "RoleName" },
                values: new object[,]
                {
                    { 3, "Instructor" },
                    { 6, "Student" }
                });

            migrationBuilder.InsertData(
                table: "SampleExamResult",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DurationSeconds", "IsDeleted", "IsPassed", "SampleExamId", "StudentId", "TotalScore", "UpdatedAt", "UpdatedBy", "UserAnswersJson" },
                values: new object[,]
                {
                    { new Guid("dddddddd-dddd-dddd-dddd-ddddddddddd3"), new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), 1320, false, true, new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee5"), new Guid("22222222-2222-2222-2222-222222222222"), 34.0, null, null, "{\"1\":\"A\",\"2\":\"C\"}" },
                    { new Guid("dddddddd-dddd-dddd-dddd-ddddddddddd4"), new DateTime(2026, 1, 10, 15, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 1600, false, false, new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee6"), new Guid("11111111-1111-1111-1111-111111111111"), 24.0, null, null, "{\"1\":\"B\",\"2\":\"D\"}" }
                });

            migrationBuilder.InsertData(
                table: "StudentEvaluation",
                columns: new[] { "Id", "ClassId", "CreatedAt", "CreatedBy", "EvaluationDate", "InstructorId", "IsDeleted", "Note", "PunctualityScore", "SkillLevel", "StudentId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("cccccccc-cccc-cccc-cccc-ccccccccccc7"), new Guid("66666666-6666-6666-6666-666666666661"), new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 3, 20, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), false, "Hoc vien tien bo tot trong sa hinh.", 9, 8, new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("cccccccc-cccc-cccc-cccc-ccccccccccc8"), new Guid("66666666-6666-6666-6666-666666666662"), new DateTime(2026, 1, 10, 14, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 4, 25, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), false, "Can tang so buoi luyen tap ngoai duong truong.", 7, 6, new Guid("11111111-1111-1111-1111-111111111111"), null, null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvatarUrl", "ClerkId", "CreatedAt", "CreatedBy", "Email", "FullName", "IsActive", "IsDeleted", "LastLoginAt", "Phone", "RoleId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "https://cdn.example.com/avatar/instructor-demo.png", "clerk_instructor_demo", new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), null, "instructor.demo@dtc.local", "Tran Van Huong", true, false, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "+84901111222", 3, new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "https://cdn.example.com/avatar/student-demo.png", "clerk_student_demo", new DateTime(2026, 1, 10, 8, 10, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), "student.demo@dtc.local", "Le Thi Minh", true, false, new DateTime(2026, 1, 16, 9, 0, 0, 0, DateTimeKind.Utc), "+84903333444", 6, new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") }
                });

            migrationBuilder.InsertData(
                table: "CollaboratorCommissions",
                columns: new[] { "Id", "Amount", "CollaboratorId", "CreatedAt", "PaidAt", "ReferralRegistrationId", "Status" },
                values: new object[,]
                {
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb5"), 800000m, new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Pending" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb6"), 1250000m, new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Paid" }
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "CenterId", "CourseName", "CreatedAt", "CreatedBy", "Description", "DurationInWeeks", "IsActive", "IsDeleted", "LicenseType", "MaxStudents", "Price", "ThumbnailUrl", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444441"), new Guid("33333333-3333-3333-3333-333333333331"), "Khoa B2 Tieu Chuan", new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), "Khoa hoc B2 danh cho hoc vien moi.", 12, true, false, "B", 25, 12500000m, "https://cdn.example.com/course/b2.jpg", new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("44444444-4444-4444-4444-444444444442"), new Guid("33333333-3333-3333-3333-333333333332"), "Khoa C Nang Cao", new DateTime(2026, 1, 10, 8, 20, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), "Khoa hoc C danh cho tai xe tai nhe.", 16, true, false, "C", 20, 15800000m, "https://cdn.example.com/course/c.jpg", new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") }
                });

            migrationBuilder.InsertData(
                table: "Documents",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Extension", "FileName", "IsDeleted", "IsVerified", "ProviderPublicId", "ResourceType", "Size", "UpdatedAt", "UpdatedBy", "UserId", "Version" },
                values: new object[,]
                {
                    { new Guid("cccccccc-cccc-cccc-cccc-ccccccccccc1"), new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), ".pdf", "cccd-student-demo.pdf", false, true, "user_docs/student-demo-cccd", "raw", 512000, new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("22222222-2222-2222-2222-222222222222"), "1710000001" },
                    { new Guid("cccccccc-cccc-cccc-cccc-ccccccccccc2"), new DateTime(2026, 1, 10, 13, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), ".jpg", "giay-phep-instructor-demo.jpg", false, false, "user_docs/instructor-demo-license", "image", 348000, null, null, new Guid("11111111-1111-1111-1111-111111111111"), "1710000002" }
                });

            migrationBuilder.InsertData(
                table: "ExamBatches",
                columns: new[] { "Id", "BatchName", "CenterId", "CreatedAt", "CreatedBy", "CurrentCandidates", "ExamStartDate", "IsDeleted", "MaxCandidates", "RegistrationEndDate", "RegistrationStartDate", "ScopeType", "Status", "UpdatedAt", "UpdatedBy" },
                values: new object[] { new Guid("88888888-8888-8888-8888-888888888881"), "Ky thi sat hach thang 05", new Guid("33333333-3333-3333-3333-333333333331"), new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 1, new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), false, 100, new DateTime(2026, 4, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Center", "OpenForRegistration", new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.InsertData(
                table: "ExamRegistrations",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "ExamBatchId", "IsDeleted", "IsPaid", "RegistrationDate", "Status", "StudentId", "UpdatedAt", "UpdatedBy" },
                values: new object[] { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"), new DateTime(2026, 1, 10, 11, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888882"), false, false, new DateTime(2026, 5, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Pending", new Guid("11111111-1111-1111-1111-111111111111"), null, null });

            migrationBuilder.InsertData(
                table: "InstructorLeaveRequests",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "InstructorId", "IsDeleted", "LeaveDate", "Reason", "Status", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("cccccccc-cccc-cccc-cccc-ccccccccccc5"), new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), false, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Nghi phep ca nhan.", "Approved", new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("cccccccc-cccc-cccc-cccc-ccccccccccc6"), new DateTime(2026, 1, 10, 8, 50, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), false, new DateTime(2026, 4, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Tham gia boi duong nghiep vu.", "Pending", null, null }
                });

            migrationBuilder.InsertData(
                table: "ReferralCodes",
                columns: new[] { "Id", "Code", "CollaboratorId", "CreatedAt", "CreatedBy", "IsActive", "IsDeleted", "UpdatedAt", "UpdatedBy", "UsedCount" },
                values: new object[,]
                {
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1"), "CTVQ1A01", new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), true, false, new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 1 },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2"), "CTVTDA02", new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 10, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), true, false, null, null, 0 }
                });

            migrationBuilder.InsertData(
                table: "StudentDrivingDistances",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "EveningDistanceKm", "IsDeleted", "MaxEveningDistanceKm", "MaxMorningDistanceKm", "MorningDistanceKm", "StudentId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("cccccccc-cccc-cccc-cccc-ccccccccccc3"), new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 18.199999999999999, false, 40.0, 80.0, 42.5, new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("cccccccc-cccc-cccc-cccc-ccccccccccc4"), new DateTime(2026, 1, 10, 8, 55, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 6.0, false, 30.0, 60.0, 12.0, new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") }
                });

            migrationBuilder.InsertData(
                table: "UserCenters",
                columns: new[] { "CenterId", "UserId" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333331"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("33333333-3333-3333-3333-333333333332"), new Guid("22222222-2222-2222-2222-222222222222") }
                });

            migrationBuilder.InsertData(
                table: "CourseRegistrations",
                columns: new[] { "Id", "AssignedTermId", "CourseId", "CreatedAt", "CreatedBy", "IsDeleted", "Notes", "OriginalFee", "RegistrationDate", "Status", "TotalFee", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[] { new Guid("dddddddd-dddd-dddd-dddd-ddddddddddd2"), null, new Guid("44444444-4444-4444-4444-444444444442"), new DateTime(2026, 1, 10, 8, 30, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), false, "Cho xep lop.", 15800000m, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Pending", 15800000m, null, null, new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.InsertData(
                table: "ExamRegistrations",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "ExamBatchId", "IsDeleted", "IsPaid", "RegistrationDate", "Status", "StudentId", "UpdatedAt", "UpdatedBy" },
                values: new object[] { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"), new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888881"), false, true, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Approved", new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.InsertData(
                table: "Exams",
                columns: new[] { "Id", "AddressId", "CourseId", "CreatedAt", "CreatedBy", "DurationMinutes", "ExamBatchId", "ExamDate", "ExamName", "ExamType", "IsDeleted", "PassScore", "Status", "TotalScore", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("99999999-9999-9999-9999-999999999991"), 1, new Guid("44444444-4444-4444-4444-444444444441"), new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 25, new Guid("88888888-8888-8888-8888-888888888881"), new DateTime(2026, 5, 5, 1, 0, 0, 0, DateTimeKind.Utc), "Thi Ly Thuyet B2", "Theory", false, 32, "Scheduled", 35, new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("99999999-9999-9999-9999-999999999992"), 2, new Guid("44444444-4444-4444-4444-444444444442"), new DateTime(2026, 1, 10, 10, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 40, new Guid("88888888-8888-8888-8888-888888888882"), new DateTime(2026, 6, 8, 2, 0, 0, 0, DateTimeKind.Utc), "Thi Thuc Hanh C", "Practice", false, 80, "Draft", 100, null, null }
                });

            migrationBuilder.InsertData(
                table: "ReferralRegistrations",
                columns: new[] { "Id", "CourseRegistrationId", "ReferralCodeId", "RegisteredAt", "StudentId" },
                values: new object[,]
                {
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3"), null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1"), new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb4"), null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2"), new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") }
                });

            migrationBuilder.InsertData(
                table: "Terms",
                columns: new[] { "Id", "CourseId", "CreatedAt", "CreatedBy", "CurrentStudents", "EndDate", "IsActive", "IsDeleted", "MaxStudents", "StartDate", "TermName", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("55555555-5555-5555-5555-555555555551"), new Guid("44444444-4444-4444-4444-444444444441"), new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 1, new DateTime(2026, 5, 31, 0, 0, 0, 0, DateTimeKind.Utc), true, false, 25, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dot 03-2026 B2", new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("55555555-5555-5555-5555-555555555552"), new Guid("44444444-4444-4444-4444-444444444442"), new DateTime(2026, 1, 10, 8, 25, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 1, new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, false, 20, new DateTime(2026, 4, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Dot 04-2026 C", new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") }
                });

            migrationBuilder.InsertData(
                table: "Classes",
                columns: new[] { "Id", "ClassName", "ClassType", "CreatedAt", "CreatedBy", "CurrentStudents", "InstructorId", "IsDeleted", "MaxStudents", "Status", "TermId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("66666666-6666-6666-6666-666666666661"), "B2-01-Q1", "Theory", new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 1, new Guid("11111111-1111-1111-1111-111111111111"), false, 25, "InProgress", new Guid("55555555-5555-5555-5555-555555555551"), new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("66666666-6666-6666-6666-666666666662"), "C-01-TD", "Practice", new DateTime(2026, 1, 10, 8, 35, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 1, new Guid("11111111-1111-1111-1111-111111111111"), false, 20, "Pending", new Guid("55555555-5555-5555-5555-555555555552"), new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") }
                });

            migrationBuilder.InsertData(
                table: "CourseRegistrations",
                columns: new[] { "Id", "AssignedTermId", "CourseId", "CreatedAt", "CreatedBy", "IsDeleted", "Notes", "OriginalFee", "RegistrationDate", "Status", "TotalFee", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[] { new Guid("dddddddd-dddd-dddd-dddd-ddddddddddd1"), new Guid("55555555-5555-5555-5555-555555555551"), new Guid("44444444-4444-4444-4444-444444444441"), new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), false, "Da xac nhan hoc phi dot 1.", 12500000m, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Approved", 12500000m, new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.InsertData(
                table: "ExamResults",
                columns: new[] { "Id", "AttemptNo", "ExamId", "IsPassed", "Score", "StudentId" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"), 1, new Guid("99999999-9999-9999-9999-999999999991"), true, 33.0, new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4"), 1, new Guid("99999999-9999-9999-9999-999999999992"), false, 78.0, new Guid("11111111-1111-1111-1111-111111111111") }
                });

            migrationBuilder.InsertData(
                table: "ClassSchedules",
                columns: new[] { "Id", "AddressId", "ClassId", "CreatedAt", "CreatedBy", "EndTime", "InstructorId", "IsDeleted", "StartTime", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("77777777-7777-7777-7777-777777777771"), 1, new Guid("66666666-6666-6666-6666-666666666661"), new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 3, 15, 3, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), false, new DateTime(2026, 3, 15, 1, 0, 0, 0, DateTimeKind.Utc), null, null },
                    { new Guid("77777777-7777-7777-7777-777777777772"), 2, new Guid("66666666-6666-6666-6666-666666666662"), new DateTime(2026, 1, 10, 8, 40, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 4, 20, 3, 30, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), false, new DateTime(2026, 4, 20, 1, 30, 0, 0, DateTimeKind.Utc), null, null }
                });

            migrationBuilder.InsertData(
                table: "ClassStudents",
                columns: new[] { "ClassId", "StudentId" },
                values: new object[,]
                {
                    { new Guid("66666666-6666-6666-6666-666666666661"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("66666666-6666-6666-6666-666666666662"), new Guid("11111111-1111-1111-1111-111111111111") }
                });

            migrationBuilder.InsertData(
                table: "Attendance",
                columns: new[] { "Id", "CheckedAt", "ClassScheduleId", "CreatedAt", "CreatedBy", "IsDeleted", "IsPresent", "StudentId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("cccccccc-cccc-cccc-cccc-ccccccccccc9"), new DateTime(2026, 3, 15, 1, 5, 0, 0, DateTimeKind.Utc), new Guid("77777777-7777-7777-7777-777777777771"), new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), false, true, new Guid("22222222-2222-2222-2222-222222222222"), null, null },
                    { new Guid("cccccccc-cccc-cccc-cccc-ccccccccccca"), new DateTime(2026, 4, 20, 1, 35, 0, 0, DateTimeKind.Utc), new Guid("77777777-7777-7777-7777-777777777772"), new DateTime(2026, 1, 10, 8, 45, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), false, false, new Guid("11111111-1111-1111-1111-111111111111"), null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_ClassScheduleId_StudentId",
                table: "Attendance",
                columns: new[] { "ClassScheduleId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_StudentId",
                table: "Attendance",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_InstructorId",
                table: "Classes",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_TermId",
                table: "Classes",
                column: "TermId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSchedules_ClassId",
                table: "ClassSchedules",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSchedules_InstructorId",
                table: "ClassSchedules",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassStudents_StudentId",
                table: "ClassStudents",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_CollaboratorCommissions_CollaboratorId",
                table: "CollaboratorCommissions",
                column: "CollaboratorId");

            migrationBuilder.CreateIndex(
                name: "IX_CollaboratorCommissions_ReferralRegistrationId",
                table: "CollaboratorCommissions",
                column: "ReferralRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseRegistrations_AssignedTermId",
                table: "CourseRegistrations",
                column: "AssignedTermId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseRegistrations_CourseId",
                table: "CourseRegistrations",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseRegistrations_UserId",
                table: "CourseRegistrations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CenterId",
                table: "Courses",
                column: "CenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_UserId",
                table: "Documents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamBatches_CenterId",
                table: "ExamBatches",
                column: "CenterId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamRegistrations_ExamBatchId",
                table: "ExamRegistrations",
                column: "ExamBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamRegistrations_StudentId",
                table: "ExamRegistrations",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResults_ExamId",
                table: "ExamResults",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResults_StudentId",
                table: "ExamResults",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_CourseId",
                table: "Exams",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_ExamBatchId",
                table: "Exams",
                column: "ExamBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorLeaveRequests_InstructorId",
                table: "InstructorLeaveRequests",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferralCodes_Code",
                table: "ReferralCodes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReferralCodes_CollaboratorId",
                table: "ReferralCodes",
                column: "CollaboratorId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferralRegistrations_CourseRegistrationId",
                table: "ReferralRegistrations",
                column: "CourseRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferralRegistrations_ReferralCodeId",
                table: "ReferralRegistrations",
                column: "ReferralCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferralRegistrations_StudentId",
                table: "ReferralRegistrations",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentDrivingDistances_StudentId",
                table: "StudentDrivingDistances",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Terms_CourseId",
                table: "Terms",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCenters_CenterId",
                table: "UserCenters",
                column: "CenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ClerkId",
                table: "Users",
                column: "ClerkId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendance");

            migrationBuilder.DropTable(
                name: "ClassStudents");

            migrationBuilder.DropTable(
                name: "CollaboratorCommissions");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "ExamRegistrations");

            migrationBuilder.DropTable(
                name: "ExamResults");

            migrationBuilder.DropTable(
                name: "InstructorLeaveRequests");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "SampleExamResult");

            migrationBuilder.DropTable(
                name: "StudentDrivingDistances");

            migrationBuilder.DropTable(
                name: "StudentEvaluation");

            migrationBuilder.DropTable(
                name: "UserCenters");

            migrationBuilder.DropTable(
                name: "ClassSchedules");

            migrationBuilder.DropTable(
                name: "ReferralRegistrations");

            migrationBuilder.DropTable(
                name: "Exams");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "CourseRegistrations");

            migrationBuilder.DropTable(
                name: "ReferralCodes");

            migrationBuilder.DropTable(
                name: "ExamBatches");

            migrationBuilder.DropTable(
                name: "Terms");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Centers");
        }
    }
}
