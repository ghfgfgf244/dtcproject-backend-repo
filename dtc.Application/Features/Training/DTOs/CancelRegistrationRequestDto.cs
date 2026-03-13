using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Training.DTOs
{
    public class CancelRegistrationRequestDto
    {
        [Required]
        public string Reason { get; set; } = string.Empty;
    }
}
