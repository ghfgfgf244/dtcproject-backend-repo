using System;
using System.Collections.Generic;

namespace dtc.Application.Features.Training.DTOs
{
    public class ReassignRegistrationTermRequestDto
    {
        public Guid TermId { get; set; }
    }

    public class CourseRegistrationTermOptionDto
    {
        public Guid Id { get; set; }
        public string TermName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CurrentStudents { get; set; }
        public int MaxStudents { get; set; }
        public bool IsCurrentAssignment { get; set; }
        public bool IsLateForAutoPlacement { get; set; }
    }
}
