using dtc.Application.DTOs.Training;
using dtc.Application.Interfaces.Training;
using dtc.Domain.Entities.Training;
using dtc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.Application.Services.Training
{
    public class LearningRoadmapService : ILearningRoadmapService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LearningRoadmapService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LearningRoadmapResponseDto> CreateLearningRoadmapAsync(CreateLearningRoadmapRequestDto request)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
            if (course == null) throw new Exception("Course not found");

            var roadmap = new LearningRoadmap(
                courseId: request.CourseId,
                title: request.Title,
                description: request.Description,
                orderNo: request.OrderNo
            );

            await _unitOfWork.LearningRoadmaps.AddAsync(roadmap);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(roadmap);
        }

        public async Task<LearningRoadmapResponseDto> GetLearningRoadmapByIdAsync(Guid id)
        {
            var roadmap = await _unitOfWork.LearningRoadmaps.GetByIdAsync(id);
            if (roadmap == null) throw new Exception("Learning roadmap not found");

            return MapToDto(roadmap);
        }

        public async Task<IEnumerable<LearningRoadmapResponseDto>> GetLearningRoadmapsByCourseAsync(Guid courseId)
        {
            var roadmaps = await _unitOfWork.LearningRoadmaps.FindAsync(r => r.CourseId == courseId);
            return roadmaps.OrderBy(r => r.OrderNo).Select(MapToDto).ToList();
        }

        public async Task<LearningRoadmapResponseDto> UpdateLearningRoadmapAsync(Guid id, UpdateLearningRoadmapRequestDto request)
        {
            var roadmap = await _unitOfWork.LearningRoadmaps.GetByIdAsync(id);
            if (roadmap == null) throw new Exception("Learning roadmap not found");

            roadmap.UpdateContent(
                title: request.Title,
                description: request.Description
            );

            if (request.OrderNo.HasValue)
            {
                roadmap.ChangeOrder(request.OrderNo.Value);
            }

            await _unitOfWork.LearningRoadmaps.UpdateAsync(roadmap);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(roadmap);
        }

        public async Task<bool> DeleteLearningRoadmapAsync(Guid id)
        {
            var roadmap = await _unitOfWork.LearningRoadmaps.GetByIdAsync(id);
            if (roadmap == null) throw new Exception("Learning roadmap not found");

            await _unitOfWork.LearningRoadmaps.RemoveAsync(roadmap);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private LearningRoadmapResponseDto MapToDto(LearningRoadmap roadmap)
        {
            return new LearningRoadmapResponseDto
            {
                Id = roadmap.Id,
                CourseId = roadmap.CourseId,
                Title = roadmap.Title,
                Description = roadmap.Description,
                OrderNo = roadmap.OrderNo
            };
        }
    }
}
