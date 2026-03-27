using dtc.Application.Interfaces;
using dtc.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : BaseApiController
    {
        private readonly ICloudinaryService _cloudinaryService;

        public FileController(ICloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        /// <summary>
        /// Uploads a file for public access (e.g. Question images, Learning resources)
        /// </summary>
        [HttpPost("upload-public")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadPublic([FromForm] PublicFileUploadFormRequest request)
        {
            var file = request.File;
            var folder = request.Folder;
            var resourceType = request.ResourceType;
            if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

            try
            {
                using var stream = file.OpenReadStream();
                var (publicId, version) = await _cloudinaryService.UploadAsync(
                    stream, 
                    file.FileName, 
                    folder, 
                    resourceType);

                // For public assets, we return the full URL directly
                var url = _cloudinaryService.GetUrl(publicId, version, resourceType, isSecure: false);

                return Ok(new { 
                    Url = url, 
                    PublicId = publicId, 
                    Version = version,
                    FileName = file.FileName 
                });
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }
    }
}
