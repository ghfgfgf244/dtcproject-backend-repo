namespace dtc.Infrastructure.Email
{
    /// <summary>
    /// Cấu hình Resend để gửi email. Được đọc từ section "Resend" trong appsettings.json.
    /// </summary>
    public class ResendSettings
    {
        public const string SectionName = "Resend";

        /// <summary>API Key từ Resend Dashboard (re_...).</summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>Địa chỉ email người gửi (phải là domain đã verify trên Resend).</summary>
        public string SenderEmail { get; set; } = string.Empty;

        /// <summary>Tên hiển thị của người gửi.</summary>
        public string SenderName { get; set; } = string.Empty;
    }
}
