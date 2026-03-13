using System;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Training.DTOs
{
    public class UpdateTermRequestDto
    {
        [MaxLength(255)]
        public string? TermName { get; set; }

        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
    }
}
