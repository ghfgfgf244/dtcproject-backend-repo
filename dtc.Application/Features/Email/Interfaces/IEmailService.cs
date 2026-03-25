using dtc.Application.Features.Email.DTOs;

namespace dtc.Application.Features.Email.Interfaces
{
    public interface IEmailService
    {
        /// <summary>Gửi một email đơn.</summary>
        Task SendAsync(SendEmailRequestDto request);

        /// <summary>Gửi email cho nhiều người cùng một lúc.</summary>
        Task SendBulkAsync(SendBulkEmailRequestDto request);

        /// <summary>Gửi email đặt lại mật khẩu.</summary>
        Task SendPasswordResetAsync(string toEmail, string toName, string resetLink);

        /// <summary>Gửi email xác nhận đăng ký khoá học.</summary>
        Task SendCourseRegistrationConfirmationAsync(string toEmail, string toName, string courseName, string centerName);

        /// <summary>Gửi email thông báo kết quả kỳ thi.</summary>
        Task SendExamResultAsync(string toEmail, string toName, string examName, double score, bool isPassed);

        /// <summary>Gửi email thông báo trạng thái đơn xin nghỉ phép cho giáo viên.</summary>
        Task SendLeaveRequestStatusAsync(string toEmail, string toName, DateTime leaveDate, bool isApproved);
    }
}
