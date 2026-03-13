using dtc.Application.Features.Training.Interfaces;
using dtc.Application.Features.Training.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dtc.Application.Features.Training.Interfaces
{
    public interface ILearningRoadmapService
    {
        Task<LearningRoadmapResponseDto> CreateLearningRoadmapAsync(CreateLearningRoadmapRequestDto request);
        Task<LearningRoadmapResponseDto> GetLearningRoadmapByIdAsync(Guid id);
        Task<IEnumerable<LearningRoadmapResponseDto>> GetLearningRoadmapsByCourseAsync(Guid courseId);
        Task<LearningRoadmapResponseDto> UpdateLearningRoadmapAsync(Guid id, UpdateLearningRoadmapRequestDto request);
        Task<bool> DeleteLearningRoadmapAsync(Guid id);
    }
}
