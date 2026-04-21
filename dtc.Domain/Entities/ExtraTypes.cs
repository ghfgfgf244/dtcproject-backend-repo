using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dtc.Domain.Entities
{
    public enum NotificationType
    {
        [Description("Thông báo toàn hệ thống")]
        System = 1,

        [Description("Thông báo về kỳ thi")]
        Exam = 2,

        [Description("Thông báo lớp học")]
        Class = 3,

        [Description("Chào mừng người dùng mới")]
        Welcome = 4,

        [Description("Thay đổi vai trò")]
        RoleChanged = 5,

        [Description("Hoa hồng cộng tác viên")]
        Referral = 6,

        [Description("Điểm danh")]
        Attendance = 7,

        [Description("Kết quả thi")]
        ExamResult = 8,

        [Description("Đăng ký khóa học")]
        Registration = 9
    }
    public enum UserRole
    {
        [Description("Quản trị hệ thống")]
        Admin = 1,

        [Description("Quản lý đào tạo")]
        TrainingManager = 2,

        [Description("Giáo viên")]
        Instructor = 3,

        [Description("Quản lý tuyển sinh")]
        EnrollmentManager = 4,

        [Description("Cộng tác viên")]
        Collaborator = 5,

        [Description("Học viên")]
        Student = 6
    }
    public enum CommissionStatus
    {
        Pending = 1,
        Paid = 2,
        Cancelled = 3
    }
    public enum ExamType
    {
        Theory = 1,
        Simulation = 2,
        Practice = 3
    }
    public enum ExamStatus
    {
        Draft = 1,
        Scheduled = 2,
        Finished = 3,
        Cancelled = 4
    }
    public enum AnswerOption
    {
        A = 1,
        B = 2,
        C = 3,
        D = 4
    }
    public enum QuestionCategoryType
    {
        Theory = 1,
        Sign = 2,
        Simulation = 3
    }
    public enum ExamLevel
    {
        A1 = 1,
        A,
        B1,
        B,
        C1,
        C,
        D1,
        D2,
        D,
        BE,
        C1E,
        CE,
        D1E,
        D2E,
        DE
    }
    public enum ResourceType
    {
        Video = 1,
        Pdf,
        Link,
        Slide,
        Image
    }

    public enum CourseRegistrationStatus
    {
        [Description("Chờ duyệt")]
        Pending = 1,

        [Description("Đã duyệt")]
        Approved = 2,

        [Description("Từ chối")]
        Rejected = 3,

        [Description("Đã hủy")]
        Cancelled = 4
    }

    public enum ClassStatus
    {
        [Description("Chưa bắt đầu")]
        Pending = 1,

        [Description("Đang diễn ra")]
        InProgress = 2,

        [Description("Đã kết thúc")]
        Completed = 3,

        [Description("Đã hủy")]
        Cancelled = 4
    }

    public enum ClassType
    {
        Theory = 1,
        Practice = 2
    }

    public enum ExamBatchStatus
    {
        [Description("Chưa mở đăng ký")]
        Pending = 1,

        [Description("Đang mở đăng ký")]
        OpenForRegistration = 2,

        [Description("Đã đóng đăng ký")]
        ClosedForRegistration = 3,

        [Description("Đang diễn ra kỳ thi")]
        InProgress = 4,

        [Description("Đã kết thúc")]
        Completed = 5,

        [Description("Đã hủy")]
        Cancelled = 6
    }

    public enum ExamRegistrationStatus
    {
        [Description("Chờ duyệt")]
        Pending = 1,

        [Description("Đã duyệt")]
        Approved = 2,

        [Description("Từ chối")]
        Rejected = 3,

        [Description("Đã hủy")]
        Cancelled = 4
    }

    public enum LeaveRequestStatus
    {
        Pending,
        Approved,
        Rejected,
        Cancelled
    }

    public static class QuestionCategoryNames
    {
        public const string Theory = "Lý thuyết";
        public const string Sign = "Biển báo";
        public const string Simulation = "Sa hình";

        public static readonly string[] All =
        {
            Theory,
            Sign,
            Simulation
        };

        public static string Normalize(string? value)
        {
            var normalized = (value ?? string.Empty).Trim().ToLowerInvariant();

            return normalized switch
            {
                "ly thuyet" or "lý thuyết" or "lythuyet" or "theory" => Theory,
                "bien bao" or "biển báo" or "bienbao" or "sign" or "signs" => Sign,
                "sa hinh" or "sa hình" or "sahinh" or "simulation" => Simulation,
                _ => throw new ArgumentException("Question category is invalid.")
            };
        }
    }
}
