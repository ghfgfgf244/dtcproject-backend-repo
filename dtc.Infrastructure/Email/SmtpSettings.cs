namespace dtc.Infrastructure.Email
{
    public class SmtpSettings
    {
        public const string SectionName = "EmailSettings";

        public string FromEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string SmtpServer { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
        public string SenderName { get; set; } = "DTC - Trung Tâm Đào Tạo Lái Xe";
        public string? ReplyToEmail { get; set; }
        public string? ReplyToName { get; set; }
    }
}
