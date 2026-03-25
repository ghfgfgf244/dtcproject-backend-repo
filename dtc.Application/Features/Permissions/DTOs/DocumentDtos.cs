using System;

namespace dtc.Application.Features.Permissions.DTOs
{
    public class DocumentResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string ResourceType { get; set; } = string.Empty; // image, raw, video
        public string ProviderPublicId { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public int Size { get; set; }
        public bool IsVerified { get; set; }
        public string FileUrl { get; set; } = string.Empty;
    }

    public class CreateDocumentRequestDto
    {
        public string ResourceType { get; set; } = "raw";
        public string ProviderPublicId { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public int Size { get; set; }
    }

    public class UpdateDocumentRequestDto
    {
        public string ResourceType { get; set; } = "raw";
        public string ProviderPublicId { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public int Size { get; set; }
    }
}
