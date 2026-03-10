using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Training.DTOs
{
    public class AssignLocationRequestDto
    {
        [Required]
        [MaxLength(255)]
        public string Location { get; set; } = string.Empty;
    }
}
