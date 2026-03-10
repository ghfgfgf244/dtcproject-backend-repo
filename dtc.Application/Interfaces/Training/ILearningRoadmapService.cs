using dtc.Application.DTOs.Training;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dtc.Application.Interfaces.Training
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
