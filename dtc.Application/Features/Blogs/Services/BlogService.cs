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
            var category = (await _unitOfWork.Categories.FindAsync(c => c.CategoryId == request.CategoryId)).FirstOrDefault();
            if (category == null) throw new KeyNotFoundException("Category not found");

            var blog = new Blog(request.Title, request.CategoryId, request.Content, adminId, request.Summary, request.Avatar);
            if (request.Status)
            {
                blog.Publish(adminId);
            }
            
            await _unitOfWork.Blogs.AddAsync(blog);
            await _unitOfWork.SaveChangesAsync();

            return await MapToDtoAsync(blog);
        }

        public async Task<BlogResponseDto> UpdateBlogAsync(Guid id, UpdateBlogRequestDto request, Guid adminId)
        {
            var blog = await _unitOfWork.Blogs.GetByIdAsync(id);
            if (blog == null) throw new KeyNotFoundException("Blog not found");

            if (request.CategoryId.HasValue)
            {
                var category = (await _unitOfWork.Categories.FindAsync(c => c.CategoryId == request.CategoryId.Value)).FirstOrDefault();
                if (category == null) throw new KeyNotFoundException("Category not found");
            }

            blog.Update(
                title: request.Title,
                avatar: request.Avatar,
                categoryId: request.CategoryId,
                summary: request.Summary,
                content: request.Content,
                updatedBy: adminId
            );

            if (request.Status.HasValue)
            {
                if (request.Status.Value)
                {
                    blog.Publish(adminId);
                }
                else
                {
                    blog.Unpublish(adminId);
                }
            }

            await _unitOfWork.Blogs.UpdateAsync(blog);
            await _unitOfWork.SaveChangesAsync();

            return await MapToDtoAsync(blog);
        }

        public async Task<bool> DeleteBlogAsync(Guid id, Guid adminId)
        {
            var blog = await _unitOfWork.Blogs.GetByIdAsync(id);
            if (blog == null) throw new KeyNotFoundException("Blog not found");

            blog.SoftDelete(adminId);
            await _unitOfWork.Blogs.UpdateAsync(blog);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<BlogResponseDto> GetBlogByIdAsync(Guid id)
        {
            var blog = await _unitOfWork.Blogs.GetByIdAsync(id);
            if (blog == null || blog.IsDeleted) throw new KeyNotFoundException("Blog not found");

            return await MapToDtoAsync(blog);
        }

        public async Task<IEnumerable<BlogResponseDto>> GetAllBlogsAsync(bool onlyPublished = false)
        {
            IEnumerable<Blog> blogs;
            if (onlyPublished)
            {
                blogs = await _unitOfWork.Blogs.FindAsync(b => b.Status && !b.IsDeleted);
            }
            else
            {
                blogs = await _unitOfWork.Blogs.FindAsync(b => !b.IsDeleted);
            }

            var dtos = new List<BlogResponseDto>();
            foreach (var blog in blogs.OrderByDescending(b => b.CreatedAt))
            {
                try
                {
                    dtos.Add(await MapToDtoAsync(blog));
                }
                catch (Exception ex)
                {
                    // Defensive: skip items that fail to map instead of return 500
                    Console.WriteLine($"Error mapping blog {blog.Id}: {ex.Message}");
                }
            }
            return dtos;
        }

        public async Task<bool> PublishBlogAsync(Guid id, Guid adminId)
        {
            var blog = await _unitOfWork.Blogs.GetByIdAsync(id);
            if (blog == null) throw new KeyNotFoundException("Blog not found");

            blog.Publish(adminId);
            await _unitOfWork.Blogs.UpdateAsync(blog);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UnpublishBlogAsync(Guid id, Guid adminId)
        {
            var blog = await _unitOfWork.Blogs.GetByIdAsync(id);
            if (blog == null) throw new KeyNotFoundException("Blog not found");

            blog.Unpublish(adminId);
            await _unitOfWork.Blogs.UpdateAsync(blog);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<BlogResponseDto>> GetBlogsByUserAsync(Guid userId)
        {
            var blogs = await _unitOfWork.Blogs.FindAsync(b => b.CreatedBy == userId && !b.IsDeleted);
            var dtos = new List<BlogResponseDto>();
            foreach (var blog in blogs.OrderByDescending(b => b.CreatedAt))
            {
                dtos.Add(await MapToDtoAsync(blog));
            }
            return dtos;
        }

        private async Task<BlogResponseDto> MapToDtoAsync(Blog blog)
        {
            dtc.Domain.Entities.Permissions.User? user = null;
            if (blog.CreatedBy.HasValue && blog.CreatedBy.Value != Guid.Empty)
            {
                try
                {
                    user = await _unitOfWork.Users.GetByIdAsync(blog.CreatedBy.Value);
                }
                catch
                {
                    // Fallback to null if user lookup fails
                }
            }

            var category = (await _unitOfWork.Categories.FindAsync(c => c.CategoryId == blog.CategoryId)).FirstOrDefault();

            return new BlogResponseDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Avatar = blog.Avatar,
                CategoryId = blog.CategoryId,
                CategoryName = category?.CategoryName,
                Summary = blog.Summary,
                Content = blog.Content,
                Status = blog.Status,
                CreatedAt = blog.CreatedAt,
                AuthorName = user?.FullName ?? "Tác giả",
                AuthorAvatar = user?.AvatarUrl
            };
        }
    }
}
