using dtc.Application.Features.Location.Interfaces;
using dtc.Application.Features.Location.DTOs;
using dtc.Application.Features.Location.Interfaces;
using dtc.Application.Features.Location.DTOs;
using dtc.Application.Features.Location.Interfaces;
using dtc.Application.Features.Location.DTOs;
using dtc.Application.Features.Location.Interfaces;
using dtc.Application.Features.Location.DTOs;
using dtc.Application.Features.Location.Interfaces;
using dtc.Application.Features.Location.DTOs;
using dtc.Domain.Entities.Permissions;
using dtc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.Application.Features.Location.Services
{
    public class CenterService : ICenterService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CenterService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CenterResponseDto> CreateCenterAsync(CreateCenterRequestDto request, Guid adminId)
        {
            var phoneNumber = dtc.Domain.ValueObjects.PhoneNumber.Create(request.Phone);
            var email = dtc.Domain.ValueObjects.Email.Create(request.Email);

            // Wait, Center is in dtc.Domain.Entities.Permissions or dtc.Domain.Entities.Location?
            // In the previous view, it was dtc.Domain.Entities.Permissions namespace! Let's use var fully qualified if needed.
            var center = new dtc.Domain.Entities.Permissions.Center(
                name: request.CenterName,
                address: request.Address,
                phone: phoneNumber,
                email: email,
                numberOfClasses: request.NumberOfClasses > 0 ? request.NumberOfClasses : 1,
                maxStudentPerClass: request.MaxStudentPerClass > 0 ? request.MaxStudentPerClass : 1,
                createdBy: adminId
            );

            await _unitOfWork.Centers.AddAsync(center);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(center);
        }

        public async Task<CenterResponseDto> UpdateCenterAsync(Guid id, UpdateCenterRequestDto request, Guid adminId)
        {
            var center = await _unitOfWork.Centers.GetByIdAsync(id);
            if (center == null) throw new Exception("Center not found");

            dtc.Domain.ValueObjects.PhoneNumber? phone = null;
            if (!string.IsNullOrWhiteSpace(request.Phone))
                phone = dtc.Domain.ValueObjects.PhoneNumber.Create(request.Phone);

            dtc.Domain.ValueObjects.Email? email = null;
            if (!string.IsNullOrWhiteSpace(request.Email))
                email = dtc.Domain.ValueObjects.Email.Create(request.Email);

            bool isUpdated = center.UpdateInfo(
                name: request.CenterName,
                address: request.Address,
                phone: phone,
                email: email,
                updatedBy: adminId
            );

            if (isUpdated)
            {
                await _unitOfWork.Centers.UpdateAsync(center);
                await _unitOfWork.SaveChangesAsync();
            }

            return MapToDto(center);
        }

        public async Task<bool> DeactivateCenterAsync(Guid id, Guid adminId)
        {
            var center = await _unitOfWork.Centers.GetByIdAsync(id);
            if (center == null) throw new Exception("Center not found");

            // Check if there are active courses in this center before deleting
            var courses = await _unitOfWork.Courses.FindAsync(c => c.CenterId == id && c.IsActive);
            if (courses != null && courses.Any())
            {
                throw new InvalidOperationException("Cannot delete center because there are active courses in this center.");
            }

            center.SoftDelete(adminId);
            await _unitOfWork.Centers.UpdateAsync(center);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<CenterResponseDto>> GetAllCentersAsync()
        {
            var centers = await _unitOfWork.Centers.GetAllAsync();
            return centers.Select(MapToDto).ToList();
        }

        public async Task<CenterResponseDto> GetCenterDetailAsync(Guid id)
        {
            // FirstOrDefaultAsync with Users included if we want
            var centers = await _unitOfWork.Centers.FindAsync(c => c.Id == id);
            var center = centers.FirstOrDefault();
            
            if (center == null) throw new Exception("Center not found");

            return MapToDto(center);
        }

        public async Task<bool> AssignUsersToCenterAsync(Guid centerId, AssignUsersRequestDto request, Guid adminId)
        {
            var center = await _unitOfWork.Centers.GetByIdAsync(centerId);
            if (center == null) throw new Exception("Center not found");

            var requestedUserIds = (request.UserIds ?? new List<Guid>()).Distinct().ToList();

            var existingLinks = await _unitOfWork.UserCenters.FindAsync(uc => uc.CenterId == centerId);
            var existingUserIds = existingLinks.Select(uc => uc.UserId).ToHashSet();

            var toRemove = existingLinks.Where(uc => !requestedUserIds.Contains(uc.UserId)).ToList();
            if (toRemove.Count > 0)
                await _unitOfWork.UserCenters.RemoveRange(toRemove);

            var toAddIds = requestedUserIds.Where(id => !existingUserIds.Contains(id)).ToList();
            if (toAddIds.Count > 0)
            {
                var users = await _unitOfWork.Users.FindAsync(u => toAddIds.Contains(u.Id));
                var foundIds = users.Select(u => u.Id).ToHashSet();
                var missing = toAddIds.Where(id => !foundIds.Contains(id)).ToList();
                if (missing.Count > 0)
                    throw new InvalidOperationException("One or more users were not found.");

                foreach (var userId in toAddIds)
                    await _unitOfWork.UserCenters.AddAsync(new UserCenter(userId, centerId));
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private CenterResponseDto MapToDto(dtc.Domain.Entities.Permissions.Center center)
        {
            return new CenterResponseDto
            {
                Id = center.Id,
                CenterName = center.CenterName,
                Address = center.Address,
                Phone = center.Phone.Value,
                Email = center.Email.Value,
                IsActive = center.IsActive,
                CreatedAt = center.CreatedAt
            };
        }
    }
}
