using System;
using System.Reflection;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Blogs;
using dtc.Domain.Entities.Classes;
using dtc.Domain.Entities.Exams;
using dtc.Domain.Entities.Location;
using dtc.Domain.Entities.Notifications;
using dtc.Domain.Entities.Training;

namespace dtc.Infrastructure.Persistence.Seeding
{
    internal static class MongoSeedData
    {
        public static IReadOnlyCollection<Blog> Blogs => new[]
        {
            SetBaseFields(
                new Blog(
                    title: "Kinh nghiem hoc lai xe trong thanh pho",
                    categoryId: 1,
                    content: "Tong hop kinh nghiem giu khoang cach, vao bai va xu ly tinh huong giao thong do thi.",
                    createdBy: SeedIds.UserA,
                    summary: "Mau bai viet gioi thieu kinh nghiem hoc lai xe.",
                    avatar: "https://cdn.example.com/blog/kinh-nghiem-lai-xe.jpg"),
                SeedIds.BlogA,
                new DateTime(2026, 1, 20, 8, 0, 0, DateTimeKind.Utc),
                SeedIds.UserA),
            SetBaseFields(
                new Blog(
                    title: "Checklist truoc ngay thi sat hach",
                    categoryId: 2,
                    content: "Kiem tra ho so, lich thi, suc khoe va thoi gian co mat tai trung tam truoc ky thi.",
                    createdBy: SeedIds.UserA,
                    summary: "Mau bai viet nhac viec truoc ky thi.",
                    avatar: "https://cdn.example.com/blog/checklist-thi.jpg"),
                SeedIds.BlogB,
                new DateTime(2026, 1, 21, 8, 0, 0, DateTimeKind.Utc),
                SeedIds.UserA)
        };

        public static IReadOnlyCollection<Category> Categories => new[]
        {
            SetCategoryFields(
                new Category("Tin dao tao", SeedIds.UserA),
                SeedIds.CategoryA,
                1,
                new DateTime(2026, 1, 18, 8, 0, 0, DateTimeKind.Utc),
                SeedIds.UserA),
            SetCategoryFields(
                new Category("Huong dan thi sat hach", SeedIds.UserA),
                SeedIds.CategoryB,
                2,
                new DateTime(2026, 1, 19, 8, 0, 0, DateTimeKind.Utc),
                SeedIds.UserA)
        };

        public static IReadOnlyCollection<Question> Questions => new[]
        {
            SetProperty(new Question(
                content: "Khi gap den vang nhap nhay, nguoi lai xe phai lam gi?",
                correctAnswer: AnswerOption.A,
                a: "Giam toc do va quan sat ky.",
                b: "Tang toc vuot nhanh.",
                c: "Dung giua nga tu.",
                d: "Bat den khan cap."),
                "Id",
                1),
            SetProperty(new Question(
                content: "Khoang cach an toan trong do thi duoc uu tien theo yeu to nao?",
                correctAnswer: AnswerOption.B,
                a: "Mau xe.",
                b: "Toc do va mat do giao thong.",
                c: "So ghe tren xe.",
                d: "Muc xang."),
                "Id",
                2)
        };

        public static IReadOnlyCollection<SampleExam> SampleExams => new[]
        {
            SetProperty(
                new SampleExam(SeedIds.CourseA, 1, ExamLevel.B, 25, 32),
                "Id",
                SeedIds.SampleExamA),
            SetProperty(
                new SampleExam(SeedIds.CourseB, 2, ExamLevel.C, 30, 28),
                "Id",
                SeedIds.SampleExamB)
        };

        public static IReadOnlyCollection<SampleExamQuestion> SampleExamQuestions => new[]
        {
            SetProperty(new SampleExamQuestion(SeedIds.SampleExamA, 1, 1), "Id", SeedIds.SampleExamQuestionA),
            SetProperty(new SampleExamQuestion(SeedIds.SampleExamB, 2, 1), "Id", SeedIds.SampleExamQuestionB)
        };

        public static IReadOnlyCollection<Address> Addresses => new[]
        {
            new Address(1, "Bai tap sa hinh Trung tam Quan 1"),
            new Address(2, "San tap Thu Duc - khu B")
        };

        public static IReadOnlyCollection<LearningLocation> LearningLocations => new[]
        {
            SetProperty(new LearningLocation(SeedIds.ClassScheduleA, 1), "Id", SeedIds.LearningLocationA),
            SetProperty(new LearningLocation(SeedIds.ClassScheduleB, 2), "Id", SeedIds.LearningLocationB)
        };

