using System;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Training.DTOs
{
    public class UploadedFileDto
    {
        public required string FileName { get; set; }
        public required string Extension { get; set; }
        public required string ResourceType { get; set; }
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public int Length => Content.Length;
    }

    public class RegisterCourseRequestDto
    {
        [Required]
        public Guid CourseId { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalFee { get; set; }

        public string? Notes { get; set; }

        public string? ReferralCode { get; set; }

        public UploadedFileDto? Photo { get; set; }
        public UploadedFileDto? IdFront { get; set; }
        public UploadedFileDto? IdBack { get; set; }
    }
}
