using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.DTOs.Location.Centers
{
    public class AssignUsersRequestDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one user must be provided.")]
        public List<Guid> UserIds { get; set; } = new List<Guid>();
    }
}
