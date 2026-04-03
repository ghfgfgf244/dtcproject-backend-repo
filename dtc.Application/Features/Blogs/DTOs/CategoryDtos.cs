using System;

namespace dtc.Application.Features.Blogs.DTOs
{
    public class CategoryResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ParentCategoryId { get; set; }
        public bool ShowMenuStatus { get; set; }
    }

    public class CreateCategoryRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public int? ParentCategoryId { get; set; }
    }

    public class UpdateCategoryRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public int? ParentCategoryId { get; set; }
        public bool? ShowMenuStatus { get; set; }
    }
}
