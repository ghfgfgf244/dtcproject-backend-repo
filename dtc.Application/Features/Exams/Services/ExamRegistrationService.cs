using dtc.Application.Features.Exams.Interfaces;
using dtc.Application.Features.Exams.DTOs;
using dtc.Application.Features.Exams.Interfaces;
using dtc.Application.Features.Exams.DTOs;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Exams;
using dtc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.Application.Features.Exams.Services
{
    public class ExamRegistrationService : IExamRegistrationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExamRegistrationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ExamRegistrationResponseDto> RegisterAsync(CreateExamRegistrationRequestDto request)
        {
            var batch = await _unitOfWork.ExamBatches.GetByIdAsync(request.ExamBatchId);
            if (batch == null) throw new Exception("Exam batch not found");

            // Prevent registration if closed
            if (batch.Status != ExamBatchStatus.OpenForRegistration)
                throw new Exception("Exam batch is not open for registration");

            var student = await _unitOfWork.Users.GetByIdAsync(request.StudentId);
            if (student == null) throw new Exception("Student not found");

            var existingRegs = await _unitOfWork.ExamRegistrations.FindAsync(r => r.ExamBatchId == request.ExamBatchId && r.StudentId == request.StudentId);
            if (existingRegs != null && existingRegs.Any())
            {
                var existing = existingRegs.First();
                if (existing.Status != ExamRegistrationStatus.Cancelled && existing.Status != ExamRegistrationStatus.Rejected)
                    throw new Exception("Student has already registered for this exam batch");
            }

            var reg = new ExamRegistration(
                examBatchId: request.ExamBatchId,
                studentId: request.StudentId,
                isPaid: request.IsPaid,
                createdBy: request.StudentId
            );

            await _unitOfWork.ExamRegistrations.AddAsync(reg);
            await _unitOfWork.SaveChangesAsync();

            return await MapToDtoAsync(reg);
        }

        public async Task<bool> UpdateStatusAsync(Guid id, UpdateExamRegistrationStatusRequestDto request, Guid adminId)
        {
            var reg = await _unitOfWork.ExamRegistrations.GetByIdAsync(id);
            if (reg == null) throw new Exception("Registration not found");

            switch (request.Status)
            {
                case ExamRegistrationStatus.Approved:
                    reg.Approve(adminId);
                    // Could potentially auto-generate ExamResult placeholder here, but usually done right before exam
                    break;
                case ExamRegistrationStatus.Rejected:
                    reg.Reject(adminId);
                    break;
                case ExamRegistrationStatus.Cancelled:
                    reg.Cancel(adminId);
                    break;
            }

            await _unitOfWork.ExamRegistrations.UpdateAsync(reg);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> MarkAsPaidAsync(Guid id, Guid adminId)
        {
            var reg = await _unitOfWork.ExamRegistrations.GetByIdAsync(id);
            if (reg == null) throw new Exception("Registration not found");

            reg.MarkAsPaid(adminId);
            
            await _unitOfWork.ExamRegistrations.UpdateAsync(reg);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<ExamRegistrationResponseDto>> GetByExamBatchAsync(Guid examBatchId)
        {
            var regs = await _unitOfWork.ExamRegistrations.FindAsync(r => r.ExamBatchId == examBatchId);
            var dtos = new List<ExamRegistrationResponseDto>();
            foreach (var r in regs)
            {
                dtos.Add(await MapToDtoAsync(r));
            }
            return dtos;
        }

        public async Task<IEnumerable<ExamRegistrationResponseDto>> GetByStudentAsync(Guid studentId)
        {
            var regs = await _unitOfWork.ExamRegistrations.FindAsync(r => r.StudentId == studentId);
            var dtos = new List<ExamRegistrationResponseDto>();
            foreach (var r in regs)
            {
                dtos.Add(await MapToDtoAsync(r));
            }
            return dtos.OrderByDescending(r => r.RegistrationDate);
        }

        private async Task<ExamRegistrationResponseDto> MapToDtoAsync(ExamRegistration reg)
        {
            var batch = await _unitOfWork.ExamBatches.GetByIdAsync(reg.ExamBatchId);
            var student = await _unitOfWork.Users.GetByIdAsync(reg.StudentId);

            return new ExamRegistrationResponseDto
            {
                Id = reg.Id,
                ExamBatchId = reg.ExamBatchId,
                StudentId = reg.StudentId,
                StudentName = student?.FullName ?? "Unknown",
                BatchName = batch?.BatchName ?? "Unknown",
                RegistrationDate = reg.RegistrationDate,
                IsPaid = reg.IsPaid,
                Status = reg.Status,
                CreatedAt = reg.CreatedAt
            };
        }
    }
}
