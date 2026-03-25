using System;

namespace dtc.Application.Features.Training.DTOs
{
    public class InstructorLeaveRequestResponseDto
    {
        public Guid Id { get; set; }
        public Guid InstructorId { get; set; }
        public string InstructorName { get; set; } // Optional, populated if joined
        public DateTime LeaveDate { get; set; }
        public string Reason { get; set; }
        public bool IsApproved { get; set; }
        public Guid? ApprovedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateInstructorLeaveRequestDto
    {
        public DateTime LeaveDate { get; set; }
        public string Reason { get; set; }
    }

    public class UpdateInstructorLeaveRequestDto
    {
        public DateTime LeaveDate { get; set; }
        public string Reason { get; set; }
    }
}
