using System.Net;
using dtc.Application.Features.Email.DTOs;
using dtc.Application.Features.Email.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace dtc.API.Controllers
{
    public class EmailDiagnosticsController : BaseApiController
    {
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailDiagnosticsController> _logger;

        public EmailDiagnosticsController(
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<EmailDiagnosticsController> logger)
        {
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
        }

        public class SendTestEmailRequest
        {
            public string ToEmail { get; set; } = string.Empty;
            public string? ToName { get; set; }
            public string? Subject { get; set; }
            public string? Body { get; set; }
            public bool IsHtml { get; set; } = true;
        }

        [HttpPost("send-test")]
        public async Task<IActionResult> SendTest([FromBody] SendTestEmailRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ToEmail))
            {
                return Fail("ToEmail is required.");
            }

            var senderEmail = _configuration["EmailSettings:FromEmail"] ?? string.Empty;
            var senderName = _configuration["EmailSettings:SenderName"] ?? "DTC - Trung Tâm Đào Tạo Lái Xe";
            var smtpServer = _configuration["EmailSettings:SmtpServer"] ?? string.Empty;
            var port = _configuration["EmailSettings:Port"] ?? "587";
            var replyToEmail = _configuration["EmailSettings:ReplyToEmail"] ?? senderEmail;
            var replyToName = _configuration["EmailSettings:ReplyToName"] ?? senderName;

            var subject = string.IsNullOrWhiteSpace(request.Subject)
                ? $"[DTC] Test mail {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC"
                : request.Subject.Trim();

            var body = string.IsNullOrWhiteSpace(request.Body)
                ? BuildDefaultBody(senderName, senderEmail, smtpServer, port, replyToEmail, replyToName, request.ToEmail.Trim())
                : request.Body!;

            try
            {
                await _emailService.SendAsync(new SendEmailRequestDto
                {
                    ToEmail = request.ToEmail.Trim(),
                    ToName = request.ToName?.Trim(),
                    Subject = subject,
                    Body = body,
                    IsHtml = request.IsHtml
                });

                return Ok(new
                {
                    Sent = true,
                    Provider = "SMTP",
                    SenderEmail = senderEmail,
                    SenderName = senderName,
                    SmtpServer = smtpServer,
                    Port = port,
                    ReplyToEmail = replyToEmail,
                    ReplyToName = replyToName,
                    TargetEmail = request.ToEmail.Trim(),
                    Subject = subject,
                    TimestampUtc = DateTime.UtcNow
                }, "Test email sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Test email failed. ToEmail: {ToEmail}, SenderEmail: {SenderEmail}, ReplyToEmail: {ReplyToEmail}",
                    request.ToEmail,
                    senderEmail,
                    replyToEmail);

                return BadRequest(new
                {
                    success = false,
                    message = "Test email failed.",
                    error = ex.Message,
                    provider = "SMTP",
                    senderEmail,
                    senderName,
                    smtpServer,
                    port,
                    replyToEmail,
                    replyToName,
                    targetEmail = request.ToEmail.Trim(),
                    timestampUtc = DateTime.UtcNow
                });
            }
        }

        private static string BuildDefaultBody(
            string senderName,
            string senderEmail,
            string smtpServer,
            string port,
            string replyToEmail,
            string replyToName,
            string toEmail)
        {
            return $@"
<h2>Kiểm tra gửi mail DTC</h2>
<p>Đây là email test để kiểm tra cấu hình SMTP và địa chỉ Reply-To có hoạt động hay không.</p>
<ul>
  <li>Thời gian UTC: <b>{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}</b></li>
  <li>Người gửi cấu hình: <b>{WebUtility.HtmlEncode(senderName)} &lt;{WebUtility.HtmlEncode(senderEmail)}&gt;</b></li>
  <li>SMTP server: <b>{WebUtility.HtmlEncode(smtpServer)}:{WebUtility.HtmlEncode(port)}</b></li>
  <li>Reply-To: <b>{WebUtility.HtmlEncode(replyToName)} &lt;{WebUtility.HtmlEncode(replyToEmail)}&gt;</b></li>
  <li>Người nhận: <b>{WebUtility.HtmlEncode(toEmail)}</b></li>
</ul>
<p>Bạn có thể bấm <b>Reply</b> trực tiếp từ email này để kiểm tra thư phản hồi có quay về đúng hộp mail quản trị hay không.</p>";
        }
    }
}
