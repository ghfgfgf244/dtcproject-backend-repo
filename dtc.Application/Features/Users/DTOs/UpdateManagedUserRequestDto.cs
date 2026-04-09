namespace dtc.Application.Features.Users.DTOs
{
    public class UpdateManagedUserRequestDto
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public bool? IsActive { get; set; }
    }
}
