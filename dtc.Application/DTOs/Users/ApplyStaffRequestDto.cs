using System;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.DTOs.Users
{
    public class ApplyStaffRequestDto
    {
        public string? FullName { get; set; }
        
        [Required]
        public string Phone { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }

        [Required]
        public int RoleId { get; set; } // Should be Instructor (3) or Collaborator (5)
    }
}
