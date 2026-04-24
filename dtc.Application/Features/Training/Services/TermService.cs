using dtc.Application.Features.Training.Interfaces;
using dtc.Application.Features.Training.DTOs;
using dtc.Domain.Entities.Terms;
using dtc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.Application.Features.Training.Services
{
    public class TermService : ITermService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TermService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TermResponseDto> CreateTermAsync(CreateTermRequestDto request, Guid adminId)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
            if (course == null)
                throw new Exception("Course not found");

            var term = new Term(request.CourseId, request.TermName, request.StartDate, request.EndDate, request.MaxStudents > 0 ? request.MaxStudents : 1, adminId);
            
            await _unitOfWork.Terms.AddAsync(term);
            await _unitOfWork.SaveChangesAsync();

            return await MapToDtoAsync(term);
        }

        public async Task<TermResponseDto> UpdateTermAsync(Guid termId, UpdateTermRequestDto request, Guid adminId)
        {
            var term = await _unitOfWork.Terms.GetByIdAsync(termId);
            if (term == null)
                throw new Exception("Term not found");

            var changed = term.UpdateInfo(request.TermName, request.StartDate, request.EndDate, request.MaxStudents, request.IsActive, adminId);
            if (changed)
            {
                await _unitOfWork.Terms.UpdateAsync(term);
                await _unitOfWork.SaveChangesAsync();
            }

            return await MapToDtoAsync(term);
        }

        public async Task<IEnumerable<TermResponseDto>> GetAllTermsAsync()
        {
            var terms = (await _unitOfWork.Terms.GetAllAsync()).ToList();
            var courses = (await _unitOfWork.Courses.GetAllAsync()).ToDictionary(c => c.Id, c => c);
            var centerIds = courses.Values.Select(c => c.CenterId).Distinct().ToList();
            var centers = (await _unitOfWork.Centers.FindAsync(c => centerIds.Contains(c.Id)))
                .ToDictionary(c => c.Id, c => c.CenterName);

            return terms.Select(term => {
                var dto = MapToDtoInternal(term);
                if (courses.TryGetValue(term.CourseId, out var course))
                {
                    dto.CourseName = course.CourseName;
                    dto.CenterId = course.CenterId;
                    if (centers.TryGetValue(course.CenterId, out var centerName))
                    {
                        dto.CenterName = centerName;
                    }
                }
                return dto;
            });
        }

        public async Task<TermResponseDto> GetTermDetailAsync(Guid termId)
        {
            var term = await _unitOfWork.Terms.GetByIdAsync(termId);
            if (term == null)
                throw new Exception("Term not found");

            return await MapToDtoAsync(term);
        }

        public async Task<bool> DeleteTermAsync(Guid termId, Guid adminId)
        {
            var term = await _unitOfWork.Terms.GetByIdAsync(termId);
            if (term == null)
                throw new Exception("Term not found");

            var classes = await _unitOfWork.Classes.FindAsync(c => c.TermId == termId);
            if (classes != null && classes.Any())
            {
                throw new InvalidOperationException("Cannot delete term because there are classes running in this term.");
            }

            term.SoftDelete(adminId);
            await _unitOfWork.Terms.UpdateAsync(term);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private async Task<TermResponseDto> MapToDtoAsync(Term term)
        {
            var dto = MapToDtoInternal(term);
            var course = await _unitOfWork.Courses.GetByIdAsync(term.CourseId);
            if (course != null)
            {
                dto.CourseName = course.CourseName;
                dto.CenterId = course.CenterId;

                var center = await _unitOfWork.Centers.GetByIdAsync(course.CenterId);
                dto.CenterName = center?.CenterName;
            }
            
            return dto;
        }

        private TermResponseDto MapToDtoInternal(Term term)
        {
            return new TermResponseDto
            {
                Id = term.Id,
                CourseId = term.CourseId,
                CenterId = Guid.Empty,
                TermName = term.TermName,
                StartDate = term.StartDate,
                EndDate = term.EndDate,
                CurrentStudents = term.CurrentStudents,
                MaxStudents = term.MaxStudents,
                IsActive = term.IsActive,
                CreatedAt = term.CreatedAt
            };
        }
    }
}
