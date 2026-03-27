using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace dtc.Infrastructure.Persistence.SQLServer.Migrations
{
    /// <inheritdoc />
    public partial class SeedAndContextSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                columns: new[] { "Id", "BatchName", "CreatedAt", "CreatedBy", "CurrentCandidates", "ExamStartDate", "IsDeleted", "MaxCandidates", "RegistrationEndDate", "RegistrationStartDate", "Status", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("88888888-8888-8888-8888-888888888881"), "Ky thi sat hach thang 05", new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 1, new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), false, 100, new DateTime(2026, 4, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "OpenForRegistration", new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("88888888-8888-8888-8888-888888888882"), "Ky thi sat hach thang 06", new DateTime(2026, 1, 10, 9, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 1, new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Utc), false, 80, new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pending", null, null }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "RoleName" },
                values: new object[,]
                {
                    { 3, "Instructor" },
                    { 6, "Student" }
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
                columns: new[] { "Id", "Amount", "CollaboratorId", "CreatedAt", "PaidAt", "Status" },
                values: new object[,]
                {
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb5"), 800000m, new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pending" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb6"), 1250000m, new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Paid" }
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
                table: "ExamRegistrations",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "ExamBatchId", "IsDeleted", "IsPaid", "RegistrationDate", "Status", "StudentId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"), new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888881"), false, true, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Approved", new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"), new DateTime(2026, 1, 10, 11, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888882"), false, false, new DateTime(2026, 5, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Pending", new Guid("11111111-1111-1111-1111-111111111111"), null, null }
                });

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
                table: "SampleExamResults",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DurationSeconds", "IsDeleted", "IsPassed", "SampleExamId", "StudentId", "TotalScore", "UpdatedAt", "UpdatedBy", "UserAnswersJson" },
                values: new object[,]
                {
                    { new Guid("dddddddd-dddd-dddd-dddd-ddddddddddd3"), new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), 1320, false, true, new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee5"), new Guid("22222222-2222-2222-2222-222222222222"), 34.0, null, null, "{\"1\":\"A\",\"2\":\"C\"}" },
                    { new Guid("dddddddd-dddd-dddd-dddd-ddddddddddd4"), new DateTime(2026, 1, 10, 15, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 1600, false, false, new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee6"), new Guid("11111111-1111-1111-1111-111111111111"), 24.0, null, null, "{\"1\":\"B\",\"2\":\"D\"}" }
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
                columns: new[] { "Id", "CourseId", "CreatedAt", "CreatedBy", "IsDeleted", "Notes", "RegistrationDate", "Status", "TotalFee", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[,]
                {
                    { new Guid("dddddddd-dddd-dddd-dddd-ddddddddddd1"), new Guid("44444444-4444-4444-4444-444444444441"), new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), false, "Da xac nhan hoc phi dot 1.", new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Approved", 12500000m, new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("dddddddd-dddd-dddd-dddd-ddddddddddd2"), new Guid("44444444-4444-4444-4444-444444444442"), new DateTime(2026, 1, 10, 8, 30, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), false, "Cho xep lop.", new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Pending", 15800000m, null, null, new Guid("11111111-1111-1111-1111-111111111111") }
                });

            migrationBuilder.InsertData(
                table: "Exams",
                columns: new[] { "Id", "CourseId", "CreatedAt", "CreatedBy", "DurationMinutes", "ExamBatchId", "ExamDate", "ExamName", "ExamType", "IsDeleted", "PassScore", "Status", "TotalScore", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("99999999-9999-9999-9999-999999999991"), new Guid("44444444-4444-4444-4444-444444444441"), new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 25, new Guid("88888888-8888-8888-8888-888888888881"), new DateTime(2026, 5, 5, 1, 0, 0, 0, DateTimeKind.Utc), "Thi Ly Thuyet B2", "Theory", false, 32, "Scheduled", 35, new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("99999999-9999-9999-9999-999999999992"), new Guid("44444444-4444-4444-4444-444444444442"), new DateTime(2026, 1, 10, 10, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 40, new Guid("88888888-8888-8888-8888-888888888882"), new DateTime(2026, 6, 8, 2, 0, 0, 0, DateTimeKind.Utc), "Thi Thuc Hanh C", "Practice", false, 80, "Draft", 100, null, null }
                });

            migrationBuilder.InsertData(
                table: "ReferralRegistrations",
                columns: new[] { "Id", "ReferralCodeId", "RegisteredAt", "StudentId" },
                values: new object[,]
                {
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1"), new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb4"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2"), new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") }
                });

            migrationBuilder.InsertData(
                table: "Terms",
                columns: new[] { "Id", "CourseId", "CreatedAt", "CreatedBy", "CurrentStudents", "EndDate", "IsDeleted", "MaxStudents", "StartDate", "TermName", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("55555555-5555-5555-5555-555555555551"), new Guid("44444444-4444-4444-4444-444444444441"), new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 1, new DateTime(2026, 5, 31, 0, 0, 0, 0, DateTimeKind.Utc), false, 25, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dot 03-2026 B2", new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("55555555-5555-5555-5555-555555555552"), new Guid("44444444-4444-4444-4444-444444444442"), new DateTime(2026, 1, 10, 8, 25, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 1, new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), false, 20, new DateTime(2026, 4, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Dot 04-2026 C", new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") }
                });

            migrationBuilder.InsertData(
                table: "Classes",
                columns: new[] { "Id", "ClassName", "CreatedAt", "CreatedBy", "CurrentStudents", "InstructorId", "IsDeleted", "MaxStudents", "Status", "TermId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("66666666-6666-6666-6666-666666666661"), "B2-01-Q1", new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 1, new Guid("11111111-1111-1111-1111-111111111111"), false, 25, "InProgress", new Guid("55555555-5555-5555-5555-555555555551"), new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("66666666-6666-6666-6666-666666666662"), "C-01-TD", new DateTime(2026, 1, 10, 8, 35, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 1, new Guid("11111111-1111-1111-1111-111111111111"), false, 20, "Pending", new Guid("55555555-5555-5555-5555-555555555552"), new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") }
                });

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
                columns: new[] { "Id", "ClassId", "CreatedAt", "CreatedBy", "EndTime", "InstructorId", "IsDeleted", "Location", "StartTime", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("77777777-7777-7777-7777-777777777771"), new Guid("66666666-6666-6666-6666-666666666661"), new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 3, 15, 3, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), false, "San tap Trung tam Quan 1", new DateTime(2026, 3, 15, 1, 0, 0, 0, DateTimeKind.Utc), null, null },
                    { new Guid("77777777-7777-7777-7777-777777777772"), new Guid("66666666-6666-6666-6666-666666666662"), new DateTime(2026, 1, 10, 8, 40, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 4, 20, 3, 30, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), false, "Bai tap Thu Duc", new DateTime(2026, 4, 20, 1, 30, 0, 0, DateTimeKind.Utc), null, null }
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
                table: "StudentEvaluations",
                columns: new[] { "Id", "ClassId", "CreatedAt", "CreatedBy", "EvaluationDate", "InstructorId", "IsDeleted", "Note", "PunctualityScore", "SkillLevel", "StudentId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("cccccccc-cccc-cccc-cccc-ccccccccccc7"), new Guid("66666666-6666-6666-6666-666666666661"), new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 3, 20, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), false, "Hoc vien tien bo tot trong sa hinh.", 9, 8, new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("cccccccc-cccc-cccc-cccc-ccccccccccc8"), new Guid("66666666-6666-6666-6666-666666666662"), new DateTime(2026, 1, 10, 14, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 4, 25, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), false, "Can tang so buoi luyen tap ngoai duong truong.", 7, 6, new Guid("11111111-1111-1111-1111-111111111111"), null, null }
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropIndex(
                name: "IX_StudentEvaluations_ClassId",
                table: "StudentEvaluations");

            migrationBuilder.DropIndex(
                name: "IX_StudentEvaluations_InstructorId",
                table: "StudentEvaluations");

            migrationBuilder.DropIndex(
                name: "IX_StudentEvaluations_StudentId",
                table: "StudentEvaluations");

            migrationBuilder.DropIndex(
                name: "IX_SampleExamResults_StudentId",
                table: "SampleExamResults");

            migrationBuilder.DeleteData(
                table: "Attendance",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-ccccccccccc9"));

            migrationBuilder.DeleteData(
                table: "Attendance",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-ccccccccccca"));

            migrationBuilder.DeleteData(
                table: "ClassStudents",
                keyColumns: new[] { "ClassId", "StudentId" },
                keyValues: new object[] { new Guid("66666666-6666-6666-6666-666666666661"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "ClassStudents",
                keyColumns: new[] { "ClassId", "StudentId" },
                keyValues: new object[] { new Guid("66666666-6666-6666-6666-666666666662"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "CollaboratorCommissions",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb5"));

            migrationBuilder.DeleteData(
                table: "CollaboratorCommissions",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb6"));

            migrationBuilder.DeleteData(
                table: "CourseRegistrations",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-ddddddddddd1"));

            migrationBuilder.DeleteData(
                table: "CourseRegistrations",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-ddddddddddd2"));

            migrationBuilder.DeleteData(
                table: "Documents",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-ccccccccccc1"));

            migrationBuilder.DeleteData(
                table: "Documents",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-ccccccccccc2"));

            migrationBuilder.DeleteData(
                table: "ExamRegistrations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"));

            migrationBuilder.DeleteData(
                table: "ExamRegistrations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"));

            migrationBuilder.DeleteData(
                table: "ExamResults",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"));

            migrationBuilder.DeleteData(
                table: "ExamResults",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4"));

            migrationBuilder.DeleteData(
                table: "InstructorLeaveRequests",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-ccccccccccc5"));

            migrationBuilder.DeleteData(
                table: "InstructorLeaveRequests",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-ccccccccccc6"));

            migrationBuilder.DeleteData(
                table: "ReferralRegistrations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3"));

            migrationBuilder.DeleteData(
                table: "ReferralRegistrations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb4"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "SampleExamResults",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-ddddddddddd3"));

            migrationBuilder.DeleteData(
                table: "SampleExamResults",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-ddddddddddd4"));

            migrationBuilder.DeleteData(
                table: "StudentDrivingDistances",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-ccccccccccc3"));

            migrationBuilder.DeleteData(
                table: "StudentDrivingDistances",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-ccccccccccc4"));

            migrationBuilder.DeleteData(
                table: "StudentEvaluations",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-ccccccccccc7"));

            migrationBuilder.DeleteData(
                table: "StudentEvaluations",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-ccccccccccc8"));

            migrationBuilder.DeleteData(
                table: "UserCenters",
                keyColumns: new[] { "CenterId", "UserId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333331"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "UserCenters",
                keyColumns: new[] { "CenterId", "UserId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333332"), new Guid("22222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "ClassSchedules",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777771"));

            migrationBuilder.DeleteData(
                table: "ClassSchedules",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777772"));

            migrationBuilder.DeleteData(
                table: "Exams",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999991"));

            migrationBuilder.DeleteData(
                table: "Exams",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999992"));

            migrationBuilder.DeleteData(
                table: "ReferralCodes",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1"));

            migrationBuilder.DeleteData(
                table: "ReferralCodes",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666661"));

            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666662"));

            migrationBuilder.DeleteData(
                table: "ExamBatches",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888881"));

            migrationBuilder.DeleteData(
                table: "ExamBatches",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888882"));

            migrationBuilder.DeleteData(
                table: "Terms",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555551"));

            migrationBuilder.DeleteData(
                table: "Terms",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555552"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444441"));

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444442"));

            migrationBuilder.DeleteData(
                table: "Centers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333331"));

            migrationBuilder.DeleteData(
                table: "Centers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333332"));
        }
    }
}
