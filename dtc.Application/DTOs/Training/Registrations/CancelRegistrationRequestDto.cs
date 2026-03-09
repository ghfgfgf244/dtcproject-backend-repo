using System.ComponentModel.DataAnnotations;

namespace dtc.Application.DTOs.Training.Registrations
{
    public class CancelRegistrationRequestDto
    {
        [Required]
        public string Reason { get; set; } = string.Empty;
    }
}
