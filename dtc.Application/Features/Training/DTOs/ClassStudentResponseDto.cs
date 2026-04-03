using System;
using dtc.Application.Features.Users.DTOs;

namespace dtc.Application.Features.Training.DTOs
{
    public class ClassStudentResponseDto : UserResponseDto
    {
        public double MorningDistanceKm { get; set; }
        public double EveningDistanceKm { get; set; }
        public double MaxMorningDistanceKm { get; set; }
        public double MaxEveningDistanceKm { get; set; }
        public Guid? DistanceRecordId { get; set; }
    }
}
