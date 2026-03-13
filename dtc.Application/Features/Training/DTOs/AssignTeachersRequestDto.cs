using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Training.DTOs
{
    public class AssignTeachersRequestDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one instructor must be provided.")]
        public List<Guid> InstructorIds { get; set; } = new List<Guid>();
    }
}
