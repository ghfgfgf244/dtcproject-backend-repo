using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Training.DTOs
{
    public class AssignLocationRequestDto
    {
        [Required]
        public int AddressId { get; set; }
    }
}
