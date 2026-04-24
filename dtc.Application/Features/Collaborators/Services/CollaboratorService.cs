using dtc.Application.Features.Collaborators.DTOs;
using dtc.Application.Features.Collaborators.Interfaces;
using dtc.Application.Features.Users.DTOs;
using dtc.Application.Features.Users.Interfaces;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Collaborators;
using dtc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.Application.Features.Collaborators.Services
{
    public class CollaboratorService : ICollaboratorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        private const decimal STANDARD_COMMISSION_RATE = 0.05m;
        private const decimal STANDARD_STUDENT_DISCOUNT_RATE = 0.05m;

        public CollaboratorService(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<IEnumerable<UserResponseDto>> GetCollaboratorListAsync()
            => await _userService.GetUsersByRoleAsync((int)UserRole.Collaborator);

        public async Task<ReferralCodeResponseDto?> GetMyReferralCodeAsync(Guid collaboratorId)
        {
            var code = (await _unitOfWork.ReferralCodes.FindAsync(c => c.CollaboratorId == collaboratorId))
                .FirstOrDefault();

            if (code == null)
            {
                return null;
            }

            return new ReferralCodeResponseDto
            {
                Id = code.Id,
                Code = code.Code,
                UsedCount = code.UsedCount,
                IsActive = code.IsActive,
                CommissionRate = STANDARD_COMMISSION_RATE * 100
            };
        }

        public async Task<ReferralCodeResponseDto?> GenerateReferralCodeAsync(Guid collaboratorId, string code)
        {
            var normalizedCode = NormalizeCode(code);
            var existing = await _unitOfWork.ReferralCodes.FindAsync(c => c.Code == normalizedCode);
            if (existing.Any())
            {
                throw new InvalidOperationException("Referral code already exists.");
            }

            var newCode = new ReferralCode(normalizedCode, collaboratorId, collaboratorId);
            await _unitOfWork.ReferralCodes.AddAsync(newCode);
            await _unitOfWork.SaveChangesAsync();

            return await GetMyReferralCodeAsync(collaboratorId);
        }

        public async Task<int> GetTokenUsageCountAsync(Guid collaboratorId)
        {
            var code = (await _unitOfWork.ReferralCodes.FindAsync(c => c.CollaboratorId == collaboratorId))
                .FirstOrDefault();
            return code?.UsedCount ?? 0;
        }

        public Task<decimal> GetCommissionRateAsync()
            => Task.FromResult(STANDARD_COMMISSION_RATE * 100);

        public async Task<ReferralCodeValidationResponseDto> ValidateReferralCodeAsync(string code, Guid? courseId = null)
        {
            var normalizedCode = NormalizeCode(code);
            if (string.IsNullOrWhiteSpace(normalizedCode))
            {
                return new ReferralCodeValidationResponseDto
                {
                    IsValid = false,
                    Message = "Referral code is required.",
                    DiscountRate = STANDARD_STUDENT_DISCOUNT_RATE * 100,
                    CommissionRate = STANDARD_COMMISSION_RATE * 100
                };
            }

            var referralCode = (await _unitOfWork.ReferralCodes.FindAsync(c => c.Code == normalizedCode && c.IsActive))
                .FirstOrDefault();

            if (referralCode == null)
            {
                return new ReferralCodeValidationResponseDto
                {
                    IsValid = false,
                    Code = normalizedCode,
                    Message = "Referral code is invalid or inactive.",
                    DiscountRate = STANDARD_STUDENT_DISCOUNT_RATE * 100,
                    CommissionRate = STANDARD_COMMISSION_RATE * 100
                };
            }

            var collaborator = await _unitOfWork.Users.GetByIdAsync(referralCode.CollaboratorId);

            if (courseId.HasValue && courseId.Value != Guid.Empty)
            {
                var course = await _unitOfWork.Courses.GetByIdAsync(courseId.Value);
                var collaboratorCenter = (await _unitOfWork.UserCenters.FindAsync(uc => uc.UserId == referralCode.CollaboratorId))
                    .FirstOrDefault();

                if (course == null || collaboratorCenter == null || collaboratorCenter.CenterId != course.CenterId)
                {
                    return new ReferralCodeValidationResponseDto
                    {
                        IsValid = false,
                        Code = referralCode.Code,
                        CollaboratorId = referralCode.CollaboratorId,
                        CollaboratorName = collaborator?.FullName,
                        DiscountRate = STANDARD_STUDENT_DISCOUNT_RATE * 100,
                        CommissionRate = STANDARD_COMMISSION_RATE * 100,
                        Message = "Referral code does not apply to this training center."
                    };
                }
            }

            return new ReferralCodeValidationResponseDto
            {
                IsValid = true,
                Code = referralCode.Code,
                CollaboratorId = referralCode.CollaboratorId,
                CollaboratorName = collaborator?.FullName,
                DiscountRate = STANDARD_STUDENT_DISCOUNT_RATE * 100,
                CommissionRate = STANDARD_COMMISSION_RATE * 100,
                Message = collaborator == null
                    ? "Referral code is valid."
                    : $"Referral code belongs to collaborator {collaborator.FullName}."
            };
        }

        public async Task<IEnumerable<CollaboratorCommissionResponseDto>> CalculateAndGetCommissionsAsync(Guid collaboratorId)
            => await GetMyCommissionsAsync(collaboratorId);

        public async Task<IEnumerable<CollaboratorCommissionResponseDto>> GetMyCommissionsAsync(Guid collaboratorId)
        {
            var commissions = (await _unitOfWork.CollaboratorCommissions.FindAsync(c => c.CollaboratorId == collaboratorId))
                .OrderByDescending(c => c.CreatedAt)
                .ToList();

            return await MapCommissionDtosAsync(commissions);
        }

        public async Task<CollaboratorAdminStatsDto> GetAdminStatsAsync()
        {
            var collaborators = await _userService.GetUsersByRoleAsync((int)UserRole.Collaborator);
            var commissions = await _unitOfWork.CollaboratorCommissions.GetAllAsync();

            return new CollaboratorAdminStatsDto
            {
                TotalCollaborators = collaborators.Count(),
                TotalCommissions = commissions.Sum(c => c.Amount),
                PaidCommissions = commissions.Where(c => c.Status == CommissionStatus.Paid).Sum(c => c.Amount),
                UnpaidCommissions = commissions.Where(c => c.Status == CommissionStatus.Pending).Sum(c => c.Amount)
            };
        }

        public async Task<IEnumerable<CollaboratorAdminResponseDto>> GetAdminCollaboratorsAsync()
        {
            var users = await _userService.GetUsersByRoleAsync((int)UserRole.Collaborator);
            var codes = await _unitOfWork.ReferralCodes.GetAllAsync();
            var commissions = await _unitOfWork.CollaboratorCommissions.GetAllAsync();

            var result = new List<CollaboratorAdminResponseDto>();

            foreach (var user in users)
            {
                var userCode = codes.FirstOrDefault(c => c.CollaboratorId == user.Id);
                var userCommissions = commissions.Where(c => c.CollaboratorId == user.Id);

                result.Add(new CollaboratorAdminResponseDto
                {
                    UserId = user.Id,
                    CenterId = user.CenterId,
                    FullName = user.FullName,
                    Email = user.Email,
                    ReferralCode = userCode?.Code ?? "Chua co",
                    UsedCount = userCode?.UsedCount ?? 0,
                    IsCodeActive = userCode?.IsActive ?? false,
                    TotalPaidCommission = userCommissions.Where(c => c.Status == CommissionStatus.Paid).Sum(c => c.Amount),
                    TotalPendingCommission = userCommissions.Where(c => c.Status == CommissionStatus.Pending).Sum(c => c.Amount)
                });
            }

            return result;
        }

        public async Task<IEnumerable<CommissionAdminResponseDto>> GetAdminCommissionsAsync()
        {
            var commissions = (await _unitOfWork.CollaboratorCommissions.GetAllAsync())
                .OrderByDescending(c => c.CreatedAt)
                .ToList();

            var dtoList = await MapCommissionDtosAsync(commissions);
            var collaboratorNames = (await _userService.GetUsersByRoleAsync((int)UserRole.Collaborator))
                .ToDictionary(u => u.Id, u => u.FullName);

            return dtoList.Select(item => new CommissionAdminResponseDto
            {
                Id = item.Id,
                CollaboratorId = item.CollaboratorId,
                ReferralRegistrationId = item.ReferralRegistrationId,
                CollaboratorName = collaboratorNames.GetValueOrDefault(item.CollaboratorId, "Unknown"),
                Amount = item.Amount,
                Status = item.Status,
                CreatedAt = item.CreatedAt,
                PaidAt = item.PaidAt,
                ReferralCode = item.ReferralCode,
                StudentName = item.StudentName,
                CourseName = item.CourseName,
                DiscountAmount = item.DiscountAmount
            }).ToList();
        }

        public async Task<bool> ToggleReferralCodeAsync(Guid collaboratorId)
        {
            var code = (await _unitOfWork.ReferralCodes.FindAsync(c => c.CollaboratorId == collaboratorId))
                .FirstOrDefault();
            if (code == null)
            {
                return false;
            }

            if (code.IsActive)
            {
                code.Deactivate();
            }
            else
            {
                code.Activate();
            }

            await _unitOfWork.ReferralCodes.UpdateAsync(code);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PayCommissionAsync(Guid collaboratorId)
        {
            var pendingCommissions = (await _unitOfWork.CollaboratorCommissions.FindAsync(
                    c => c.CollaboratorId == collaboratorId && c.Status == CommissionStatus.Pending))
                .ToList();

            var code = (await _unitOfWork.ReferralCodes.FindAsync(c => c.CollaboratorId == collaboratorId))
                .FirstOrDefault();

            if (pendingCommissions.Count == 0 && code == null)
            {
                return false;
            }

            foreach (var commission in pendingCommissions)
            {
                commission.MarkAsPaid();
                await _unitOfWork.CollaboratorCommissions.UpdateAsync(commission);
            }

            if (code != null)
            {
                code.ResetUsage(collaboratorId);
                await _unitOfWork.ReferralCodes.UpdateAsync(code);
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private async Task<List<CollaboratorCommissionResponseDto>> MapCommissionDtosAsync(
            IEnumerable<CollaboratorCommission> commissions)
        {
            var commissionList = commissions.ToList();
            var referralRegistrationIds = commissionList
                .Where(c => c.ReferralRegistrationId.HasValue)
                .Select(c => c.ReferralRegistrationId!.Value)
                .Distinct()
                .ToList();

            var referralRegistrations = referralRegistrationIds.Count == 0
                ? new List<ReferralRegistration>()
                : (await _unitOfWork.ReferralRegistrations.FindAsync(r => referralRegistrationIds.Contains(r.Id))).ToList();

            var referralCodeIds = referralRegistrations.Select(r => r.ReferralCodeId).Distinct().ToList();
            var studentIds = referralRegistrations.Select(r => r.StudentId).Distinct().ToList();
            var courseRegistrationIds = referralRegistrations
                .Where(r => r.CourseRegistrationId.HasValue)
                .Select(r => r.CourseRegistrationId!.Value)
                .Distinct()
                .ToList();

            var referralCodes = referralCodeIds.Count == 0
                ? new List<ReferralCode>()
                : (await _unitOfWork.ReferralCodes.FindAsync(c => referralCodeIds.Contains(c.Id))).ToList();

            var students = studentIds.Count == 0
                ? new List<dtc.Domain.Entities.Permissions.User>()
                : (await _unitOfWork.Users.FindAsync(u => studentIds.Contains(u.Id))).ToList();

            var courseRegistrations = courseRegistrationIds.Count == 0
                ? new List<dtc.Domain.Entities.Terms.CourseRegistration>()
                : (await _unitOfWork.CourseRegistrations.FindAsync(cr => courseRegistrationIds.Contains(cr.Id))).ToList();

            var courseIds = courseRegistrations.Select(cr => cr.CourseId).Distinct().ToList();
            var courses = courseIds.Count == 0
                ? new List<dtc.Domain.Entities.Training.Course>()
                : (await _unitOfWork.Courses.FindAsync(c => courseIds.Contains(c.Id))).ToList();

            return commissionList.Select(commission =>
            {
                var referralRegistration = commission.ReferralRegistrationId.HasValue
                    ? referralRegistrations.FirstOrDefault(r => r.Id == commission.ReferralRegistrationId.Value)
                    : null;
                var referralCode = referralRegistration == null
                    ? null
                    : referralCodes.FirstOrDefault(c => c.Id == referralRegistration.ReferralCodeId);
                var student = referralRegistration == null
                    ? null
                    : students.FirstOrDefault(s => s.Id == referralRegistration.StudentId);
                var courseRegistration = referralRegistration?.CourseRegistrationId == null
                    ? null
                    : courseRegistrations.FirstOrDefault(cr => cr.Id == referralRegistration.CourseRegistrationId.Value);
                var course = courseRegistration == null
                    ? null
                    : courses.FirstOrDefault(c => c.Id == courseRegistration.CourseId);

                return new CollaboratorCommissionResponseDto
                {
                    Id = commission.Id,
                    CollaboratorId = commission.CollaboratorId,
                    ReferralRegistrationId = commission.ReferralRegistrationId,
                    Amount = commission.Amount,
                    Status = commission.Status.ToString(),
                    CreatedAt = commission.CreatedAt,
                    PaidAt = commission.PaidAt,
                    ReferralCode = referralCode?.Code,
                    StudentName = student?.FullName,
                    CourseName = course?.CourseName,
                    DiscountAmount = courseRegistration == null
                        ? null
                        : Math.Round(courseRegistration.OriginalFee - courseRegistration.TotalFee, 2, MidpointRounding.AwayFromZero)
                };
            }).ToList();
        }

        private static string NormalizeCode(string code)
            => string.IsNullOrWhiteSpace(code) ? string.Empty : code.Trim().ToUpperInvariant();
    }
}
