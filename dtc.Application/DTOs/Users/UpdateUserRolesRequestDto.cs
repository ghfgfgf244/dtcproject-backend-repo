using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.DTOs.Users
{
    public class UpdateUserRolesRequestDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one role must be provided.")]
        public List<int> RoleIds { get; set; } = new();
    }
}
