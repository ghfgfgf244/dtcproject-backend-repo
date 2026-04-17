using dtc.Application.Features.Collaborators.Interfaces;
using dtc.Application.Features.Collaborators.DTOs;
using dtc.Application.Features.Users.Interfaces;
using dtc.Application.Features.Users.DTOs;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Collaborators;
using dtc.Domain.Entities.Terms;
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

        private const decimal STANDARD_COMMISSION_RATE = 0.10m; // 10%

        public CollaboratorService(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        // DEV-127: View collaborator's list
        public async Task<IEnumerable<UserResponseDto>> GetCollaboratorListAsync()
        {
            return await _userService.GetUsersByRoleAsync((int)UserRole.Collaborator);
        }

        // DEV-128: View personal token
        public async Task<ReferralCodeResponseDto?> GetMyReferralCodeAsync(Guid collaboratorId)
        {
            var codes = await _unitOfWork.ReferralCodes.FindAsync(c => c.CollaboratorId == collaboratorId);
            var code = codes.FirstOrDefault();

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
                CommissionRate = STANDARD_COMMISSION_RATE * 100 // return as percentage
            };
        }

        public async Task<ReferralCodeResponseDto?> GenerateReferralCodeAsync(Guid collaboratorId, string code)
        {
            var existing = await _unitOfWork.ReferralCodes.FindAsync(c => c.Code == code.Trim().ToUpper());
            if (existing.Any()) throw new InvalidOperationException("Referral code already exists.");

            var newCode = new ReferralCode(code, collaboratorId, collaboratorId);
            await _unitOfWork.ReferralCodes.AddAsync(newCode);
            await _unitOfWork.SaveChangesAsync();

            return await GetMyReferralCodeAsync(collaboratorId);
        }

        // DEV-129: View number of user using token
        public async Task<int> GetTokenUsageCountAsync(Guid collaboratorId)
        {
            var codes = await _unitOfWork.ReferralCodes.FindAsync(c => c.CollaboratorId == collaboratorId);
            var code = codes.FirstOrDefault();
            if (code == null) return 0;

            var registrations = await _unitOfWork.ReferralRegistrations.FindAsync(r => r.ReferralCodeId == code.Id);
            return registrations.Count();
        }

        // DEV-130: View commission rate
        public Task<decimal> GetCommissionRateAsync()
        {
            return Task.FromResult(STANDARD_COMMISSION_RATE * 100);
        }

        // DEV-131: Calculate commission
        public async Task<IEnumerable<CollaboratorCommissionResponseDto>> CalculateAndGetCommissionsAsync(Guid collaboratorId)
        {
            var codes = await _unitOfWork.ReferralCodes.FindAsync(c => c.CollaboratorId == collaboratorId);
            var code = codes.FirstOrDefault();
            if (code == null) throw new KeyNotFoundException("No referral code found for this collaborator.");

            var referrals = await _unitOfWork.ReferralRegistrations.FindAsync(r => r.ReferralCodeId == code.Id);
            var studentIds = referrals.Select(r => r.StudentId).ToList();

            var courseRegs = await _unitOfWork.CourseRegistrations.FindAsync(cr => studentIds.Contains(cr.UserId) && cr.Status == CourseRegistrationStatus.Approved);

            decimal totalEligibleRevenue = courseRegs.Sum(cr => cr.TotalFee);
            decimal calculatedCommission = totalEligibleRevenue * STANDARD_COMMISSION_RATE;

            var existingCommissions = await _unitOfWork.CollaboratorCommissions.FindAsync(c => c.CollaboratorId == collaboratorId);

            decimal existingSum = existingCommissions.Sum(c => c.Amount);
            if (calculatedCommission > existingSum)
            {
                var newCommissionAmount = calculatedCommission - existingSum;
                var newComm = new CollaboratorCommission(collaboratorId, newCommissionAmount);
                await _unitOfWork.CollaboratorCommissions.AddAsync(newComm);
                await _unitOfWork.SaveChangesAsync();
                
                existingCommissions = await _unitOfWork.CollaboratorCommissions.FindAsync(c => c.CollaboratorId == collaboratorId);
            }

            return existingCommissions.Select(c => new CollaboratorCommissionResponseDto
            {
                Id = c.Id,
                CollaboratorId = c.CollaboratorId,
                Amount = c.Amount,
                Status = c.Status.ToString(),
                CreatedAt = c.CreatedAt,
                PaidAt = c.PaidAt
            });
        }

        public async Task<IEnumerable<CollaboratorCommissionResponseDto>> GetMyCommissionsAsync(Guid collaboratorId)
        {
            var existingCommissions = await _unitOfWork.CollaboratorCommissions.FindAsync(c => c.CollaboratorId == collaboratorId);
            return existingCommissions.Select(c => new CollaboratorCommissionResponseDto
            {
                Id = c.Id,
                CollaboratorId = c.CollaboratorId,
                Amount = c.Amount,
                Status = c.Status.ToString(),
                CreatedAt = c.CreatedAt,
                PaidAt = c.PaidAt
            }).OrderByDescending(c => c.CreatedAt).ToList();
        }

        // ==========================================
        // ADMIN METHODS
        // ==========================================

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
                    FullName = user.FullName,
                    Email = user.Email,
                    ReferralCode = userCode?.Code ?? "Chưa có",
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
            var commissions = await _unitOfWork.CollaboratorCommissions.GetAllAsync();
            var users = await _userService.GetUsersByRoleAsync((int)UserRole.Collaborator);

            return commissions.Select(c => new CommissionAdminResponseDto
            {
                Id = c.Id,
                CollaboratorId = c.CollaboratorId,
                Amount = c.Amount,
                Status = c.Status.ToString(),
                CreatedAt = c.CreatedAt,
                PaidAt = c.PaidAt,
                CollaboratorName = users.FirstOrDefault(u => u.Id == c.CollaboratorId)?.FullName ?? "Unknown"
            }).OrderByDescending(c => c.CreatedAt).ToList();
        }

        public async Task<bool> ToggleReferralCodeAsync(Guid collaboratorId)
        {
            var codes = await _unitOfWork.ReferralCodes.FindAsync(c => c.CollaboratorId == collaboratorId);
            var code = codes.FirstOrDefault();
            if (code == null) return false;

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
            // First, trigger a calculation to ensure any eligible unrecorded commissions are captured
            await CalculateAndGetCommissionsAsync(collaboratorId);

            // Fetch pending commissions
            var pendingCommissions = await _unitOfWork.CollaboratorCommissions.FindAsync(c => c.CollaboratorId == collaboratorId && c.Status == CommissionStatus.Pending);
            var codes = await _unitOfWork.ReferralCodes.FindAsync(c => c.CollaboratorId == collaboratorId);
            var code = codes.FirstOrDefault();

            if (!pendingCommissions.Any() && code == null) return false;

            // Mark existing pending as paid
            foreach (var comm in pendingCommissions)
            {
                comm.MarkAsPaid();
                await _unitOfWork.CollaboratorCommissions.UpdateAsync(comm);
            }

            // Reset the usage count for the referral code
            if (code != null)
            {
                code.ResetUsage();
                await _unitOfWork.ReferralCodes.UpdateAsync(code);
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
