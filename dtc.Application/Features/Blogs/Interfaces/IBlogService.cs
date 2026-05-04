using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dtc.Application.Features.Blogs.DTOs;

namespace dtc.Application.Features.Blogs.Interfaces
{
    public interface IBlogService
    {
        Task<BlogResponseDto> CreateBlogAsync(CreateBlogRequestDto request, Guid adminId);
        Task<BlogResponseDto> UpdateBlogAsync(Guid id, UpdateBlogRequestDto request, Guid adminId);
        Task<bool> DeleteBlogAsync(Guid id, Guid adminId);
        Task<BlogResponseDto> GetBlogByIdAsync(Guid id);
        Task<IEnumerable<BlogResponseDto>> GetAllBlogsAsync(bool onlyPublished = false);
        Task<BlogPagedResponseDto> GetBlogsPagedAsync(BlogPagedQueryDto query);
        Task<bool> PublishBlogAsync(Guid id, Guid adminId);
        Task<bool> UnpublishBlogAsync(Guid id, Guid adminId);
        Task<IEnumerable<BlogResponseDto>> GetBlogsByUserAsync(Guid userId);
    }
}
