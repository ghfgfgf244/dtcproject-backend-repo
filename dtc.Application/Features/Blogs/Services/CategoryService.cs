using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dtc.Application.Features.Blogs.DTOs;
using dtc.Application.Features.Blogs.Interfaces;
using dtc.Domain.Entities.Blogs;
using dtc.Domain.Interfaces;

namespace dtc.Application.Features.Blogs.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryRequestDto request)
        {
            var category = new Category(request.Name, null, request.ParentCategoryId);
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return new CategoryResponseDto { Id = category.Id, Name = category.CategoryName, ParentCategoryId = category.ParentCategoryId, ShowMenuStatus = category.ShowMenuStatus };
        }

        public async Task<CategoryResponseDto> UpdateCategoryAsync(Guid id, UpdateCategoryRequestDto request)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) throw new System.Exception("Category not found");

            category.Update(request.Name, request.ParentCategoryId, request.ShowMenuStatus, null);
            await _unitOfWork.Categories.UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return new CategoryResponseDto { Id = category.Id, Name = category.CategoryName, ParentCategoryId = category.ParentCategoryId, ShowMenuStatus = category.ShowMenuStatus };
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) throw new System.Exception("Category not found");

            await _unitOfWork.Categories.RemoveAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<CategoryResponseDto> GetCategoryByIdAsync(Guid id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) throw new System.Exception("Category not found");

            return new CategoryResponseDto { Id = category.Id, Name = category.CategoryName, ParentCategoryId = category.ParentCategoryId, ShowMenuStatus = category.ShowMenuStatus };
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return categories.Select(c => new CategoryResponseDto { Id = c.Id, Name = c.CategoryName, ParentCategoryId = c.ParentCategoryId, ShowMenuStatus = c.ShowMenuStatus });
        }
    }
}
