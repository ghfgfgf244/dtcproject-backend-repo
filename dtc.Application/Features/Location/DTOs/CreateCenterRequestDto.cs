using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Location.DTOs
{
    public class CreateCenterRequestDto
    {
        [Required]
        [MaxLength(255)]
        public string CenterName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;
    }
}
