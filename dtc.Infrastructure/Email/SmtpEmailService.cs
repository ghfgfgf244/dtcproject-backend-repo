using System.Net;
using System.Net.Mail;
using dtc.Application.Features.Email.DTOs;
using dtc.Application.Features.Email.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace dtc.Infrastructure.Email
{
    public class SmtpEmailService : IEmailService
    {
        private readonly SmtpSettings _settings;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(
            IOptions<SmtpSettings> options,
            ILogger<SmtpEmailService> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public async Task SendAsync(SendEmailRequestDto request)
        {
            ValidateSettings();

            using var client = BuildClient();
            using var message = BuildMessage(
                request.ToEmail,
                request.ToName,
                request.Subject,
                request.Body,
                request.IsHtml,
                request.ReplyToEmail,
                request.ReplyToName);

            _logger.LogInformation(
                "Sending SMTP email to {ToEmail} with subject {Subject} via {SmtpServer}:{Port}",
                request.ToEmail,
                request.Subject,
                _settings.SmtpServer,
                _settings.Port);

            await client.SendMailAsync(message);
        }

        public async Task SendBulkAsync(SendBulkEmailRequestDto request)
        {
            ValidateSettings();

            using var client = BuildClient();

            var tasks = request.ToEmails.Select(async email =>
            {
                using var message = BuildMessage(email, string.Empty, request.Subject, request.Body, request.IsHtml);
                await client.SendMailAsync(message);
            });

            await Task.WhenAll(tasks);
        }

        public Task SendPasswordResetAsync(string toEmail, string toName, string resetLink)
        {
            var body = $@"
<h2>Đặt lại mật khẩu</h2>
<p>Xin chào <b>{EscapeHtml(toName)}</b>,</p>
<p>Nhấn vào liên kết dưới đây để đặt lại mật khẩu của bạn. Liên kết có hiệu lực trong <b>15 phút</b>:</p>
<p><a href=""{resetLink}"">Đặt lại mật khẩu</a></p>
<p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>
<hr/><p style=""font-size:12px;color:gray;"">© {GetSenderName()}</p>";

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
            string toEmail,
            string toName,
            string courseName,
            string centerName)
        {
            var body = $@"
<h2>Xác nhận đăng ký khóa học</h2>
<p>Xin chào <b>{EscapeHtml(toName)}</b>,</p>
<p>Bạn đã đăng ký thành công khóa học <b>{EscapeHtml(courseName)}</b> tại trung tâm <b>{EscapeHtml(centerName)}</b>.</p>
<p>Vui lòng đến cơ sở để hoàn tất thủ tục và thanh toán học phí.</p>
<hr/><p style=""font-size:12px;color:gray;"">© {GetSenderName()}</p>";

            return SendAsync(new SendEmailRequestDto
            {
                ToEmail = toEmail,
                ToName = toName,
                Subject = $"Xác nhận đăng ký khóa học - {courseName}",
                Body = body,
                IsHtml = true
            });
        }

        public Task SendExamResultAsync(
            string toEmail,
            string toName,
            string examName,
            double score,
            bool isPassed)
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
<hr/><p style=""font-size:12px;color:gray;"">© {GetSenderName()}</p>";

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
            string toEmail,
            string toName,
            DateTime leaveDate,
            bool isApproved)
        {
            var statusText = isApproved ? "ĐƯỢC CHẤP THUẬN" : "BỊ TỪ CHỐI";
            var statusColor = isApproved ? "green" : "red";

            var body = $@"
<h2>Cập nhật đơn xin nghỉ phép</h2>
<p>Xin chào <b>{EscapeHtml(toName)}</b>,</p>
<p>Đơn xin nghỉ phép ngày <b>{leaveDate:dd/MM/yyyy}</b> của bạn đã
   <b style=""color:{statusColor}"">{statusText}</b>.</p>
<hr/><p style=""font-size:12px;color:gray;"">© {GetSenderName()}</p>";

            return SendAsync(new SendEmailRequestDto
            {
                ToEmail = toEmail,
                ToName = toName,
                Subject = "Cập nhật đơn xin nghỉ phép",
                Body = body,
                IsHtml = true
            });
        }

        private SmtpClient BuildClient()
        {
            return new SmtpClient(_settings.SmtpServer, _settings.Port)
            {
                EnableSsl = _settings.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_settings.FromEmail, _settings.Password)
            };
        }

        private MailMessage BuildMessage(
            string toEmail,
            string? toName,
            string subject,
            string body,
            bool isHtml,
            string? replyToEmail = null,
            string? replyToName = null)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, GetSenderName()),
                Sender = new MailAddress(_settings.FromEmail, GetSenderName()),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            message.ReplyToList.Add(new MailAddress(
                string.IsNullOrWhiteSpace(replyToEmail) ? GetReplyToEmail() : replyToEmail,
                string.IsNullOrWhiteSpace(replyToName) ? GetReplyToName() : replyToName));
            message.To.Add(new MailAddress(toEmail, toName ?? string.Empty));
            return message;
        }

        private string GetSenderName() =>
            string.IsNullOrWhiteSpace(_settings.SenderName)
                ? _settings.FromEmail
                : _settings.SenderName;

        private string GetReplyToEmail() =>
            string.IsNullOrWhiteSpace(_settings.ReplyToEmail)
                ? _settings.FromEmail
                : _settings.ReplyToEmail!;

        private string GetReplyToName() =>
            string.IsNullOrWhiteSpace(_settings.ReplyToName)
                ? GetSenderName()
                : _settings.ReplyToName!;

        private void ValidateSettings()
        {
            if (string.IsNullOrWhiteSpace(_settings.FromEmail) ||
                string.IsNullOrWhiteSpace(_settings.Password) ||
                string.IsNullOrWhiteSpace(_settings.SmtpServer))
            {
                throw new InvalidOperationException(
                    "EmailSettings chưa được cấu hình đầy đủ. Hãy kiểm tra FromEmail, Password và SmtpServer trong appsettings.");
            }
        }

        private static string EscapeHtml(string? value) =>
            System.Web.HttpUtility.HtmlEncode(value ?? string.Empty);
    }
}
