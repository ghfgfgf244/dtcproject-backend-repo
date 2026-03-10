using System.ComponentModel.DataAnnotations;

namespace dtc.Application.DTOs.Location.Centers
{
    public class UpdateCenterRequestDto
    {
        [MaxLength(255)]
        public string? CenterName { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [EmailAddress]
        [MaxLength(255)]
        public string? Email { get; set; }
    }
}