        public static IReadOnlyCollection<Notification> Notifications => new[]
        {
            SetBaseFields(
                new Notification(
                    title: "Cap nhat lich hoc B2-01-Q1",
                    content: "Buoi hoc thuc hanh ngay 15/03 se tap trung tai san tap Quan 1.",
                    type: NotificationType.Class,
                    createdBy: SeedIds.UserA,
                    centerId: SeedIds.CenterA,
                    target: new[] { UserRole.Student, UserRole.Instructor }),
                SeedIds.NotificationA,
                new DateTime(2026, 3, 10, 8, 0, 0, DateTimeKind.Utc),
                SeedIds.UserA),
            SetBaseFields(
                new Notification(
                    title: "Mo dang ky ky thi thang 05",
                    content: "Hoc vien du dieu kien co the dang ky ky thi sat hach thang 05 tren he thong.",
                    type: NotificationType.Exam,
                    createdBy: SeedIds.UserA,
                    centerId: null,
                    target: new[] { UserRole.Student }),
                SeedIds.NotificationB,
                new DateTime(2026, 3, 11, 8, 0, 0, DateTimeKind.Utc),
                SeedIds.UserA)
        };

        public static IReadOnlyCollection<UserNotification> UserNotifications => new[]
        {
            SetProperty(new UserNotification(SeedIds.NotificationA, SeedIds.UserB), "Id", SeedIds.UserNotificationA),
            SetProperty(new UserNotification(SeedIds.NotificationB, SeedIds.UserA), "Id", SeedIds.UserNotificationB)
        };

        public static IReadOnlyCollection<LearningRoadmap> LearningRoadmaps => new[]
        {
            SetProperty(
                new LearningRoadmap(SeedIds.CourseA, "Lam quen xe va sa hinh", "Buoc 1 la nhan dien xe, ghe ngoi, guong va bai xuat phat.", 1),
                "Id",
                SeedIds.LearningRoadmapA),
            SetProperty(
                new LearningRoadmap(SeedIds.CourseB, "Duong truong nang cao", "Tap trung vao len doc, de-pa, vao bai ghep xe va xu ly tinh huong xe tai.", 2),
                "Id",
                SeedIds.LearningRoadmapB)
        };

        public static IReadOnlyCollection<ResourceLearning> ResourceLearnings => new[]
        {
            SetBaseFields(
                new ResourceLearning(SeedIds.CourseA, ResourceType.Video, "Video huong dan bai de-pa", "https://cdn.example.com/resources/de-pa.mp4", SeedIds.UserA),
                SeedIds.ResourceLearningA,
                new DateTime(2026, 1, 25, 8, 0, 0, DateTimeKind.Utc),
                SeedIds.UserA),
            SetBaseFields(
                new ResourceLearning(SeedIds.CourseB, ResourceType.Pdf, "Tai lieu duong truong an toan", "https://cdn.example.com/resources/duong-truong.pdf", SeedIds.UserA),
                SeedIds.ResourceLearningB,
                new DateTime(2026, 1, 26, 8, 0, 0, DateTimeKind.Utc),
                SeedIds.UserA)
        };

        private static T SetBaseFields<T>(T entity, Guid id, DateTime createdAt, Guid? createdBy) where T : BaseEntity
        {
            SetProperty(entity, "Id", id);
            SetProperty(entity, "CreatedAt", createdAt);
            SetProperty(entity, "CreatedBy", createdBy);
            SetProperty(entity, "IsDeleted", false);
            return entity;
        }

        private static Category SetCategoryFields(Category entity, Guid id, int categoryId, DateTime createdAt, Guid? createdBy)
        {
            SetBaseFields(entity, id, createdAt, createdBy);
            SetProperty(entity, "CategoryId", categoryId);
            return entity;
        }

        private static T SetProperty<T>(T entity, string propertyName, object? value)
        {
            var type = entity!.GetType();
            var property = GetProperty(type, propertyName)
                ?? throw new InvalidOperationException($"Property '{propertyName}' was not found on type '{type.FullName}'.");

            var setter = property.SetMethod ?? property.GetSetMethod(true);
            if (setter != null)
            {
                setter.Invoke(entity, new[] { value });
                return entity;
            }

            var backingField = GetField(type, $"<{propertyName}>k__BackingField");
            if (backingField != null)
            {
                backingField.SetValue(entity, value);
                return entity;
            }

            throw new InvalidOperationException(
                $"Property '{propertyName}' on type '{type.FullName}' does not have a writable setter or backing field.");
        }

        private static PropertyInfo? GetProperty(Type type, string propertyName)
        {
            while (type != typeof(object) && type != null)
            {
                var property = type.GetProperty(
                    propertyName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (property != null)
                {
                    return property;
                }

                type = type.BaseType!;
            }

            return null;
        }

        private static FieldInfo? GetField(Type type, string fieldName)
        {
            while (type != typeof(object) && type != null)
            {
                var field = type.GetField(
                    fieldName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (field != null)
                {
                    return field;
                }

                type = type.BaseType!;
            }

            return null;
        }
    }
}
