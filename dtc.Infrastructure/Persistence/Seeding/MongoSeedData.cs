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
            CreatePublishedBlog(
                SeedIds.BlogA,
                new DateTime(2026, 3, 20, 8, 0, 0, DateTimeKind.Utc),
                "5 bước chuẩn bị trước buổi học thực hành đầu tiên",
                1,
                "Học viên nên mang theo giấy tờ tùy thân, nước uống, giày dép gọn gàng và đến sớm khoảng 15 phút để nghe hướng dẫn an toàn trước khi lên xe tập. Ngay khi bắt đầu buổi học, cần chỉnh ghế ngồi, gương chiếu hậu, dây an toàn và làm quen vị trí chân ga, chân phanh. Việc chuẩn bị kỹ từ đầu sẽ giúp học viên đỡ áp lực và tự tin hơn trong buổi thực hành đầu tiên.",
                "Danh sách chuẩn bị ngắn gọn giúp học viên bước vào buổi thực hành đầu tiên tự tin và an toàn hơn.",
                "https://cdn.example.com/blog/chuan-bi-buoi-hoc-thuc-hanh.jpg"),
            CreatePublishedBlog(
                SeedIds.BlogB,
                new DateTime(2026, 3, 22, 8, 0, 0, DateTimeKind.Utc),
                "Hướng dẫn ôn tập lý thuyết hiệu quả trong 7 ngày",
                2,
                "Nên chia bộ câu hỏi thành từng nhóm như biển báo, sa hình, quy tắc giao thông và tình huống nguy hiểm. Mỗi ngày học một nhóm nhỏ, làm đề ngắn rồi ghi lại các câu sai để xem vì sao đáp án đúng. Cách học theo nhóm lỗi sai và lặp lại theo chu kỳ sẽ giúp nhớ lâu hơn so với học thuộc máy móc.",
                "Lộ trình 7 ngày ôn tập lý thuyết gọn, dễ nhớ và bám sát cấu trúc đề sát hạch.",
                "https://cdn.example.com/blog/on-tap-ly-thuyet-7-ngay.jpg"),
            CreatePublishedBlog(
                SeedIds.BlogC,
                new DateTime(2026, 3, 24, 8, 0, 0, DateTimeKind.Utc),
                "Lịch mở rộng sân tập cuối tuần cho học viên ô tô",
                3,
                "Trung tâm mở thêm các khung giờ 07:30, 09:30 và 14:00 vào thứ Bảy, Chủ nhật để học viên đang học các khóa ô tô đăng ký tập sa hình. Học viên cần đặt lịch trước trên hệ thống để được sắp xe và phân công giảng viên. Các ca cuối tuần ưu tiên cho học viên đang gần đến kỳ thi hoặc cần ôn tập các bài khó như dừng xe ngang dốc, ghép dọc và ghép ngang.",
                "Thông báo các khung giờ mở rộng sân tập cuối tuần để học viên chủ động đăng ký.",
                "https://cdn.example.com/blog/lich-san-tap-cuoi-tuan.jpg"),
            CreatePublishedBlog(
                SeedIds.BlogD,
                new DateTime(2026, 3, 26, 8, 0, 0, DateTimeKind.Utc),
                "3 lỗi khiến học viên dễ mất điểm trong bài thi sa hình",
                2,
                "Ba lỗi thường gặp nhất là quên xi nhan đúng thời điểm, canh bánh xe chưa đều khi ghép xe và xử lý chân côn hoặc chân phanh chưa ổn định ở bài dừng ngang dốc. Để giảm rủi ro, học viên nên luyện từng bài riêng, ghi nhớ mốc canh và hỏi giảng viên ngay khi chưa hiểu vì sao bị trừ điểm.",
                "Tổng hợp các lỗi phổ biến trong sa hình và cách khắc phục để hạn chế mất điểm.",
                "https://cdn.example.com/blog/loi-mat-diem-sa-hinh.jpg"),
            CreatePublishedBlog(
                SeedIds.BlogE,
                new DateTime(2026, 3, 28, 8, 0, 0, DateTimeKind.Utc),
                "Kinh nghiệm giữ bình tĩnh khi bước vào phòng thi lý thuyết",
                4,
                "Trước giờ thi, học viên nên đến sớm, kiểm tra giấy tờ và hít thở sâu để ổn định tâm lý. Khi làm bài, hãy ưu tiên các câu dễ trước, không dành quá lâu cho một câu khó và luôn đọc kỹ từ khóa phủ định như “không được”, “nghiêm cấm”, “phải”. Giữ nhịp làm bài đều sẽ giúp hạn chế áp lực thời gian.",
                "Một số mẹo đơn giản giúp học viên ổn định tâm lý và làm bài lý thuyết hiệu quả hơn.",
                "https://cdn.example.com/blog/giu-binh-tinh-khi-thi-ly-thuyet.jpg"),
            CreatePublishedBlog(
                SeedIds.BlogF,
                new DateTime(2026, 3, 30, 8, 0, 0, DateTimeKind.Utc),
                "Nên chọn học gần nhà hay học tại trung tâm có sân tập tốt",
                5,
                "Nếu lịch cá nhân bận rộn và cần di chuyển thuận tiện, học gần nhà sẽ giúp duy trì đều buổi học. Tuy nhiên, với học viên ô tô cần luyện sa hình hoặc đường trường nhiều, việc ưu tiên trung tâm có sân tập chuẩn, xe tập ổn định và giảng viên phù hợp sẽ giúp tiến độ học hiệu quả hơn. Tốt nhất nên cân nhắc đồng thời quãng đường đi lại, khung giờ học và chất lượng cơ sở vật chất.",
                "So sánh hai hướng chọn trung tâm học lái xe để học viên dễ đưa ra quyết định phù hợp.",
                "https://cdn.example.com/blog/chon-trung-tam-hoc-lai-xe.jpg")
        };

        public static IReadOnlyCollection<Category> Categories => new[]
        {
            SetCategoryFields(
                new Category("Cẩm nang học viên", SeedIds.UserA),
                SeedIds.CategoryA,
                1,
                new DateTime(2026, 3, 18, 8, 0, 0, DateTimeKind.Utc),
                SeedIds.UserA),
            SetCategoryFields(
                new Category("Ôn tập sát hạch", SeedIds.UserA),
                SeedIds.CategoryB,
                2,
                new DateTime(2026, 3, 19, 8, 0, 0, DateTimeKind.Utc),
                SeedIds.UserA),
            SetCategoryFields(
                new Category("Thông báo trung tâm", SeedIds.UserA),
                SeedIds.CategoryC,
                3,
                new DateTime(2026, 3, 20, 8, 0, 0, DateTimeKind.Utc),
                SeedIds.UserA),
            SetCategoryFields(
                new Category("Kinh nghiệm thi cử", SeedIds.UserA),
                SeedIds.CategoryD,
                4,
                new DateTime(2026, 3, 21, 8, 0, 0, DateTimeKind.Utc),
                SeedIds.UserA),
            SetCategoryFields(
                new Category("Tư vấn chọn khóa học", SeedIds.UserA),
                SeedIds.CategoryE,
                5,
                new DateTime(2026, 3, 22, 8, 0, 0, DateTimeKind.Utc),
                SeedIds.UserA)
        };

        public static IReadOnlyCollection<Address> Addresses => new[]
        {
            new Address(1, "53 Phan Đăng Lưu, Hải Châu, Đà Nẵng - Trung tâm Đà Nẵng STC"),
            new Address(2, "98 Núi Thành, Hải Châu, Đà Nẵng - Trung tâm đào tạo lái xe 579"),
            new Address(3, "113 Núi Thành, Hải Châu, Đà Nẵng - Trung tâm đào tạo lái xe Masco"),
            new Address(4, "224 Lê Trọng Tấn, Cẩm Lệ, Đà Nẵng - Trung tâm đào tạo lái xe Miền Trung"),
            new Address(5, "Lô 19 Khu D10 Nguyễn Sinh Sắc, Liên Chiểu, Đà Nẵng - Trung tâm đào tạo lái xe Sao Vàng"),
            new Address(6, "75 Nguyễn Lương Bằng, Liên Chiểu, Đà Nẵng - Trung tâm đào tạo lái xe Liên Chiểu"),
            new Address(7, "Sân tập sa hình Hòa Cầm, Cẩm Lệ, Đà Nẵng"),
            new Address(8, "Sân tập đường trường Hòa Xuân, Cẩm Lệ, Đà Nẵng"),
            new Address(9, "Bãi thi mô phỏng nội bộ Hải Châu, Đà Nẵng"),
            new Address(10, "Khu thực hành ghép xe và đề-pa Liên Chiểu, Đà Nẵng")
        };

        public static IReadOnlyCollection<LearningLocation> LearningLocations => new[]
        {
            SetProperty(new LearningLocation(SeedIds.ClassScheduleA, 1), "Id", SeedIds.LearningLocationA),
            SetProperty(new LearningLocation(SeedIds.ClassScheduleA, 7), "Id", SeedIds.LearningLocationB),
            SetProperty(new LearningLocation(SeedIds.ClassScheduleB, 2), "Id", SeedIds.LearningLocationC),
            SetProperty(new LearningLocation(SeedIds.ClassScheduleB, 8), "Id", SeedIds.LearningLocationD),
            SetProperty(new LearningLocation(SeedIds.ClassScheduleB, 9), "Id", SeedIds.LearningLocationE)
        };

        public static IReadOnlyCollection<LearningRoadmap> LearningRoadmaps => new[]
        {
            SetProperty(
                new LearningRoadmap(SeedIds.CourseA, "Làm quen xe và tư thế lái chuẩn", "Học viên nhận diện các bộ phận cơ bản của xe, chỉnh ghế ngồi, gương chiếu hậu, vô lăng và thực hành quy trình khởi động an toàn.", 1),
                "Id",
                SeedIds.LearningRoadmapA),
            SetProperty(
                new LearningRoadmap(SeedIds.CourseA, "Thực hành sa hình cơ bản", "Tập trung vào xuất phát, dừng xe đúng vị trí, ghép xe cơ bản và làm quen với mốc canh trong sân tập.", 2),
                "Id",
                SeedIds.LearningRoadmapB),
            SetProperty(
                new LearningRoadmap(SeedIds.CourseA, "Ôn tập trước kỳ thi và rà lỗi", "Tổng hợp các lỗi thường gặp, tăng thời lượng chạy bài hoàn chỉnh và luyện phản xạ xử lý tình huống trước ngày thi.", 3),
                "Id",
                SeedIds.LearningRoadmapC),
            SetProperty(
                new LearningRoadmap(SeedIds.CourseB, "Kiến thức nền tảng và an toàn khi điều khiển xe tải", "Giới thiệu đặc thù xe tải, điểm mù, khoảng phanh, cách quan sát và quy trình kiểm tra xe trước khi xuất phát.", 1),
                "Id",
                SeedIds.LearningRoadmapD),
            SetProperty(
                new LearningRoadmap(SeedIds.CourseB, "Thực hành đường trường và bài thi nâng cao", "Tập trung vào lên dốc, đề-pa, ghép xe, xử lý cua gắt, giữ làn và phối hợp số phù hợp với tải trọng xe.", 2),
                "Id",
                SeedIds.LearningRoadmapE),
            SetProperty(
                new LearningRoadmap(SeedIds.CourseB, "Củng cố kỹ năng và chuẩn bị sát hạch", "Luyện tổ hợp bài thi hoàn chỉnh, ổn định tâm lý, ghi nhớ các mốc trừ điểm và hoàn thiện kỹ năng xử lý tình huống thực tế.", 3),
                "Id",
                SeedIds.LearningRoadmapF)
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

        private static Blog CreatePublishedBlog(
            Guid id,
            DateTime createdAt,
            string title,
            int categoryId,
            string content,
            string summary,
            string avatar)
        {
            var blog = new Blog(
                title: title,
                categoryId: categoryId,
                content: content,
                createdBy: SeedIds.UserA,
                summary: summary,
                avatar: avatar);

            SetBaseFields(blog, id, createdAt, SeedIds.UserA);
            SetProperty(blog, "Status", true);
            return blog;
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
