using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Exams.DTOs
{
    public class UpdateExamResultRequestDto
    {
        [Required]
        [Range(0, 100)]
        public double Score { get; set; }
    }
}
