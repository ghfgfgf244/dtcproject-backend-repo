using System.Net.Http.Json;
using dtc.Application.Features.Email.DTOs;
using dtc.Application.Features.Email.Interfaces;
using Microsoft.Extensions.Options;

namespace dtc.Infrastructure.Email
{
    /// <summary>
    /// Gửi email thông qua Resend API (https://resend.com).
    /// Thay thế SmtpEmailService, giữ nguyên contract IEmailService.
    /// </summary>
    public class ResendEmailService : IEmailService
    {
        private readonly ResendSettings _settings;
        private readonly IHttpClientFactory _httpClientFactory;

        private const string ResendApiUrl = "https://api.resend.com/emails";

        public ResendEmailService(
            IOptions<ResendSettings> options,
            IHttpClientFactory httpClientFactory)
        {
            _settings = options.Value;
            _httpClientFactory = httpClientFactory;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Core send helpers
        // ─────────────────────────────────────────────────────────────────────

        public async Task SendAsync(SendEmailRequestDto request)
        {
            var client = _httpClientFactory.CreateClient("Resend");

            var payload = new
            {
                from = $"{_settings.SenderName} <{_settings.SenderEmail}>",
                to = new[] { string.IsNullOrWhiteSpace(request.ToName)
                    ? request.ToEmail
                    : $"{request.ToName} <{request.ToEmail}>" },
                subject = request.Subject,
                html = request.Body
            };

            var response = await client.PostAsJsonAsync(ResendApiUrl, payload);
            response.EnsureSuccessStatusCode();
        }

        public async Task SendBulkAsync(SendBulkEmailRequestDto request)
        {
            var tasks = request.ToEmails.Select(email =>
                SendAsync(new SendEmailRequestDto
                {
                    ToEmail = email,
                    Subject = request.Subject,
                    Body = request.Body,
                    IsHtml = request.IsHtml
                }));

            await Task.WhenAll(tasks);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Domain-specific helpers (HTML templates giữ nguyên từ SmtpEmailService)
        // ─────────────────────────────────────────────────────────────────────

        public Task SendPasswordResetAsync(string toEmail, string toName, string resetLink)
        {
            var body = $@"
<h2>Đặt lại mật khẩu</h2>
<p>Xin chào <b>{EscapeHtml(toName)}</b>,</p>
<p>Nhấn vào liên kết dưới đây để đặt lại mật khẩu của bạn. Liên kết có hiệu lực trong <b>15 phút</b>:</p>
<p><a href=""{resetLink}"">Đặt lại mật khẩu</a></p>
<p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>
<hr/><p style=""font-size:12px;color:gray;"">© {_settings.SenderName}</p>";

            return SendAsync(new SendEmailRequestDto
            {
                ToEmail = toEmail,
                ToName = toName,
                Subject = "Đặt lại mật khẩu - DTC",
                Body = body,
                IsHtml = true
            });
        }

        public Task SendCourseRegistrationConfirmationAsync(
            string toEmail, string toName, string courseName, string centerName)
        {
            var body = $@"
<h2>Xác nhận đăng ký khoá học</h2>
<p>Xin chào <b>{EscapeHtml(toName)}</b>,</p>
<p>Bạn đã đăng ký thành công khoá học <b>{EscapeHtml(courseName)}</b> tại trung tâm <b>{EscapeHtml(centerName)}</b>.</p>
<p>Vui lòng đến cơ sở để hoàn tất thủ tục và thanh toán học phí.</p>
<hr/><p style=""font-size:12px;color:gray;"">© {_settings.SenderName}</p>";

            return SendAsync(new SendEmailRequestDto
            {
                ToEmail = toEmail,
                ToName = toName,
                Subject = $"Xác nhận đăng ký khoá học - {courseName}",
                Body = body,
                IsHtml = true
            });
        }

        public Task SendExamResultAsync(
            string toEmail, string toName, string examName, double score, bool isPassed)
        {
            var statusText = isPassed ? "ĐẠT" : "KHÔNG ĐẠT";
            var statusColor = isPassed ? "green" : "red";

            var body = $@"
<h2>Kết quả kỳ thi</h2>
<p>Xin chào <b>{EscapeHtml(toName)}</b>,</p>
<p>Kết quả kỳ thi <b>{EscapeHtml(examName)}</b> của bạn:</p>
<table border=""1"" cellpadding=""8"" style=""border-collapse:collapse;"">
  <tr><td>Điểm</td><td><b>{score:0.##}</b></td></tr>
  <tr><td>Kết quả</td><td><b style=""color:{statusColor}"">{statusText}</b></td></tr>
</table>
<hr/><p style=""font-size:12px;color:gray;"">© {_settings.SenderName}</p>";

            return SendAsync(new SendEmailRequestDto
            {
                ToEmail = toEmail,
                ToName = toName,
                Subject = $"Kết quả kỳ thi - {examName}",
                Body = body,
                IsHtml = true
            });
        }

        public Task SendLeaveRequestStatusAsync(
            string toEmail, string toName, DateTime leaveDate, bool isApproved)
        {
            var statusText = isApproved ? "ĐƯỢC CHẤP THUẬN" : "BỊ TỪ CHỐI";
            var statusColor = isApproved ? "green" : "red";

            var body = $@"
<h2>Cập nhật đơn xin nghỉ phép</h2>
<p>Xin chào <b>{EscapeHtml(toName)}</b>,</p>
<p>Đơn xin nghỉ phép ngày <b>{leaveDate:dd/MM/yyyy}</b> của bạn đã
   <b style=""color:{statusColor}"">{statusText}</b>.</p>
<hr/><p style=""font-size:12px;color:gray;"">© {_settings.SenderName}</p>";

            return SendAsync(new SendEmailRequestDto
            {
                ToEmail = toEmail,
                ToName = toName,
                Subject = "Cập nhật đơn xin nghỉ phép",
                Body = body,
                IsHtml = true
            });
        }

        // ─────────────────────────────────────────────────────────────────────
        // Private
        // ─────────────────────────────────────────────────────────────────────

        private static string EscapeHtml(string? value)
            => System.Web.HttpUtility.HtmlEncode(value ?? string.Empty);
    }
}
