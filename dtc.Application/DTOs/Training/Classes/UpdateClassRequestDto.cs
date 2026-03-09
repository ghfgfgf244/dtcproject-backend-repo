using System.ComponentModel.DataAnnotations;
using dtc.Domain.Entities;

namespace dtc.Application.DTOs.Training.Classes
{
    public class UpdateClassRequestDto
    {
        [MaxLength(255)]
        public string? ClassName { get; set; }

        [Range(1, int.MaxValue)]
        public int? MaxStudents { get; set; }

        public ClassStatus? Status { get; set; }
    }
}
