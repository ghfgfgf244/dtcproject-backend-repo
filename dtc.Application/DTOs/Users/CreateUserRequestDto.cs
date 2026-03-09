using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.DTOs.Users
{
    public class CreateUserRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        // Admin can assign roles right at creation
        public List<int>? RoleIds { get; set; }
    }
}
