using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using dtc.API.Utilities;
using dtc.Application.Features.Email.DTOs;
using dtc.Application.Features.Email.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace dtc.API.Controllers
{
    public class ContactController : BaseApiController
    {
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ContactController> _logger;
        private readonly IDistributedCache _cache;

        private const int ContactRateLimit = 5;
        private static readonly TimeSpan ContactRateWindow = TimeSpan.FromMinutes(30);

        public ContactController(
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<ContactController> logger,
            IDistributedCache cache)
        {
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
            _cache = cache;
        }

        public class ContactRequest
        {
            [Required]
            [StringLength(120)]
            public string FullName { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            [StringLength(200)]
            public string Email { get; set; } = string.Empty;

            [Required]
            [StringLength(30)]
            public string PhoneNumber { get; set; } = string.Empty;

            [StringLength(150)]
            public string? Subject { get; set; }

            [Required]
            [StringLength(4000, MinimumLength = 3)]
            public string Message { get; set; } = string.Empty;
        }

        [HttpPost]
        public async Task<IActionResult> SendContact([FromBody] ContactRequest request)
        {
            var rateLimitKey = $"contact:{GetRequestFingerprint()}";
            if (await DistributedRequestRateLimiter.IsLimitedAsync(_cache, rateLimitKey, ContactRateLimit, ContactRateWindow))
            {
                return StatusCode(429, dtc.Application.Common.ApiResponse<object?>.Fail("Bạn gửi liên hệ quá nhanh. Vui lòng thử lại sau ít phút."));
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Dữ liệu không hợp lệ." : e.ErrorMessage)
                    .ToArray();

                return BadRequest(dtc.Application.Common.ApiResponse<object?>.Fail(errors));
            }

            var systemEmail = _configuration["EmailSettings:FromEmail"];
            if (string.IsNullOrWhiteSpace(systemEmail))
            {
                return Fail("Email hệ thống chưa được cấu hình.");
            }

            var subject = string.IsNullOrWhiteSpace(request.Subject)
                ? $"[Liên hệ website] {request.FullName}"
                : $"[Liên hệ website] {request.Subject!.Trim()}";

            var body = BuildContactEmailBody(request);

            try
            {
                await _emailService.SendAsync(new SendEmailRequestDto
                {
                    ToEmail = systemEmail,
                    ToName = _configuration["EmailSettings:SenderName"] ?? "DTC - Trung Tâm Đào Tạo Lái Xe",
                    Subject = subject,
                    Body = body,
                    IsHtml = true,
                    ReplyToEmail = request.Email.Trim(),
                    ReplyToName = request.FullName.Trim()
                });

                return Ok(new
                {
                    sent = true,
                    targetEmail = systemEmail,
                    replyToEmail = request.Email.Trim()
                }, "Đã gửi liên hệ thành công. Trung tâm sẽ phản hồi sớm cho bạn.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send contact message from {Email} / {PhoneNumber}",
                    request.Email,
                    request.PhoneNumber);

                return Fail("Không thể gửi liên hệ lúc này. Vui lòng thử lại sau.");
            }
        }

        private static string BuildContactEmailBody(ContactRequest request)
        {
            var builder = new StringBuilder();
            builder.AppendLine("<div style=\"font-family:Arial,Helvetica,sans-serif;color:#1f2937;line-height:1.6;\">");
            builder.AppendLine("<h2 style=\"margin:0 0 16px;font-size:22px;color:#0f172a;\">Liên hệ mới từ khách hàng</h2>");
            builder.AppendLine("<table cellpadding=\"0\" cellspacing=\"0\" style=\"width:100%;max-width:680px;border-collapse:separate;border-spacing:0 10px;\">");
            builder.AppendLine($"<tr><td style=\"width:140px;color:#64748b;font-weight:600;vertical-align:top;\">Họ và tên</td><td style=\"color:#0f172a;\">{WebUtility.HtmlEncode(request.FullName.Trim())}</td></tr>");
            builder.AppendLine($"<tr><td style=\"color:#64748b;font-weight:600;vertical-align:top;\">Email</td><td style=\"color:#0f172a;\">{WebUtility.HtmlEncode(request.Email.Trim())}</td></tr>");
            builder.AppendLine($"<tr><td style=\"color:#64748b;font-weight:600;vertical-align:top;\">Số điện thoại</td><td style=\"color:#0f172a;\">{WebUtility.HtmlEncode(request.PhoneNumber.Trim())}</td></tr>");
            builder.AppendLine($"<tr><td style=\"color:#64748b;font-weight:600;vertical-align:top;\">Chủ đề</td><td style=\"color:#0f172a;\">{WebUtility.HtmlEncode((request.Subject ?? "Liên hệ chung").Trim())}</td></tr>");
            builder.AppendLine("</table>");
            builder.AppendLine("<div style=\"margin-top:20px;\">");
            builder.AppendLine("<div style=\"margin-bottom:8px;color:#64748b;font-weight:600;\">Nội dung</div>");
            builder.AppendLine($"<div style=\"padding:16px;border:1px solid #e2e8f0;border-radius:12px;background:#f8fafc;white-space:pre-wrap;color:#0f172a;\">{WebUtility.HtmlEncode(request.Message.Trim())}</div>");
            builder.AppendLine("</div>");
            builder.AppendLine("</div>");
            return builder.ToString();
        }

        private string GetRequestFingerprint()
        {
            var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
}
