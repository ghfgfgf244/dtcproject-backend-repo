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
    public class StudentEvaluationService : IStudentEvaluationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentEvaluationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<StudentEvaluationResponseDto> CreateEvaluationAsync(Guid instructorId, CreateStudentEvaluationRequestDto request)
        {
            var student = await _unitOfWork.Users.GetByIdAsync(request.StudentId.ToString());
            if (student == null) throw new Exception("Student not found");

            var evaluation = new StudentEvaluation(
                studentId: request.StudentId,
                instructorId: instructorId,
                classId: request.ClassId,
                punctualityScore: request.PunctualityScore,
                skillLevel: request.SkillLevel,
                note: request.Note,
                createdBy: instructorId
            );

            await _unitOfWork.StudentEvaluations.AddAsync(evaluation);
            await _unitOfWork.SaveChangesAsync();

            return await MapToDtoAsync(evaluation);
        }

        public async Task<StudentEvaluationResponseDto> UpdateEvaluationAsync(Guid id, Guid instructorId, UpdateStudentEvaluationRequestDto request)
        {
            var evaluation = await _unitOfWork.StudentEvaluations.GetByIdAsync(id);
            if (evaluation == null) throw new Exception("Evaluation not found");

            // Option: check if instructor is the one who created it, or if they are admin

            evaluation.UpdateEvaluation(
                punctualityScore: request.PunctualityScore,
                skillLevel: request.SkillLevel,
                note: request.Note,
                updatedBy: instructorId
            );

            await _unitOfWork.StudentEvaluations.UpdateAsync(evaluation);
            await _unitOfWork.SaveChangesAsync();

            return await MapToDtoAsync(evaluation);
        }

        public async Task<StudentEvaluationResponseDto> GetEvaluationByIdAsync(Guid id)
        {
            var evaluation = await _unitOfWork.StudentEvaluations.GetByIdAsync(id);
            if (evaluation == null) throw new Exception("Evaluation not found");

            return await MapToDtoAsync(evaluation);
        }

        public async Task<IEnumerable<StudentEvaluationResponseDto>> GetEvaluationsForStudentAsync(Guid studentId)
        {
            var evaluations = await _unitOfWork.StudentEvaluations.FindAsync(e => e.StudentId == studentId);
            var dtos = new List<StudentEvaluationResponseDto>();
            foreach (var eval in evaluations.OrderByDescending(e => e.EvaluationDate))
            {
                dtos.Add(await MapToDtoAsync(eval));
            }
            return dtos;
        }

        public async Task<IEnumerable<StudentEvaluationResponseDto>> GetEvaluationsByClassAsync(Guid classId)
        {
            var evaluations = await _unitOfWork.StudentEvaluations.FindAsync(e => e.ClassId == classId);
            var dtos = new List<StudentEvaluationResponseDto>();
            foreach (var eval in evaluations.OrderByDescending(e => e.EvaluationDate))
            {
                dtos.Add(await MapToDtoAsync(eval));
            }
            return dtos;
        }

        public async Task<bool> DeleteEvaluationAsync(Guid id)
        {
            var evaluation = await _unitOfWork.StudentEvaluations.GetByIdAsync(id);
            if (evaluation == null) throw new Exception("Evaluation not found");

            await _unitOfWork.StudentEvaluations.RemoveAsync(evaluation);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private async Task<StudentEvaluationResponseDto> MapToDtoAsync(StudentEvaluation evaluation)
        {
            var student = await _unitOfWork.Users.GetByIdAsync(evaluation.StudentId.ToString());
            var instructor = await _unitOfWork.Users.GetByIdAsync(evaluation.InstructorId.ToString());

            return new StudentEvaluationResponseDto
            {
                Id = evaluation.Id,
                StudentId = evaluation.StudentId,
                StudentName = student?.FullName ?? "Unknown",
                InstructorId = evaluation.InstructorId,
                InstructorName = instructor?.FullName ?? "Unknown",
                ClassId = evaluation.ClassId,
                PunctualityScore = evaluation.PunctualityScore,
                SkillLevel = evaluation.SkillLevel,
                Note = evaluation.Note,
                EvaluationDate = evaluation.EvaluationDate
            };
        }
    }
}
