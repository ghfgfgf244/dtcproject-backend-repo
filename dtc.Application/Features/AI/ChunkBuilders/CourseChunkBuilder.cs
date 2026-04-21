using System.Collections.Generic;
using System.Text;
using dtc.Domain.Entities.Training;

namespace dtc.Application.Features.AI.ChunkBuilders
{
    internal static class CourseChunkBuilder
    {
        public static KnowledgeChunkPayload Build(Course course)
        {
            var text = new StringBuilder()
                .AppendLine("Loai tai lieu: Khoa hoc dao tao lai xe.")
                .AppendLine($"Ten khoa hoc: {course.CourseName}")
                .AppendLine($"Hang GPLX: {course.LicenseType}")
                .AppendLine($"Trung tam: {course.Center?.CenterName ?? "Khong ro"}")
                .AppendLine($"Dia chi trung tam: {course.Center?.Address ?? "Khong ro"}")
                .AppendLine($"Thoi luong: {course.DurationInWeeks} tuan")
                .AppendLine($"Si so toi da: {course.MaxStudents}")
                .AppendLine($"Hoc phi: {course.Price:0.##}")
                .AppendLine($"Mo ta: {course.Description}")
                .ToString();

            return new KnowledgeChunkPayload
            {
                Id = $"course-{course.Id}",
                Text = text,
                Metadata = new Dictionary<string, string>
                {
                    ["documentType"] = "course",
                    ["referenceId"] = course.Id.ToString(),
                    ["title"] = course.CourseName,
                    ["courseId"] = course.Id.ToString(),
                    ["courseName"] = course.CourseName,
                    ["examLevel"] = course.LicenseType.ToString(),
                    ["centerId"] = course.CenterId.ToString(),
                    ["centerName"] = course.Center?.CenterName ?? string.Empty
                }
            };
        }
    }
}
