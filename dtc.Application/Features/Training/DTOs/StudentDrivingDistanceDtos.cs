using System;

namespace dtc.Application.Features.Training.DTOs
{
    public class StudentDrivingDistanceResponseDto
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; }
        public double MorningDistanceKm { get; set; }
        public double EveningDistanceKm { get; set; }
        public double MaxMorningDistanceKm { get; set; }
        public double MaxEveningDistanceKm { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateStudentDrivingDistanceRequestDto
    {
        public Guid StudentId { get; set; }
        public double MaxMorningDistanceKm { get; set; }
        public double MaxEveningDistanceKm { get; set; }
    }

    public class UpdateStudentDrivingDistanceRequestDto
    {
        public double MorningDistanceKm { get; set; }
        public double EveningDistanceKm { get; set; }
    }
}
