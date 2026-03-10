using System.ComponentModel.DataAnnotations;

namespace dtc.Application.DTOs.Exams
{
    public class UpdateExamResultRequestDto
    {
        [Required]
        [Range(0, 100)]
        public double Score { get; set; }
    }
}
