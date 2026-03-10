using System.ComponentModel.DataAnnotations;

namespace dtc.Application.DTOs.Training.Schedules
{
    public class AssignLocationRequestDto
    {
        [Required]
        [MaxLength(255)]
        public string Location { get; set; } = string.Empty;
    }
}
