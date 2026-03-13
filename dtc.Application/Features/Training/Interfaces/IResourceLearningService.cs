using dtc.Application.Features.Training.Interfaces;
using dtc.Application.Features.Training.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dtc.Application.Features.Training.Interfaces
{
    public interface IResourceLearningService
    {
        Task<ResourceLearningResponseDto> CreateResourceLearningAsync(CreateResourceLearningRequestDto request);
        Task<ResourceLearningResponseDto> GetResourceLearningByIdAsync(Guid id);
        Task<IEnumerable<ResourceLearningResponseDto>> GetResourceLearningsByCourseAsync(Guid courseId);
        Task<ResourceLearningResponseDto> UpdateResourceLearningAsync(Guid id, UpdateResourceLearningRequestDto request);
        Task<bool> DeleteResourceLearningAsync(Guid id); // Soft delete
    }
}
