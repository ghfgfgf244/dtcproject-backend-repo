namespace dtc.Application.Features.Email.DTOs
{
    public class SendEmailRequestDto
    {
        public string ToEmail { get; set; } = default!;
        public string ToName { get; set; } = string.Empty;
        public string Subject { get; set; } = default!;
        public string Body { get; set; } = default!;
        public bool IsHtml { get; set; } = true;
    }

    public class SendBulkEmailRequestDto
    {
        public List<string> ToEmails { get; set; } = new();
        public string Subject { get; set; } = default!;
        public string Body { get; set; } = default!;
        public bool IsHtml { get; set; } = true;
    }
}
