using System.ComponentModel.DataAnnotations;
using dtc.Domain.Entities;
using System.Collections.Generic;

namespace dtc.Application.Features.Training.DTOs
{
    public class UpdateRegistrationStatusDto : IValidatableObject
    {
        [Required]
        public CourseRegistrationStatus Status { get; set; }

        public string Reason { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if ((Status == CourseRegistrationStatus.Rejected || Status == CourseRegistrationStatus.Cancelled)
                && string.IsNullOrWhiteSpace(Reason))
            {
                yield return new ValidationResult(
                    "Reason is required when rejecting or cancelling a registration.",
                    new[] { nameof(Reason) });
            }
        }
    }
}
