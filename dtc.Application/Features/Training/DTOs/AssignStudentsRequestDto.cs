using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Training.DTOs
{
    public class AssignStudentsRequestDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one student must be provided.")]
        public List<Guid> StudentIds { get; set; } = new List<Guid>();
    }
}
