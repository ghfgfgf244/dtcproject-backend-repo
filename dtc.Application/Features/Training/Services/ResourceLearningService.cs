using dtc.Application.Features.Training.Interfaces;
using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Training.Interfaces;
using dtc.Application.Features.Training.DTOs;
using dtc.Domain.Entities.Training;
using dtc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.Application.Features.Training.Services
{
    public class ResourceLearningService : IResourceLearningService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ResourceLearningService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResourceLearningResponseDto> CreateResourceLearningAsync(CreateResourceLearningRequestDto request)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
            if (course == null) throw new Exception("Course not found");

            var resource = new ResourceLearning(
                courseId: request.CourseId,
                resourceType: request.ResourceType,
                title: request.Title,
                resourceUrl: request.ResourceUrl
            );

            await _unitOfWork.ResourceLearnings.AddAsync(resource);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(resource);
        }

        public async Task<ResourceLearningResponseDto> GetResourceLearningByIdAsync(Guid id)
        {
            var resource = await _unitOfWork.ResourceLearnings.GetByIdAsync(id);
            if (resource == null) throw new Exception("Resource learning not found");

            return MapToDto(resource);
        }

        public async Task<IEnumerable<ResourceLearningResponseDto>> GetResourceLearningsByCourseAsync(Guid courseId)
        {
            var resources = await _unitOfWork.ResourceLearnings.FindAsync(r => r.CourseId == courseId);
            return resources.Select(MapToDto).ToList();
        }

        public async Task<ResourceLearningResponseDto> UpdateResourceLearningAsync(Guid id, UpdateResourceLearningRequestDto request)
        {
            var resource = await _unitOfWork.ResourceLearnings.GetByIdAsync(id);
            if (resource == null) throw new Exception("Resource learning not found");

            resource.UpdateInfo(
                resourceType: request.ResourceType,
                title: request.Title,
                resourceUrl: request.ResourceUrl
            );

            await _unitOfWork.ResourceLearnings.UpdateAsync(resource);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(resource);
        }

        public async Task<bool> DeleteResourceLearningAsync(Guid id)
        {
            var resource = await _unitOfWork.ResourceLearnings.GetByIdAsync(id);
            if (resource == null) throw new Exception("Resource learning not found");

            resource.Deactivate();

            await _unitOfWork.ResourceLearnings.UpdateAsync(resource);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private ResourceLearningResponseDto MapToDto(ResourceLearning resource)
        {
            return new ResourceLearningResponseDto
            {
                Id = resource.Id,
                CourseId = resource.CourseId,
                ResourceType = resource.ResourceType,
                Title = resource.Title,
                ResourceUrl = resource.ResourceUrl,
                IsActive = resource.IsActive,
                CreatedAt = resource.CreatedAt
            };
        }
    }
}
