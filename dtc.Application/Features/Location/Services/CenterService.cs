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
using dtc.Domain.Entities.Location; // Ensure correct namespace depending on Center entity
using dtc.Domain.Entities.Permissions; // Required for Center/User if they are there
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
            // Wait, IGenericRepository doesn't support Include inside GetByIdAsync, but we can use FindAsync.
            var centers = await _unitOfWork.Centers.FindAsync(c => c.Id == id, c => c.Users);
            var center = centers.FirstOrDefault();
            
            if (center == null) throw new Exception("Center not found");

            return MapToDto(center);
        }

        public async Task<bool> AssignUsersToCenterAsync(Guid centerId, AssignUsersRequestDto request, Guid adminId)
        {
            var centers = await _unitOfWork.Centers.FindAsync(c => c.Id == centerId, c => c.Users);
            var center = centers.FirstOrDefault();
            if (center == null) throw new Exception("Center not found");

            if (request.UserIds == null || !request.UserIds.Any())
            {
                center.SyncUsers(new List<User>(), adminId);
            }
            else
            {
                // Fetch valid users
                var users = await _unitOfWork.Users.FindAsync(u => request.UserIds.Contains(u.Id));
                if (users.Count() != request.UserIds.Count)
                {
                    // Optionally throw error if some IDs are invalid, but bulk assign often just ignores invalid ones
                    // We'll proceed with valid users
                }
                
                center.SyncUsers(users, adminId);
            }

            await _unitOfWork.Centers.UpdateAsync(center);
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
