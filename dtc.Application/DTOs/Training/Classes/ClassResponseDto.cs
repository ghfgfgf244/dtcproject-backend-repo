using System;

namespace dtc.Application.DTOs.Training.Classes
{
    public class ClassResponseDto
    {
        public Guid Id { get; set; }
        public Guid TermId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int CurrentStudents { get; set; }
        public int MaxStudents { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
