using System;

namespace dtc.Application.Features.Permissions.DTOs
{
    public class DocumentResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string ResourceType { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public int Size { get; set; }
        public bool IsVerified { get; set; }
    }

    public class CreateDocumentRequestDto
    {
        public int ResourceType { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public int Size { get; set; }
    }

    public class UpdateDocumentRequestDto
    {
        public string FileUrl { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public int Size { get; set; }
    }
}
