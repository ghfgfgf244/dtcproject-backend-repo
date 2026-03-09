using dtc.Application.DTOs.Training.Terms;
using dtc.Application.Interfaces.Training;
using dtc.Domain.Entities.Terms;
using dtc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.Application.Services.Training
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

            var term = new Term(request.CourseId, request.TermName, request.StartDate, request.EndDate, adminId);
            
            await _unitOfWork.Terms.AddAsync(term);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(term);
        }

        public async Task<TermResponseDto> UpdateTermAsync(Guid termId, UpdateTermRequestDto request, Guid adminId)
        {
            var term = await _unitOfWork.Terms.GetByIdAsync(termId);
            if (term == null)
                throw new Exception("Term not found");

            var changed = term.UpdateInfo(request.TermName, request.StartDate, request.EndDate, adminId);
            if (changed)
            {
                await _unitOfWork.Terms.UpdateAsync(term);
                await _unitOfWork.SaveChangesAsync();
            }

            return MapToDto(term);
        }

        public async Task<IEnumerable<TermResponseDto>> GetAllTermsAsync()
        {
            var terms = await _unitOfWork.Terms.GetAllAsync();
            return terms.Select(MapToDto);
        }

        public async Task<TermResponseDto> GetTermDetailAsync(Guid termId)
        {
            var term = await _unitOfWork.Terms.GetByIdAsync(termId);
            if (term == null)
                throw new Exception("Term not found");

            return MapToDto(term);
        }

        private TermResponseDto MapToDto(Term term)
        {
            return new TermResponseDto
            {
                Id = term.Id,
                CourseId = term.CourseId,
                TermName = term.TermName,
                StartDate = term.StartDate,
                EndDate = term.EndDate,
                CreatedAt = term.CreatedAt
            };
        }
    }
}
