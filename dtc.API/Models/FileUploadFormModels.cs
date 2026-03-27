using Microsoft.AspNetCore.Http;

namespace dtc.API.Models
{
    public class DocumentUploadFormRequest
    {
        public IFormFile File { get; set; } = default!;
        public string ResourceType { get; set; } = "raw";
    }

    public class PublicFileUploadFormRequest
    {
        public IFormFile File { get; set; } = default!;
        public string Folder { get; set; } = "public_assets";
        public string ResourceType { get; set; } = "image";
    }
}
