namespace dtc.Infrastructure.Email
{
    /// <summary>
    /// Cấu hình SMTP để gửi email. Được đọc từ section "Smtp" trong appsettings.json.
    /// </summary>
    public class SmtpSettings
    {
        public const string SectionName = "Smtp";

        /// <summary>Địa chỉ SMTP server. Ví dụ: smtp.gmail.com</summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>Cổng SMTP. Thường là 587 (TLS) hoặc 465 (SSL).</summary>
        public int Port { get; set; } = 587;

        /// <summary>Bật/tắt SSL (nên để true khi dùng port 587/465).</summary>
        public bool EnableSsl { get; set; } = true;

        /// <summary>Tài khoản đăng nhập SMTP (thường là email người gửi).</summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>Mật khẩu ứng dụng (App Password) của tài khoản SMTP.</summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>Địa chỉ email người gửi hiển thị.</summary>
        public string SenderEmail { get; set; } = string.Empty;

        /// <summary>Tên hiển thị của người gửi.</summary>
        public string SenderName { get; set; } = string.Empty;
    }
}
