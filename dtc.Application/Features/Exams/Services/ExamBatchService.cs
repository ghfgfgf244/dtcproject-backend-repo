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
    public class ExamBatchService : IExamBatchService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExamBatchService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ExamBatchResponseDto> CreateExamBatchAsync(CreateExamBatchRequestDto request, Guid adminId)
        {
            await ValidateScopeAsync(request.ScopeType, request.CenterId);

            var batch = new ExamBatch(
                scopeType: request.ScopeType,
                centerId: request.CenterId,
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

            if (request.ScopeType.HasValue)
            {
                await ValidateScopeAsync(request.ScopeType.Value, request.CenterId);
            }

            batch.UpdateInfo(
                scopeType: request.ScopeType,
                centerId: request.ScopeType.HasValue ? request.CenterId : batch.CenterId,
                batchName: request.BatchName,
                regStart: request.RegistrationStartDate,
                regEnd: request.RegistrationEndDate,
                examStart: request.ExamStartDate,
                maxCandidates: request.MaxCandidates,
                updatedBy: adminId
            );

            if (request.Status.HasValue)
            {
                batch.ChangeStatus(request.Status.Value, adminId);
            }

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

        public async Task<ExamBatchPagedResponseDto> GetExamBatchesPagedAsync(ExamBatchPagedQueryDto query, Guid? managedCenterId = null)
        {
            var pageNumber = Math.Max(1, query.PageNumber);
            var pageSize = Math.Max(1, query.PageSize);
            var keyword = query.Keyword?.Trim().ToLowerInvariant();

            var allBatches = (await _unitOfWork.ExamBatches.GetAllAsync()).ToList();
            var batchDtos = new List<ExamBatchResponseDto>();

            foreach (var batch in allBatches)
            {
                if (managedCenterId.HasValue &&
                    batch.ScopeType == ExamBatchScopeType.Center &&
                    batch.CenterId != managedCenterId)
                {
                    continue;
                }

                batchDtos.Add(await MapToDtoAsync(batch));
            }

            var filtered = batchDtos.AsEnumerable();

            if (managedCenterId.HasValue)
            {
                filtered = filtered.Where(batch =>
                    batch.ScopeType == ExamBatchScopeType.National ||
                    batch.CenterId == managedCenterId);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                filtered = filtered.Where(batch =>
                    batch.BatchName.ToLowerInvariant().Contains(keyword) ||
                    (!string.IsNullOrWhiteSpace(batch.CenterName) &&
                     batch.CenterName.ToLowerInvariant().Contains(keyword)));
            }

            if (query.Status.HasValue)
            {
                filtered = filtered.Where(batch => batch.Status == query.Status.Value);
            }

            if (query.ScopeType.HasValue)
            {
                filtered = filtered.Where(batch => batch.ScopeType == query.ScopeType.Value);
            }

            var filteredList = filtered
                .OrderByDescending(batch => batch.ExamStartDate)
                .ThenByDescending(batch => batch.CreatedAt)
                .ToList();

            var totalItems = filteredList.Count;
            var items = filteredList
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new ExamBatchPagedResponseDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalItems == 0 ? 1 : (int)Math.Ceiling((double)totalItems / pageSize),
                PendingItems = filteredList.Count(batch => batch.Status == ExamBatchStatus.Pending),
                ApprovedItems = filteredList.Count(batch =>
                    batch.Status == ExamBatchStatus.OpenForRegistration ||
                    batch.Status == ExamBatchStatus.ClosedForRegistration ||
                    batch.Status == ExamBatchStatus.InProgress ||
                    batch.Status == ExamBatchStatus.Completed),
                TotalCandidates = filteredList.Sum(batch => batch.CurrentCandidates),
                TotalCapacity = filteredList.Sum(batch => batch.MaxCandidates),
                Items = items
            };
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
            string? centerName = null;
            if (batch.CenterId.HasValue)
            {
                var center = await _unitOfWork.Centers.GetByIdAsync(batch.CenterId.Value);
                centerName = center?.CenterName;
            }

            return new ExamBatchResponseDto
            {
                Id = batch.Id,
                ScopeType = batch.ScopeType,
                CenterId = batch.CenterId,
                CenterName = centerName,
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

        private async Task ValidateScopeAsync(ExamBatchScopeType scopeType, Guid? centerId)
        {
            if (scopeType == ExamBatchScopeType.Center)
            {
                if (!centerId.HasValue || centerId == Guid.Empty)
                {
                    throw new Exception("Center exam batch must include CenterId.");
                }

                var center = await _unitOfWork.Centers.GetByIdAsync(centerId.Value);
                if (center == null)
                {
                    throw new Exception("Center not found.");
                }

                return;
            }

            if (scopeType == ExamBatchScopeType.National)
            {
                return;
            }

            throw new Exception("Invalid exam batch scope type.");
        }
    }
}
