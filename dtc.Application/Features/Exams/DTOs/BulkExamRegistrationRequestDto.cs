using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Exams.DTOs
{
    public class BulkExamRegistrationRequestDto
    {
        [Required]
        public Guid ExamBatchId { get; set; }

        [Required]
        public List<Guid> StudentIds { get; set; } = new();

        public bool IsPaid { get; set; } = false;
    }
}
