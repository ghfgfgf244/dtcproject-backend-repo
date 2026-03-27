using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Training.Interfaces;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Classes;
using dtc.Domain.Interfaces;

namespace dtc.Application.Features.Training.Services
{
    public class InstructorLeaveRequestService : IInstructorLeaveRequestService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InstructorLeaveRequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<InstructorLeaveRequestResponseDto> CreateLeaveRequestAsync(Guid instructorId, CreateInstructorLeaveRequestDto request)
        {
            var leaveRequest = new InstructorLeaveRequest(instructorId, request.LeaveDate, request.Reason);
            await _unitOfWork.InstructorLeaveRequests.AddAsync(leaveRequest);
            await _unitOfWork.SaveChangesAsync();

            return await MapToDtoAsync(leaveRequest);
        }

        public async Task<InstructorLeaveRequestResponseDto> UpdateLeaveRequestAsync(Guid id, Guid instructorId, UpdateInstructorLeaveRequestDto request)
        {
            throw new Exception("Leave requests are immutable. Please delete and recreate.");
        }

        public async Task<bool> DeleteLeaveRequestAsync(Guid id, Guid instructorId)
        {
            var leaveRequest = await _unitOfWork.InstructorLeaveRequests.GetByIdAsync(id);
            if (leaveRequest == null) throw new Exception("Leave request not found");

            if (leaveRequest.InstructorId != instructorId) throw new Exception("Unauthorized to delete this leave request");
            if (leaveRequest.Status != LeaveRequestStatus.Pending) throw new Exception("Cannot delete a non-pending leave request");

            await _unitOfWork.InstructorLeaveRequests.RemoveAsync(leaveRequest);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<InstructorLeaveRequestResponseDto> GetLeaveRequestByIdAsync(Guid id)
        {
            var leaveRequest = await _unitOfWork.InstructorLeaveRequests.GetByIdAsync(id);
            if (leaveRequest == null) throw new Exception("Leave request not found");

            return await MapToDtoAsync(leaveRequest);
        }

        public async Task<IEnumerable<InstructorLeaveRequestResponseDto>> GetMyLeaveRequestsAsync(Guid instructorId)
        {
            var requests = await _unitOfWork.InstructorLeaveRequests.FindAsync(r => r.InstructorId == instructorId);
            var dtos = new List<InstructorLeaveRequestResponseDto>();
            foreach (var r in requests)
            {
                dtos.Add(await MapToDtoAsync(r));
            }
            return dtos.OrderByDescending(d => d.LeaveDate);
        }

        public async Task<IEnumerable<InstructorLeaveRequestResponseDto>> GetAllLeaveRequestsAsync()
        {
            var requests = await _unitOfWork.InstructorLeaveRequests.GetAllAsync();
            var dtos = new List<InstructorLeaveRequestResponseDto>();
            foreach (var r in requests)
            {
                dtos.Add(await MapToDtoAsync(r));
            }
            return dtos.OrderByDescending(d => d.LeaveDate);
        }

        public async Task<bool> ApproveLeaveRequestAsync(Guid id, Guid adminId)
        {
            var leaveRequest = await _unitOfWork.InstructorLeaveRequests.GetByIdAsync(id);
            if (leaveRequest == null) throw new Exception("Leave request not found");

            leaveRequest.ChangeStatus(LeaveRequestStatus.Approved, adminId);
            await _unitOfWork.InstructorLeaveRequests.UpdateAsync(leaveRequest);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RejectLeaveRequestAsync(Guid id, Guid adminId)
        {
            var leaveRequest = await _unitOfWork.InstructorLeaveRequests.GetByIdAsync(id);
            if (leaveRequest == null) throw new Exception("Leave request not found");

            leaveRequest.ChangeStatus(LeaveRequestStatus.Rejected, adminId);
            await _unitOfWork.InstructorLeaveRequests.UpdateAsync(leaveRequest);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private async Task<InstructorLeaveRequestResponseDto> MapToDtoAsync(InstructorLeaveRequest request)
        {
            var instructor = await _unitOfWork.Users.GetByIdAsync(request.InstructorId);
            return new InstructorLeaveRequestResponseDto
            {
                Id = request.Id,
                InstructorId = request.InstructorId,
                InstructorName = instructor?.FullName ?? "Unknown",
                LeaveDate = request.LeaveDate,
                Reason = request.Reason,
                IsApproved = request.Status == LeaveRequestStatus.Approved,
                ApprovedBy = request.UpdatedBy, // Using UpdatedBy as ApprovedBy per the Domain pattern
                CreatedAt = request.CreatedAt
            };
        }
    }
}
