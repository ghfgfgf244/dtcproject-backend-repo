using System;
using dtc.Domain.Entities;
using EmailVO = dtc.Domain.ValueObjects.Email;
using PhoneNumberVO = dtc.Domain.ValueObjects.PhoneNumber;

namespace dtc.Infrastructure.Persistence.Seeding
{
    internal static class SqlSeedData
    {
        private static readonly DateTime BaseCreatedAt = new(2026, 1, 10, 8, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime UpdatedAt = new(2026, 1, 12, 8, 0, 0, DateTimeKind.Utc);

        public static object[] Roles =>
        [
            new
            {
                Id = (int)UserRole.Instructor,
                RoleName = UserRole.Instructor
            },
            new
            {
                Id = (int)UserRole.Student,
                RoleName = UserRole.Student
            }
        ];

        public static object[] Users =>
        [
            new
            {
                Id = SeedIds.UserA,
                ClerkId = "clerk_instructor_demo",
                Email = EmailVO.Create("instructor.demo@dtc.local"),
                FullName = "Tran Van Huong",
                Phone = PhoneNumberVO.Create("+84901111222"),
                AvatarUrl = "https://cdn.example.com/avatar/instructor-demo.png",
                IsActive = true,
                LastLoginAt = new DateTime(2026, 1, 15, 9, 0, 0, DateTimeKind.Utc),
                RoleId = UserRole.Instructor,
                CreatedAt = BaseCreatedAt,
                CreatedBy = (Guid?)null,
                UpdatedAt = (DateTime?)UpdatedAt,
                UpdatedBy = (Guid?)SeedIds.UserA,
                IsDeleted = false
            },
            new
            {
                Id = SeedIds.UserB,
                ClerkId = "clerk_student_demo",
                Email = EmailVO.Create("student.demo@dtc.local"),
                FullName = "Le Thi Minh",
                Phone = PhoneNumberVO.Create("+84903333444"),
                AvatarUrl = "https://cdn.example.com/avatar/student-demo.png",
                IsActive = true,
                LastLoginAt = new DateTime(2026, 1, 16, 9, 0, 0, DateTimeKind.Utc),
                RoleId = UserRole.Student,
                CreatedAt = BaseCreatedAt.AddMinutes(10),
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)UpdatedAt,
                UpdatedBy = (Guid?)SeedIds.UserA,
                IsDeleted = false
            }
        ];

