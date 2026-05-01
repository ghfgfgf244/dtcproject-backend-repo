using dtc.API.Models;
using dtc.API.Utilities;
using dtc.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq;

namespace dtc.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : BaseApiController
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IDistributedCache _cache;

        private const int UploadRateLimit = 20;
        private static readonly TimeSpan UploadRateWindow = TimeSpan.FromMinutes(15);
        private const long MaxPublicUploadBytes = 15 * 1024 * 1024;

        public FileController(
            ICloudinaryService cloudinaryService,
            IDistributedCache cache)
        {
            _cloudinaryService = cloudinaryService;
            _cache = cache;
        }

        [HttpPost("upload-public")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadPublic([FromForm] PublicFileUploadFormRequest request)
        {
            var rateLimitKey = $"upload-public:{GetRequestFingerprint()}";
            if (await DistributedRequestRateLimiter.IsLimitedAsync(_cache, rateLimitKey, UploadRateLimit, UploadRateWindow))
            {
                return StatusCode(429, new
                {
                    success = false,
                    message = "Bạn tải tệp lên quá nhanh. Vui lòng chờ rồi thử lại."
                });
            }

            var file = request.File;
            var folder = request.Folder;
            var resourceType = request.ResourceType;

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            if (file.Length > MaxPublicUploadBytes)
            {
                return BadRequest("Kích thước tệp vượt quá giới hạn 15MB.");
            }

            try
            {
                using var stream = file.OpenReadStream();
                var (publicId, version) = await _cloudinaryService.UploadAsync(
                    stream,
                    file.FileName,
                    folder,
                    resourceType);

                var url = _cloudinaryService.GetUrl(publicId, version, resourceType, isSecure: false);

                return Ok(new
                {
                    Url = url,
                    PublicId = publicId,
                    Version = version,
                    FileName = file.FileName
                });
            }
            catch (Exception)
            {
                return Fail("Không thể tải tệp lên lúc này. Vui lòng thử lại sau.");
            }
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
