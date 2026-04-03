using System;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Auth.DTOs
{
    public class SyncUserRequestDto
    {
        [Required]
        public string ClerkId { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? FullName { get; set; }

        public string? Phone { get; set; }
        public string? AvatarUrl { get; set; }

        // Metadata from Clerk
        public string? Role { get; set; }
        public Guid? CenterId { get; set; }
    }
}
