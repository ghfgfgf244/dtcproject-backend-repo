using System.Collections.Generic;

namespace dtc.Application.Features.Users.DTOs
{
    public class UpdateUserRolesRequestDto
    {
        public List<int> RoleIds { get; set; } = new List<int>();
    }
}
