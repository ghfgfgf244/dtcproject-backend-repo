using dtc.Application.DTOs.Training;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dtc.Application.Interfaces.Training
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
