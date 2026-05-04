using dtc.Application.Features.Blogs.DTOs;
using dtc.Application.Features.Blogs.Interfaces;
using dtc.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    public class BlogController : BaseApiController
    {
        private readonly IBlogService _blogService;
        private readonly ICloudinaryService _cloudinaryService;

        public BlogController(IBlogService blogService, ICloudinaryService cloudinaryService)
        {
            _blogService = blogService;
            _cloudinaryService = cloudinaryService;
        }

        // GET /api/Blog - Danh sách bài post (có thể lọc chỉ bài đã publish)
        [HttpGet]
        public async Task<IActionResult> GetAllBlogs([FromQuery] bool onlyPublished = false)
        {
            var response = await _blogService.GetAllBlogsAsync(onlyPublished);
            return Ok(response);
        }

        // GET /api/Blog/{id} - Xem chi tiết bài post
        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedBlogs([FromQuery] BlogPagedQueryDto query)
        {
            var response = await _blogService.GetBlogsPagedAsync(query);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogById(Guid id)
        {
            try
            {
                var response = await _blogService.GetBlogByIdAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST /api/Blog - Tạo bài post mới
        [HttpPost]
        [Authorize(Roles = "Admin,TrainingManager,Collaborator,EnrollmentManager")]
        public async Task<IActionResult> CreateBlog([FromBody] CreateBlogRequestDto request)
        {
            var adminId = await GetInternalUserIdAsync();
            try
            {
                var response = await _blogService.CreateBlogAsync(request, adminId);
                return Created(response, "Blog created successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        // PUT /api/Blog/{id} - Cập nhật bài post
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,TrainingManager,Collaborator,EnrollmentManager")]
        public async Task<IActionResult> UpdateBlog(Guid id, [FromBody] UpdateBlogRequestDto request)
        {
            var adminId = await GetInternalUserIdAsync();
            try
            {
                var response = await _blogService.UpdateBlogAsync(id, request, adminId);
                return Ok(response, "Blog updated successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        // DELETE /api/Blog/{id} - Xóa mềm bài post
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,TrainingManager,Collaborator,EnrollmentManager")]
        public async Task<IActionResult> DeleteBlog(Guid id)
        {
            var adminId = await GetInternalUserIdAsync();
            try
            {
                await _blogService.DeleteBlogAsync(id, adminId);
                return NoContent("Blog deleted successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        // PATCH /api/Blog/{id}/publish - Xuất bản bài post
        [HttpPatch("{id}/publish")]
        [Authorize(Roles = "Admin,TrainingManager,Collaborator,EnrollmentManager")]
        public async Task<IActionResult> PublishBlog(Guid id)
        {
            var adminId = await GetInternalUserIdAsync();
            try
            {
                await _blogService.PublishBlogAsync(id, adminId);
                return NoContent("Blog published.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        // PATCH /api/Blog/{id}/unpublish - Ẩn bài post
        [HttpPatch("{id}/unpublish")]
        [Authorize(Roles = "Admin,TrainingManager,Collaborator,EnrollmentManager")]
        public async Task<IActionResult> UnpublishBlog(Guid id)
        {
            var adminId = await GetInternalUserIdAsync();
            try
            {
                await _blogService.UnpublishBlogAsync(id, adminId);
                return NoContent("Blog unpublished.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        // GET /api/Blog/user/{userId} - Danh sách bài viết của 1 user
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetBlogsByUser(Guid userId)
        {
            var response = await _blogService.GetBlogsByUserAsync(userId);
            return Ok(response);
        }

        [HttpPost("upload-image")]
        [Authorize(Roles = "Admin,TrainingManager,Collaborator,EnrollmentManager")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Fail("Image file is required.");
            }

            var userId = await GetInternalUserIdAsync();
            var extension = Path.GetExtension(file.FileName);
            var generatedFileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}{extension}";

            await using var stream = file.OpenReadStream();
            var (publicId, version) = await _cloudinaryService.UploadAsync(
                stream,
                generatedFileName,
                $"blog-posts/{userId}",
                "image");

            var url = _cloudinaryService.GetUrl(publicId, version, "image");
            return Ok(new { url }, "Blog image uploaded successfully.");
        }
    }
}
