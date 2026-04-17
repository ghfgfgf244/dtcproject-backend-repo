using System;

namespace dtc.Application.Features.Users.DTOs
{
    public class UserStatsDto
    {
        public int TotalUsers { get; set; }
        public int StaffCount { get; set; } // TrainingManager + EnrollmentManager
        public int InstructorCount { get; set; }
        public int CollaboratorCount { get; set; }
        public int StudentCount { get; set; }
    }
}
