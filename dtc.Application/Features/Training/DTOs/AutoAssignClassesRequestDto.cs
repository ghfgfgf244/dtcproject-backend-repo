using System;
using System.ComponentModel.DataAnnotations;
using dtc.Domain.Entities;

namespace dtc.Application.Features.Training.DTOs
{
    public class AutoAssignClassesRequestDto
    {
        [Required]
        public Guid TermId { get; set; }

        [Required]
        public ClassType ClassType { get; set; }
    }
}