        public static object[] Centers =>
        [
            new
            {
                Id = SeedIds.CenterA,
                CenterName = "Trung Tam Lai Xe Quan 1",
                Address = "12 Nguyen Hue, Quan 1",
                Phone = PhoneNumberVO.Create("+842812345678"),
                Email = EmailVO.Create("q1-center@dtc.local"),
                IsActive = true,
                NumberOfClasses = 6,
                MaxStudentPerClass = 25,
                CreatedAt = BaseCreatedAt,
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)UpdatedAt,
                UpdatedBy = (Guid?)SeedIds.UserA,
                IsDeleted = false
            },
            new
            {
                Id = SeedIds.CenterB,
                CenterName = "Trung Tam Lai Xe Thu Duc",
                Address = "99 Vo Van Ngan, Thu Duc",
                Phone = PhoneNumberVO.Create("+842876543210"),
                Email = EmailVO.Create("thuduc-center@dtc.local"),
                IsActive = true,
                NumberOfClasses = 8,
                MaxStudentPerClass = 30,
                CreatedAt = BaseCreatedAt.AddMinutes(5),
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)UpdatedAt,
                UpdatedBy = (Guid?)SeedIds.UserA,
                IsDeleted = false
            }
        ];

        public static object[] UserCenters =>
        [
            new { UserId = SeedIds.UserA, CenterId = SeedIds.CenterA },
            new { UserId = SeedIds.UserB, CenterId = SeedIds.CenterB }
        ];

        public static object[] Courses =>
        [
            new
            {
                Id = SeedIds.CourseA,
                CenterId = SeedIds.CenterA,
                CourseName = "Khoa B2 Tieu Chuan",
                LicenseType = ExamLevel.B,
                DurationInWeeks = 12,
                MaxStudents = 25,
                ThumbnailUrl = "https://cdn.example.com/course/b2.jpg",
                Description = "Khoa hoc B2 danh cho hoc vien moi.",
                Price = 12500000m,
                IsActive = true,
                CreatedAt = BaseCreatedAt,
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)UpdatedAt,
                UpdatedBy = (Guid?)SeedIds.UserA,
                IsDeleted = false
            },
            new
            {
                Id = SeedIds.CourseB,
                CenterId = SeedIds.CenterB,
                CourseName = "Khoa C Nang Cao",
                LicenseType = ExamLevel.C,
                DurationInWeeks = 16,
                MaxStudents = 20,
                ThumbnailUrl = "https://cdn.example.com/course/c.jpg",
                Description = "Khoa hoc C danh cho tai xe tai nhe.",
                Price = 15800000m,
                IsActive = true,
                CreatedAt = BaseCreatedAt.AddMinutes(20),
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)UpdatedAt,
                UpdatedBy = (Guid?)SeedIds.UserA,
                IsDeleted = false
            }
        ];

        public static object[] Terms =>
        [
            new
            {
                Id = SeedIds.TermA,
                CourseId = SeedIds.CourseA,
                TermName = "Dot 03-2026 B2",
                StartDate = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2026, 5, 31, 0, 0, 0, DateTimeKind.Utc),
                CurrentStudents = 1,
                MaxStudents = 25,
                IsActive = true,
                CreatedAt = BaseCreatedAt,
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)UpdatedAt,
                UpdatedBy = (Guid?)SeedIds.UserA,
                IsDeleted = false
            },
            new
            {
                Id = SeedIds.TermB,
                CourseId = SeedIds.CourseB,
                TermName = "Dot 04-2026 C",
                StartDate = new DateTime(2026, 4, 5, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2026, 7, 20, 0, 0, 0, DateTimeKind.Utc),
                CurrentStudents = 1,
                MaxStudents = 20,
                IsActive = true,
                CreatedAt = BaseCreatedAt.AddMinutes(25),
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)UpdatedAt,
                UpdatedBy = (Guid?)SeedIds.UserA,
                IsDeleted = false
            }
        ];

        public static object[] CourseRegistrations =>
        [
            new
            {
                Id = SeedIds.CourseRegistrationA,
                CourseId = SeedIds.CourseA,
                UserId = SeedIds.UserB,
                RegistrationDate = new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc),
                Status = CourseRegistrationStatus.Approved,
                TotalFee = 12500000m,
                Notes = "Da xac nhan hoc phi dot 1.",
                CreatedAt = BaseCreatedAt,
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)UpdatedAt,
                UpdatedBy = (Guid?)SeedIds.UserA,
                IsDeleted = false
            },
            new
            {
                Id = SeedIds.CourseRegistrationB,
                CourseId = SeedIds.CourseB,
                UserId = SeedIds.UserA,
                RegistrationDate = new DateTime(2026, 2, 12, 0, 0, 0, DateTimeKind.Utc),
                Status = CourseRegistrationStatus.Pending,
                TotalFee = 15800000m,
                Notes = "Cho xep lop.",
                CreatedAt = BaseCreatedAt.AddMinutes(30),
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (Guid?)null,
                IsDeleted = false
            }
        ];

        public static object[] Classes =>
        [
            new
            {
                Id = SeedIds.ClassA,
                TermId = SeedIds.TermA,
                InstructorId = SeedIds.UserA,
                ClassName = "B2-01-Q1",
                CurrentStudents = 1,
                MaxStudents = 25,
                ClassType = ClassType.Theory,
                Status = ClassStatus.InProgress,
                CreatedAt = BaseCreatedAt,
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)UpdatedAt,
                UpdatedBy = (Guid?)SeedIds.UserA,
                IsDeleted = false
            },
            new
            {
                Id = SeedIds.ClassB,
                TermId = SeedIds.TermB,
                InstructorId = SeedIds.UserA,
                ClassName = "C-01-TD",
                CurrentStudents = 1,
                MaxStudents = 20,
                ClassType = ClassType.Practice,
                Status = ClassStatus.Pending,
                CreatedAt = BaseCreatedAt.AddMinutes(35),
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)UpdatedAt,
                UpdatedBy = (Guid?)SeedIds.UserA,
                IsDeleted = false
            }
        ];

        public static object[] ClassStudents =>
        [
            new { ClassId = SeedIds.ClassA, StudentId = SeedIds.UserB },
            new { ClassId = SeedIds.ClassB, StudentId = SeedIds.UserA }
        ];

        public static object[] ClassSchedules =>
        [
            new
            {
                Id = SeedIds.ClassScheduleA,
                ClassId = SeedIds.ClassA,
                InstructorId = SeedIds.UserA,
                StartTime = new DateTime(2026, 3, 15, 1, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2026, 3, 15, 3, 0, 0, DateTimeKind.Utc),
                AddressId = 1,
                CreatedAt = BaseCreatedAt,
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (Guid?)null,
                IsDeleted = false
            },
            new
            {
                Id = SeedIds.ClassScheduleB,
                ClassId = SeedIds.ClassB,
                InstructorId = SeedIds.UserA,
                StartTime = new DateTime(2026, 4, 20, 1, 30, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2026, 4, 20, 3, 30, 0, DateTimeKind.Utc),
                AddressId = 2,
                CreatedAt = BaseCreatedAt.AddMinutes(40),
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (Guid?)null,
                IsDeleted = false
            }
        ];

        public static object[] Attendances =>
        [
            new
            {
                Id = SeedIds.AttendanceA,
                ClassScheduleId = SeedIds.ClassScheduleA,
                StudentId = SeedIds.UserB,
                IsPresent = true,
                CheckedAt = new DateTime(2026, 3, 15, 1, 5, 0, DateTimeKind.Utc),
                CreatedAt = BaseCreatedAt,
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (Guid?)null,
                IsDeleted = false
            },
            new
            {
                Id = SeedIds.AttendanceB,
                ClassScheduleId = SeedIds.ClassScheduleB,
                StudentId = SeedIds.UserA,
                IsPresent = false,
                CheckedAt = new DateTime(2026, 4, 20, 1, 35, 0, DateTimeKind.Utc),
                CreatedAt = BaseCreatedAt.AddMinutes(45),
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (Guid?)null,
                IsDeleted = false
            }
        ];

        public static object[] InstructorLeaveRequests =>
        [
            new
            {
                Id = SeedIds.InstructorLeaveRequestA,
                InstructorId = SeedIds.UserA,
                LeaveDate = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc),
                Reason = "Nghi phep ca nhan.",
                Status = LeaveRequestStatus.Approved,
                CreatedAt = BaseCreatedAt,
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)UpdatedAt,
                UpdatedBy = (Guid?)SeedIds.UserA,
                IsDeleted = false
            },
            new
            {
                Id = SeedIds.InstructorLeaveRequestB,
                InstructorId = SeedIds.UserA,
                LeaveDate = new DateTime(2026, 4, 15, 0, 0, 0, DateTimeKind.Utc),
                Reason = "Tham gia boi duong nghiep vu.",
                Status = LeaveRequestStatus.Pending,
                CreatedAt = BaseCreatedAt.AddMinutes(50),
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (Guid?)null,
                IsDeleted = false
            }
        ];

        public static object[] StudentDrivingDistances =>
        [
            new
            {
                Id = SeedIds.StudentDrivingDistanceA,
                StudentId = SeedIds.UserB,
                MorningDistanceKm = 42.5d,
                EveningDistanceKm = 18.2d,
                MaxMorningDistanceKm = 80d,
                MaxEveningDistanceKm = 40d,
                CreatedAt = BaseCreatedAt,
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)UpdatedAt,
                UpdatedBy = (Guid?)SeedIds.UserA,
                IsDeleted = false
            },
            new
            {
                Id = SeedIds.StudentDrivingDistanceB,
                StudentId = SeedIds.UserA,
                MorningDistanceKm = 12d,
                EveningDistanceKm = 6d,
                MaxMorningDistanceKm = 60d,
                MaxEveningDistanceKm = 30d,
                CreatedAt = BaseCreatedAt.AddMinutes(55),
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)UpdatedAt,
                UpdatedBy = (Guid?)SeedIds.UserA,
                IsDeleted = false
            }
        ];

        public static object[] ExamBatches =>
        [
            new
            {
                Id = SeedIds.ExamBatchA,
                BatchName = "Ky thi sat hach thang 05",
                RegistrationStartDate = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc),
                RegistrationEndDate = new DateTime(2026, 4, 20, 0, 0, 0, DateTimeKind.Utc),
                ExamStartDate = new DateTime(2026, 5, 5, 0, 0, 0, DateTimeKind.Utc),
                CurrentCandidates = 1,
                MaxCandidates = 100,
                Status = ExamBatchStatus.OpenForRegistration,
                CreatedAt = BaseCreatedAt,
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)UpdatedAt,
                UpdatedBy = (Guid?)SeedIds.UserA,
                IsDeleted = false
            },
            new
            {
                Id = SeedIds.ExamBatchB,
                BatchName = "Ky thi sat hach thang 06",
                RegistrationStartDate = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc),
                RegistrationEndDate = new DateTime(2026, 5, 22, 0, 0, 0, DateTimeKind.Utc),
                ExamStartDate = new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc),
                CurrentCandidates = 1,
                MaxCandidates = 80,
                Status = ExamBatchStatus.Pending,
                CreatedAt = BaseCreatedAt.AddHours(1),
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (Guid?)null,
                IsDeleted = false
            }
        ];

        public static object[] Exams =>
        [
            new
            {
                Id = SeedIds.ExamA,
                ExamBatchId = SeedIds.ExamBatchA,
                CourseId = SeedIds.CourseA,
                AddressId = 1,
                ExamName = "Thi Ly Thuyet B2",
                ExamDate = new DateTime(2026, 5, 5, 1, 0, 0, DateTimeKind.Utc),
                ExamType = ExamType.Theory,
                DurationMinutes = 25,
                TotalScore = 35,
                PassScore = 32,
                Status = ExamStatus.Scheduled,
                CreatedAt = BaseCreatedAt,
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)UpdatedAt,
                UpdatedBy = (Guid?)SeedIds.UserA,
                IsDeleted = false
            },
            new
            {
                Id = SeedIds.ExamB,
                ExamBatchId = SeedIds.ExamBatchB,
                CourseId = SeedIds.CourseB,
                AddressId = 2,
                ExamName = "Thi Thuc Hanh C",
                ExamDate = new DateTime(2026, 6, 8, 2, 0, 0, DateTimeKind.Utc),
                ExamType = ExamType.Practice,
                DurationMinutes = 40,
                TotalScore = 100,
                PassScore = 80,
                Status = ExamStatus.Draft,
                CreatedAt = BaseCreatedAt.AddHours(2),
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (Guid?)null,
                IsDeleted = false
            }
        ];

        public static object[] ExamRegistrations =>
        [
            new
            {
                Id = SeedIds.ExamRegistrationA,
                ExamBatchId = SeedIds.ExamBatchA,
                StudentId = SeedIds.UserB,
                RegistrationDate = new DateTime(2026, 4, 10, 0, 0, 0, DateTimeKind.Utc),
                IsPaid = true,
                Status = ExamRegistrationStatus.Approved,
                CreatedAt = BaseCreatedAt,
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)UpdatedAt,
                UpdatedBy = (Guid?)SeedIds.UserA,
                IsDeleted = false
            },
            new
            {
                Id = SeedIds.ExamRegistrationB,
                ExamBatchId = SeedIds.ExamBatchB,
                StudentId = SeedIds.UserA,
                RegistrationDate = new DateTime(2026, 5, 8, 0, 0, 0, DateTimeKind.Utc),
                IsPaid = false,
                Status = ExamRegistrationStatus.Pending,
                CreatedAt = BaseCreatedAt.AddHours(3),
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (Guid?)null,
                IsDeleted = false
            }
        ];

        public static object[] ExamResults =>
        [
            new
            {
                Id = SeedIds.ExamResultA,
                ExamId = SeedIds.ExamA,
                StudentId = SeedIds.UserB,
                AttemptNo = 1,
                Score = 33d,
                IsPassed = true
            },
            new
            {
                Id = SeedIds.ExamResultB,
                ExamId = SeedIds.ExamB,
                StudentId = SeedIds.UserA,
                AttemptNo = 1,
                Score = 78d,
                IsPassed = false
            }
        ];

        public static object[] ReferralCodes =>
        [
            new
            {
                Id = SeedIds.ReferralCodeA,
                Code = "CTVQ1A01",
                CollaboratorId = SeedIds.UserA,
                UsedCount = 1,
                IsActive = true,
                CreatedAt = BaseCreatedAt,
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)UpdatedAt,
                UpdatedBy = (Guid?)SeedIds.UserA,
                IsDeleted = false
            },
            new
            {
                Id = SeedIds.ReferralCodeB,
                Code = "CTVTDA02",
                CollaboratorId = SeedIds.UserA,
                UsedCount = 0,
                IsActive = true,
                CreatedAt = BaseCreatedAt.AddHours(4),
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (Guid?)null,
                IsDeleted = false
            }
        ];

        public static object[] ReferralRegistrations =>
        [
            new
            {
                Id = SeedIds.ReferralRegistrationA,
                ReferralCodeId = SeedIds.ReferralCodeA,
                StudentId = SeedIds.UserB,
                RegisteredAt = new DateTime(2026, 2, 11, 0, 0, 0, DateTimeKind.Utc)
            },
            new
            {
                Id = SeedIds.ReferralRegistrationB,
                ReferralCodeId = SeedIds.ReferralCodeB,
                StudentId = SeedIds.UserA,
                RegisteredAt = new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc)
            }
        ];

        public static object[] CollaboratorCommissions =>
        [
            new
            {
                Id = SeedIds.CollaboratorCommissionA,
                CollaboratorId = SeedIds.UserA,
                Amount = 800000m,
                Status = CommissionStatus.Pending,
                CreatedAt = new DateTime(2026, 2, 20, 0, 0, 0, DateTimeKind.Utc),
                PaidAt = (DateTime?)null
            },
            new
            {
                Id = SeedIds.CollaboratorCommissionB,
                CollaboratorId = SeedIds.UserA,
                Amount = 1250000m,
                Status = CommissionStatus.Paid,
                CreatedAt = new DateTime(2026, 2, 25, 0, 0, 0, DateTimeKind.Utc),
                PaidAt = (DateTime?)new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        ];

        public static object[] Documents =>
        [
            new
            {
                Id = SeedIds.DocumentA,
                UserId = SeedIds.UserB,
                ProviderPublicId = "user_docs/student-demo-cccd",
                Version = "1710000001",
                ResourceType = "raw",
                FileName = "cccd-student-demo.pdf",
                Extension = ".pdf",
                Size = 512000,
                IsVerified = true,
                CreatedAt = BaseCreatedAt,
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)UpdatedAt,
                UpdatedBy = (Guid?)SeedIds.UserA,
                IsDeleted = false
            },
            new
            {
                Id = SeedIds.DocumentB,
                UserId = SeedIds.UserA,
                ProviderPublicId = "user_docs/instructor-demo-license",
                Version = "1710000002",
                ResourceType = "image",
                FileName = "giay-phep-instructor-demo.jpg",
                Extension = ".jpg",
                Size = 348000,
                IsVerified = false,
                CreatedAt = BaseCreatedAt.AddHours(5),
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (Guid?)null,
                IsDeleted = false
            }
        ];

        public static object[] StudentEvaluations =>
        [
            new
            {
                Id = SeedIds.StudentEvaluationA,
                StudentId = SeedIds.UserB,
                InstructorId = SeedIds.UserA,
                ClassId = (Guid?)SeedIds.ClassA,
                PunctualityScore = 9,
                SkillLevel = 8,
                Note = "Hoc vien tien bo tot trong sa hinh.",
                EvaluationDate = new DateTime(2026, 3, 20, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = BaseCreatedAt,
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)UpdatedAt,
                UpdatedBy = (Guid?)SeedIds.UserA,
                IsDeleted = false
            },
            new
            {
                Id = SeedIds.StudentEvaluationB,
                StudentId = SeedIds.UserA,
                InstructorId = SeedIds.UserA,
                ClassId = (Guid?)SeedIds.ClassB,
                PunctualityScore = 7,
                SkillLevel = 6,
                Note = "Can tang so buoi luyen tap ngoai duong truong.",
                EvaluationDate = new DateTime(2026, 4, 25, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = BaseCreatedAt.AddHours(6),
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (Guid?)null,
                IsDeleted = false
            }
        ];

        public static object[] SampleExamResults =>
        [
            new
            {
                Id = SeedIds.SampleExamResultA,
                SampleExamId = SeedIds.SampleExamA,
                StudentId = SeedIds.UserB,
                TotalScore = 34d,
                DurationSeconds = 1320,
                IsPassed = true,
                UserAnswersJson = "{\"1\":\"A\",\"2\":\"C\"}",
                CreatedAt = BaseCreatedAt,
                CreatedBy = (Guid?)SeedIds.UserB,
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (Guid?)null,
                IsDeleted = false
            },
            new
            {
                Id = SeedIds.SampleExamResultB,
                SampleExamId = SeedIds.SampleExamB,
                StudentId = SeedIds.UserA,
                TotalScore = 24d,
                DurationSeconds = 1600,
                IsPassed = false,
                UserAnswersJson = "{\"1\":\"B\",\"2\":\"D\"}",
                CreatedAt = BaseCreatedAt.AddHours(7),
                CreatedBy = (Guid?)SeedIds.UserA,
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (Guid?)null,
                IsDeleted = false
            }
        ];
    }
}
