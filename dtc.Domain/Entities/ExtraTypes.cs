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
        Class = 3
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
        Practice = 2
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
        A = 'A',
        B = 'B',
        C = 'C',
        D = 'D'
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
}
