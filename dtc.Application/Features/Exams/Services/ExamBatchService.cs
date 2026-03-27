using dtc.Application.Features.Exams.Interfaces;
using dtc.Application.Features.Exams.DTOs;
using dtc.Domain.Entities.Exams;
using dtc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.Application.Features.Exams.Services
{
    public class ExamBatchService : IExamBatchService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExamBatchService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ExamBatchResponseDto> CreateExamBatchAsync(CreateExamBatchRequestDto request, Guid adminId)
        {
            var batch = new ExamBatch(
                batchName: request.BatchName,
                registrationStartDate: request.RegistrationStartDate,
                registrationEndDate: request.RegistrationEndDate,
                examStartDate: request.ExamStartDate,
                maxCandidates: request.MaxCandidates > 0 ? request.MaxCandidates : 1,
                createdBy: adminId
            );

            await _unitOfWork.ExamBatches.AddAsync(batch);
            await _unitOfWork.SaveChangesAsync();

            return await MapToDtoAsync(batch);
        }

        public async Task<ExamBatchResponseDto> UpdateExamBatchAsync(Guid id, UpdateExamBatchRequestDto request, Guid adminId)
        {
            var batch = await _unitOfWork.ExamBatches.GetByIdAsync(id);
            if (batch == null) throw new Exception("Exam batch not found");

            batch.UpdateInfo(
                batchName: request.BatchName,
                regStart: request.RegistrationStartDate,
                regEnd: request.RegistrationEndDate,
                examStart: request.ExamStartDate,
                updatedBy: adminId
            );

            await _unitOfWork.ExamBatches.UpdateAsync(batch);
            await _unitOfWork.SaveChangesAsync();

            return await MapToDtoAsync(batch);
        }

        public async Task<bool> DeleteExamBatchAsync(Guid id, Guid adminId)
        {
            var batch = await _unitOfWork.ExamBatches.GetByIdAsync(id);
            if (batch == null) throw new Exception("Exam batch not found");

            batch.SoftDelete(adminId);
            await _unitOfWork.ExamBatches.UpdateAsync(batch);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<ExamBatchResponseDto> GetExamBatchDetailAsync(Guid id)
        {
            var batch = await _unitOfWork.ExamBatches.GetByIdAsync(id);
            if (batch == null) throw new Exception("Exam batch not found");

            return await MapToDtoAsync(batch);
        }

        public async Task<IEnumerable<ExamBatchResponseDto>> GetAllExamBatchesAsync()
        {
            var batches = await _unitOfWork.ExamBatches.GetAllAsync();
            var dtos = new List<ExamBatchResponseDto>();
            foreach (var b in batches)
            {
                dtos.Add(await MapToDtoAsync(b));
            }
            return dtos;
        }

        public async Task<bool> UpdateExamBatchStatusAsync(Guid id, UpdateExamBatchStatusRequestDto request, Guid adminId)
        {
            var batch = await _unitOfWork.ExamBatches.GetByIdAsync(id);
            if (batch == null) throw new Exception("Exam batch not found");

            batch.ChangeStatus(request.Status, adminId);
            await _unitOfWork.ExamBatches.UpdateAsync(batch);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private async Task<ExamBatchResponseDto> MapToDtoAsync(ExamBatch batch)
        {
            return new ExamBatchResponseDto
            {
                Id = batch.Id,
                BatchName = batch.BatchName,
                RegistrationStartDate = batch.RegistrationStartDate,
                RegistrationEndDate = batch.RegistrationEndDate,
                ExamStartDate = batch.ExamStartDate,
                CurrentCandidates = batch.CurrentCandidates,
                MaxCandidates = batch.MaxCandidates,
                Status = batch.Status,
                CreatedAt = batch.CreatedAt
            };
        }
    }
}
