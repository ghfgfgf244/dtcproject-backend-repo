using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dtc.Application.Features.Blogs.DTOs;
using dtc.Application.Features.Blogs.Interfaces;
using dtc.Domain.Entities.Blogs;
using dtc.Domain.Interfaces;

namespace dtc.Application.Features.Blogs.Services
{
    public class BlogService : IBlogService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BlogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BlogResponseDto> CreateBlogAsync(CreateBlogRequestDto request, Guid adminId)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
            if (category == null) throw new Exception("Category not found");

            var blog = new Blog(request.Title, request.CategoryId, request.Content, adminId, request.Summary, request.Avatar);
            
            await _unitOfWork.Blogs.AddAsync(blog);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(blog);
        }

        public async Task<BlogResponseDto> UpdateBlogAsync(Guid id, UpdateBlogRequestDto request, Guid adminId)
        {
            var blog = await _unitOfWork.Blogs.GetByIdAsync(id);
            if (blog == null) throw new Exception("Blog not found");

            if (request.CategoryId.HasValue)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId.Value);
                if (category == null) throw new Exception("Category not found");
            }

            blog.Update(
                title: request.Title,
                avatar: request.Avatar,
                categoryId: request.CategoryId,
                summary: request.Summary,
                content: request.Content,
                updatedBy: adminId
            );

            await _unitOfWork.Blogs.UpdateAsync(blog);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(blog);
        }

        public async Task<bool> DeleteBlogAsync(Guid id, Guid adminId)
        {
            var blog = await _unitOfWork.Blogs.GetByIdAsync(id);
            if (blog == null) throw new Exception("Blog not found");

            blog.SoftDelete(adminId);
            await _unitOfWork.Blogs.UpdateAsync(blog);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<BlogResponseDto> GetBlogByIdAsync(Guid id)
        {
            var blog = await _unitOfWork.Blogs.GetByIdAsync(id);
            if (blog == null || blog.IsDeleted) throw new Exception("Blog not found");

            return MapToDto(blog);
        }

        public async Task<IEnumerable<BlogResponseDto>> GetAllBlogsAsync(bool onlyPublished = false)
        {
            if (onlyPublished)
            {
                var blogs = await _unitOfWork.Blogs.FindAsync(b => b.Status && !b.IsDeleted);
                return blogs.Select(MapToDto);
            }
            else
            {
                var blogs = await _unitOfWork.Blogs.FindAsync(b => !b.IsDeleted);
                return blogs.Select(MapToDto);
            }
        }

        public async Task<bool> PublishBlogAsync(Guid id, Guid adminId)
        {
            var blog = await _unitOfWork.Blogs.GetByIdAsync(id);
            if (blog == null) throw new Exception("Blog not found");

            blog.Publish(adminId);
            await _unitOfWork.Blogs.UpdateAsync(blog);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UnpublishBlogAsync(Guid id, Guid adminId)
        {
            var blog = await _unitOfWork.Blogs.GetByIdAsync(id);
            if (blog == null) throw new Exception("Blog not found");

            blog.Unpublish(adminId);
            await _unitOfWork.Blogs.UpdateAsync(blog);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private static BlogResponseDto MapToDto(Blog blog)
        {
            return new BlogResponseDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Avatar = blog.Avatar,
                CategoryId = blog.CategoryId,
                Summary = blog.Summary,
                Content = blog.Content,
                Status = blog.Status,
                CreatedAt = blog.CreatedAt
            };
        }
    }
}
