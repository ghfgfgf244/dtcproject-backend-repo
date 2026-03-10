using dtc.Application.DTOs.Collaborators;
using dtc.Application.DTOs.Users;
using dtc.Application.Interfaces;
using dtc.Application.Interfaces.Collaborators;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Collaborators;
using dtc.Domain.Entities.Terms;
using dtc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.Application.Services.Collaborators
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
        public async Task<ReferralCodeResponseDto> GetMyReferralCodeAsync(Guid collaboratorId)
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

        public async Task<ReferralCodeResponseDto> GenerateReferralCodeAsync(Guid collaboratorId, string code)
        {
            var existing = await _unitOfWork.ReferralCodes.FindAsync(c => c.Code == code.Trim().ToUpper());
            if (existing.Any()) throw new Exception("Referral code already exists.");

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
            // Returning fixed mapping rate.
            return Task.FromResult(STANDARD_COMMISSION_RATE * 100);
        }

        // DEV-131: Calculate commission
        public async Task<IEnumerable<CollaboratorCommissionResponseDto>> CalculateAndGetCommissionsAsync(Guid collaboratorId)
        {
            var codes = await _unitOfWork.ReferralCodes.FindAsync(c => c.CollaboratorId == collaboratorId);
            var code = codes.FirstOrDefault();
            if (code == null) throw new Exception("No referral code found for this collaborator.");

            // 1. Get all students referred by this collaborator
            var referrals = await _unitOfWork.ReferralRegistrations.FindAsync(r => r.ReferralCodeId == code.Id);
            var studentIds = referrals.Select(r => r.StudentId).ToList();

            // 2. Get approved course registrations for these students
            var courseRegs = await _unitOfWork.CourseRegistrations.FindAsync(cr => studentIds.Contains(cr.UserId) && cr.Status == CourseRegistrationStatus.Approved);

            // Calculate theoretical total
            decimal totalEligibleRevenue = courseRegs.Sum(cr => cr.TotalFee);
            decimal calculatedCommission = totalEligibleRevenue * STANDARD_COMMISSION_RATE;

            // Simple logic: we generate a single aggregated commission record representing the calculation (if amount > 0)
            // Or typically we calculate per registration. Here we just return the existing ones + a newly calculated pending line if missing.
            
            // For simplicity of this module: we check existing commissions to prevent duplicates or just display it.
            var existingCommissions = await _unitOfWork.CollaboratorCommissions.FindAsync(c => c.CollaboratorId == collaboratorId);

            // If calculated Commission > sum of existing amounts, we could create a new record for the difference.
            decimal existingSum = existingCommissions.Sum(c => c.Amount);
            if (calculatedCommission > existingSum)
            {
                var newCommissionAmount = calculatedCommission - existingSum;
                var newComm = new CollaboratorCommission(collaboratorId, newCommissionAmount);
                await _unitOfWork.CollaboratorCommissions.AddAsync(newComm);
                await _unitOfWork.SaveChangesAsync();
                
                // Fetch updated
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
    }
}
