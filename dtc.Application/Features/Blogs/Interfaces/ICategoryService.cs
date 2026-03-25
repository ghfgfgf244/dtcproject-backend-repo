using System.Collections.Generic;
using System.Threading.Tasks;
using dtc.Application.Features.Blogs.DTOs;

namespace dtc.Application.Features.Blogs.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryRequestDto request);
        Task<CategoryResponseDto> UpdateCategoryAsync(Guid id, UpdateCategoryRequestDto request);
        Task<bool> DeleteCategoryAsync(Guid id);
        Task<CategoryResponseDto> GetCategoryByIdAsync(Guid id);
        Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync();
    }
}
