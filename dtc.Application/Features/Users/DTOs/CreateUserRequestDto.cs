using System.Collections.Generic;

namespace dtc.Application.Features.Users.DTOs
{
    public class CreateUserRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public List<int>? RoleIds { get; set; }
    }
}
