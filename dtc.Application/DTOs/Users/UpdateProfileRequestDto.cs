using System;

namespace dtc.Application.DTOs.Users
{
    public class UpdateProfileRequestDto
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        // Note: Password update is excluded as per user request
    }
}
