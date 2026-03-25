using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dtc.Application.Features.Training.DTOs;

namespace dtc.Application.Features.Training.Interfaces
{
    public interface IInstructorLeaveRequestService
    {
        Task<InstructorLeaveRequestResponseDto> CreateLeaveRequestAsync(Guid instructorId, CreateInstructorLeaveRequestDto request);
        Task<InstructorLeaveRequestResponseDto> UpdateLeaveRequestAsync(Guid id, Guid instructorId, UpdateInstructorLeaveRequestDto request);
        Task<bool> DeleteLeaveRequestAsync(Guid id, Guid instructorId);
        Task<InstructorLeaveRequestResponseDto> GetLeaveRequestByIdAsync(Guid id);
        Task<IEnumerable<InstructorLeaveRequestResponseDto>> GetMyLeaveRequestsAsync(Guid instructorId);
        Task<IEnumerable<InstructorLeaveRequestResponseDto>> GetAllLeaveRequestsAsync();
        Task<bool> ApproveLeaveRequestAsync(Guid id, Guid adminId);
        Task<bool> RejectLeaveRequestAsync(Guid id, Guid adminId);
    }
}
