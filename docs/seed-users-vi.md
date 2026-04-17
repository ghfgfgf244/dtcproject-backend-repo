# Script SQL thêm 300 học viên và 10 giáo viên

Script dưới đây dùng cho SQL Server, thêm:

- `300` user mới có `RoleId = 6` (`Student`)
- `10` user mới có `RoleId = 3` (`Instructor`)
- gán `UserCenters` cho giáo viên dựa trên các trung tâm hiện có trong bảng `Centers`

Script được viết theo hướng:

- có thể chạy lại mà không bị nhân bản dữ liệu do kiểm tra theo `ClerkId`
- `FullName` là tiếng Việt có dấu
- `Email`, `Phone`, `ClerkId` là duy nhất

```sql
SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRAN;

DECLARE @Now DATETIME2(7) = SYSUTCDATETIME();
DECLARE @CreatedBy UNIQUEIDENTIFIER = (
    SELECT TOP (1) Id
    FROM dbo.Users
    WHERE IsDeleted = 0
    ORDER BY CreatedAt, Id
);

DECLARE @Centers TABLE
(
    RowNo INT PRIMARY KEY,
    CenterId UNIQUEIDENTIFIER NOT NULL
);

INSERT INTO @Centers (RowNo, CenterId)
SELECT
    ROW_NUMBER() OVER (ORDER BY Id) AS RowNo,
    Id
FROM dbo.Centers
WHERE IsDeleted = 0;

DECLARE @CenterCount INT = (SELECT COUNT(*) FROM @Centers);

DECLARE @Ho TABLE
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Value NVARCHAR(50) NOT NULL
);

INSERT INTO @Ho (Value)
VALUES
(N'Nguyễn'),
(N'Trần'),
(N'Lê'),
(N'Phạm'),
(N'Hoàng'),
(N'Huỳnh'),
(N'Phan'),
(N'Vũ'),
(N'Võ'),
(N'Đặng'),
(N'Bùi'),
(N'Đỗ'),
(N'Hồ'),
(N'Ngô'),
(N'Dương');

DECLARE @DemNam TABLE
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Value NVARCHAR(50) NOT NULL
);

INSERT INTO @DemNam (Value)
VALUES
(N'Văn'),
(N'Hữu'),
(N'Đức'),
(N'Minh'),
(N'Gia'),
(N'Thanh'),
(N'Quang'),
(N'Tấn'),
(N'Khánh'),
(N'Thành'),
(N'Anh'),
(N'Chí');

DECLARE @DemNu TABLE
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Value NVARCHAR(50) NOT NULL
);

INSERT INTO @DemNu (Value)
VALUES
(N'Thị'),
(N'Ngọc'),
(N'Thu'),
(N'Thanh'),
(N'Minh'),
(N'Phương'),
(N'Tường'),
(N'Bảo'),
(N'Kim'),
(N'Ánh'),
(N'Hoài'),
(N'Diễm');

DECLARE @TenNam TABLE
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Value NVARCHAR(50) NOT NULL
);

INSERT INTO @TenNam (Value)
VALUES
(N'An'),
(N'Bình'),
(N'Cường'),
(N'Duy'),
(N'Đạt'),
(N'Hải'),
(N'Hiếu'),
(N'Hưng'),
(N'Khang'),
(N'Khôi'),
(N'Long'),
(N'Mạnh'),
(N'Nam'),
(N'Nhân'),
(N'Phong'),
(N'Phúc'),
(N'Quân'),
(N'Sơn'),
(N'Thắng'),
(N'Thiện'),
(N'Thịnh'),
(N'Trí'),
(N'Trung'),
(N'Tuấn'),
(N'Việt'),
(N'Vinh');

DECLARE @TenNu TABLE
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Value NVARCHAR(50) NOT NULL
);

INSERT INTO @TenNu (Value)
VALUES
(N'Anh'),
(N'Chi'),
(N'Duyên'),
(N'Giang'),
(N'Hà'),
(N'Hạnh'),
(N'Huyền'),
(N'Lan'),
(N'Linh'),
(N'Mai'),
(N'My'),
(N'Nga'),
(N'Ngân'),
(N'Nhung'),
(N'Oanh'),
(N'Phương'),
(N'Quỳnh'),
(N'Thảo'),
(N'Trang'),
(N'Trinh'),
(N'Trúc'),
(N'Tuyền'),
(N'Uyên'),
(N'Vân'),
(N'Vi'),
(N'Yến');

DECLARE @SoDemNam INT = (SELECT COUNT(*) FROM @DemNam);
DECLARE @SoDemNu INT = (SELECT COUNT(*) FROM @DemNu);
DECLARE @SoTenNam INT = (SELECT COUNT(*) FROM @TenNam);
DECLARE @SoTenNu INT = (SELECT COUNT(*) FROM @TenNu);
DECLARE @SoHo INT = (SELECT COUNT(*) FROM @Ho);

DECLARE @StudentSource TABLE
(
    Seq INT PRIMARY KEY,
    Id UNIQUEIDENTIFIER NOT NULL,
    ClerkId NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    AvatarUrl NVARCHAR(500) NULL,
    IsActive BIT NOT NULL,
    LastLoginAt DATETIME2(7) NULL,
    RoleId INT NOT NULL,
    CreatedAt DATETIME2(7) NOT NULL,
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2(7) NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL,
    IsDeleted BIT NOT NULL
);

;WITH Numbers AS
(
    SELECT TOP (300)
        ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS Seq
    FROM sys.all_objects
)
INSERT INTO @StudentSource
(
    Seq, Id, ClerkId, Email, FullName, Phone, AvatarUrl, IsActive,
    LastLoginAt, RoleId, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
)
SELECT
    n.Seq,
    NEWID(),
    CONCAT(N'seed_student_', RIGHT(CONCAT(N'0000', n.Seq), 4)),
    CONCAT(N'hocvien', RIGHT(CONCAT(N'0000', n.Seq), 4), N'@drivesafe.local'),
    CONCAT(
        h.Value, N' ',
        CASE WHEN n.Seq % 2 = 0 THEN dnu.Value ELSE dnam.Value END, N' ',
        CASE WHEN n.Seq % 2 = 0 THEN tnu.Value ELSE tnam.Value END
    ),
    CONCAT(N'+849', RIGHT(CONCAT(N'000000000', 100000000 + n.Seq), 9)),
    NULL,
    1,
    DATEADD(DAY, -((n.Seq - 1) % 45), @Now),
    6,
    DATEADD(MINUTE, n.Seq, @Now),
    @CreatedBy,
    DATEADD(MINUTE, n.Seq, @Now),
    @CreatedBy,
    0
FROM Numbers n
JOIN @Ho h
    ON h.Id = ((n.Seq - 1) % @SoHo) + 1
JOIN @DemNam dnam
    ON dnam.Id = ((n.Seq - 1) % @SoDemNam) + 1
JOIN @DemNu dnu
    ON dnu.Id = ((n.Seq - 1) % @SoDemNu) + 1
JOIN @TenNam tnam
    ON tnam.Id = ((n.Seq - 1) % @SoTenNam) + 1
JOIN @TenNu tnu
    ON tnu.Id = ((n.Seq - 1) % @SoTenNu) + 1;

INSERT INTO dbo.Users
(
    Id, ClerkId, Email, FullName, Phone, AvatarUrl, IsActive, LastLoginAt,
    RoleId, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
)
SELECT
    s.Id, s.ClerkId, s.Email, s.FullName, s.Phone, s.AvatarUrl, s.IsActive, s.LastLoginAt,
    s.RoleId, s.CreatedAt, s.CreatedBy, s.UpdatedAt, s.UpdatedBy, s.IsDeleted
FROM @StudentSource s
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Users u
    WHERE u.ClerkId = s.ClerkId
);

DECLARE @InstructorSource TABLE
(
    Seq INT PRIMARY KEY,
    Id UNIQUEIDENTIFIER NOT NULL,
    ClerkId NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    AvatarUrl NVARCHAR(500) NULL,
    IsActive BIT NOT NULL,
    LastLoginAt DATETIME2(7) NULL,
    RoleId INT NOT NULL,
    CreatedAt DATETIME2(7) NOT NULL,
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2(7) NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL,
    IsDeleted BIT NOT NULL
);

;WITH Numbers AS
(
    SELECT TOP (10)
        ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS Seq
    FROM sys.all_objects
)
INSERT INTO @InstructorSource
(
    Seq, Id, ClerkId, Email, FullName, Phone, AvatarUrl, IsActive,
    LastLoginAt, RoleId, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
)
SELECT
    n.Seq,
    NEWID(),
    CONCAT(N'seed_instructor_', RIGHT(CONCAT(N'000', n.Seq), 3)),
    CONCAT(N'giaovien', RIGHT(CONCAT(N'000', n.Seq), 3), N'@drivesafe.local'),
    CONCAT(h.Value, N' ', dnam.Value, N' ', tnam.Value),
    CONCAT(N'+8488', RIGHT(CONCAT(N'0000000', 1000000 + n.Seq), 7)),
    NULL,
    1,
    DATEADD(DAY, -n.Seq, @Now),
    3,
    DATEADD(HOUR, n.Seq, @Now),
    @CreatedBy,
    DATEADD(HOUR, n.Seq, @Now),
    @CreatedBy,
    0
FROM Numbers n
JOIN @Ho h
    ON h.Id = ((n.Seq + 2) % @SoHo) + 1
JOIN @DemNam dnam
    ON dnam.Id = ((n.Seq + 3) % @SoDemNam) + 1
JOIN @TenNam tnam
    ON tnam.Id = ((n.Seq + 5) % @SoTenNam) + 1;

INSERT INTO dbo.Users
(
    Id, ClerkId, Email, FullName, Phone, AvatarUrl, IsActive, LastLoginAt,
    RoleId, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
)
SELECT
    s.Id, s.ClerkId, s.Email, s.FullName, s.Phone, s.AvatarUrl, s.IsActive, s.LastLoginAt,
    s.RoleId, s.CreatedAt, s.CreatedBy, s.UpdatedAt, s.UpdatedBy, s.IsDeleted
FROM @InstructorSource s
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Users u
    WHERE u.ClerkId = s.ClerkId
);

IF @CenterCount > 0
BEGIN
    INSERT INTO dbo.UserCenters (UserId, CenterId)
    SELECT
        i.Id,
        c.CenterId
    FROM @InstructorSource i
    JOIN @Centers c
        ON c.RowNo = ((i.Seq - 1) % @CenterCount) + 1
    WHERE EXISTS
    (
        SELECT 1
        FROM dbo.Users u
        WHERE u.Id = i.Id
    )
    AND NOT EXISTS
    (
        SELECT 1
        FROM dbo.UserCenters uc
        WHERE uc.UserId = i.Id
          AND uc.CenterId = c.CenterId
    );
END;

SELECT
    COUNT(*) AS SoHocVienDaTao
FROM dbo.Users
WHERE ClerkId LIKE N'seed_student_%';

SELECT
    COUNT(*) AS SoGiaoVienDaTao
FROM dbo.Users
WHERE ClerkId LIKE N'seed_instructor_%';

COMMIT TRAN;
```

## Ghi chú

- `RoleId = 6`: `Student`
- `RoleId = 3`: `Instructor`
- Nếu bạn muốn, mình có thể viết tiếp thêm một script thứ hai để:
  - phân bổ các học viên vào `Terms`
  - gán vào `Classes`
  - sinh `StudentDrivingDistances`

---

# Script SQL thêm khóa học A1, A, B1, B, C1, C cho các trung tâm và đăng ký học viên vào `CourseRegistrations`

Script dưới đây sẽ:

- thêm các khóa học `A1`, `A`, `B1`, `B`, `C1`, `C` cho tất cả trung tâm đang có trong bảng `Centers`
- dùng `300` học viên seed ở script trên để tạo đăng ký học vào `CourseRegistrations`
- mỗi học viên được gán `1` khóa học theo kiểu vòng tròn
- tự động thêm `UserCenters` cho học viên theo trung tâm của khóa học đã đăng ký
- script có thể chạy lại mà không bị tạo trùng dữ liệu

```sql
SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRAN;

DECLARE @Now DATETIME2(7) = SYSUTCDATETIME();
DECLARE @CreatedBy UNIQUEIDENTIFIER = (
    SELECT TOP (1) Id
    FROM dbo.Users
    WHERE IsDeleted = 0
    ORDER BY CreatedAt, Id
);

DECLARE @CourseTemplates TABLE
(
    Seq INT PRIMARY KEY,
    LicenseType NVARCHAR(20) NOT NULL,
    CourseSuffix NVARCHAR(100) NOT NULL,
    DurationInWeeks INT NOT NULL,
    MaxStudents INT NOT NULL,
    Price DECIMAL(18, 2) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL
);

INSERT INTO @CourseTemplates
(
    Seq, LicenseType, CourseSuffix, DurationInWeeks, MaxStudents, Price, Description
)
VALUES
(1, N'A1', N'Hạng A1 cơ bản', 8, 35, 4800000, N'Khóa học lái xe hạng A1 dành cho học viên mới bắt đầu, tập trung vào luật giao thông và kỹ năng điều khiển xe hai bánh.'),
(2, N'A',  N'Hạng A nâng cao', 8, 35, 5200000, N'Khóa học lái xe hạng A giúp học viên nâng cao kỹ năng vận hành xe mô tô phân khối lớn và xử lý tình huống thực tế.'),
(3, N'B1', N'Hạng B1 số tự động', 12, 30, 10800000, N'Khóa học hạng B1 chuyên cho xe số tự động, chú trọng kỹ năng lái an toàn trong đô thị và đường trường.'),
(4, N'B',  N'Hạng B số sàn', 14, 28, 12800000, N'Khóa học hạng B dành cho học viên thi xe số sàn, kết hợp lý thuyết, sa hình và thực hành đường trường đầy đủ.'),
(5, N'C1', N'Hạng C1 tải nhẹ', 16, 24, 15600000, N'Khóa học hạng C1 đào tạo lái xe tải nhẹ với nội dung thực hành chuyên sâu, phù hợp học viên định hướng lái xe dịch vụ.'),
(6, N'C',  N'Hạng C tải trung', 18, 22, 17200000, N'Khóa học hạng C đào tạo lái xe tải trung, tăng cường các buổi thực hành đường dài và kỹ năng xử lý tải trọng.');

DECLARE @Centers TABLE
(
    RowNo INT PRIMARY KEY,
    CenterId UNIQUEIDENTIFIER NOT NULL,
    CenterName NVARCHAR(255) NOT NULL
);

INSERT INTO @Centers (RowNo, CenterId, CenterName)
SELECT
    ROW_NUMBER() OVER (ORDER BY Id) AS RowNo,
    Id,
    CenterName
FROM dbo.Centers
WHERE IsDeleted = 0;

DECLARE @CenterCount INT = (SELECT COUNT(*) FROM @Centers);

IF @CenterCount = 0
BEGIN
    RAISERROR(N'Không tìm thấy trung tâm nào trong bảng Centers.', 16, 1);
    ROLLBACK TRAN;
    RETURN;
END;

DECLARE @CourseSeed TABLE
(
    CenterId UNIQUEIDENTIFIER NOT NULL,
    LicenseType NVARCHAR(20) NOT NULL,
    CourseName NVARCHAR(255) NOT NULL,
    DurationInWeeks INT NOT NULL,
    MaxStudents INT NOT NULL,
    Price DECIMAL(18, 2) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL
);

INSERT INTO @CourseSeed
(
    CenterId, LicenseType, CourseName, DurationInWeeks, MaxStudents, Price, Description
)
SELECT
    c.CenterId,
    t.LicenseType,
    CONCAT(N'Khóa ', t.CourseSuffix, N' - ', c.CenterName),
    t.DurationInWeeks,
    t.MaxStudents,
    t.Price,
    CONCAT(t.Description, N' Trung tâm đào tạo: ', c.CenterName, N'.')
FROM @Centers c
CROSS JOIN @CourseTemplates t;

INSERT INTO dbo.Courses
(
    Id, CenterId, CourseName, LicenseType, DurationInWeeks, MaxStudents,
    ThumbnailUrl, Description, Price, IsActive,
    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
)
SELECT
    NEWID(),
    s.CenterId,
    s.CourseName,
    s.LicenseType,
    s.DurationInWeeks,
    s.MaxStudents,
    NULL,
    s.Description,
    s.Price,
    1,
    @Now,
    @CreatedBy,
    @Now,
    @CreatedBy,
    0
FROM @CourseSeed s
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Courses c
    WHERE c.CenterId = s.CenterId
      AND c.LicenseType = s.LicenseType
      AND c.CourseName = s.CourseName
      AND c.IsDeleted = 0
);

DECLARE @TargetCourses TABLE
(
    RowNo INT PRIMARY KEY,
    CourseId UNIQUEIDENTIFIER NOT NULL,
    Price DECIMAL(18, 2) NOT NULL
);

INSERT INTO @TargetCourses (RowNo, CourseId, Price)
SELECT
    ROW_NUMBER() OVER (ORDER BY c.CenterId, c.LicenseType, c.CourseName),
    c.Id,
    c.Price
FROM dbo.Courses c
WHERE c.IsDeleted = 0
  AND c.LicenseType IN (N'A1', N'A', N'B1', N'B', N'C1', N'C');

DECLARE @TargetCourseCount INT = (SELECT COUNT(*) FROM @TargetCourses);

IF @TargetCourseCount = 0
BEGIN
    RAISERROR(N'Không tìm thấy khóa học mục tiêu sau khi seed.', 16, 1);
    ROLLBACK TRAN;
    RETURN;
END;

DECLARE @SeedStudents TABLE
(
    RowNo INT PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    ClerkId NVARCHAR(255) NOT NULL
);

INSERT INTO @SeedStudents (RowNo, UserId, ClerkId)
SELECT
    ROW_NUMBER() OVER (ORDER BY u.ClerkId),
    u.Id,
    u.ClerkId
FROM dbo.Users u
WHERE u.IsDeleted = 0
  AND u.RoleId = 6
  AND u.ClerkId LIKE N'seed_student_%';

INSERT INTO dbo.CourseRegistrations
(
    Id, CourseId, UserId, AssignedTermId, RegistrationDate, Status, TotalFee, Notes,
    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
)
SELECT
    NEWID(),
    tc.CourseId,
    ss.UserId,
    NULL,
    DATEADD(DAY, -((ss.RowNo - 1) % 30), @Now),
    N'Pending',
    tc.Price,
    N'Đăng ký tự động từ script seed dữ liệu học viên.',
    @Now,
    @CreatedBy,
    @Now,
    @CreatedBy,
    0
FROM @SeedStudents ss
JOIN @TargetCourses tc
    ON tc.RowNo = ((ss.RowNo - 1) % @TargetCourseCount) + 1
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.CourseRegistrations cr
    WHERE cr.UserId = ss.UserId
      AND cr.CourseId = tc.CourseId
      AND cr.IsDeleted = 0
);

INSERT INTO dbo.UserCenters (UserId, CenterId)
SELECT DISTINCT
    cr.UserId,
    c.CenterId
FROM dbo.CourseRegistrations cr
JOIN dbo.Courses c
    ON c.Id = cr.CourseId
JOIN dbo.Users u
    ON u.Id = cr.UserId
WHERE cr.IsDeleted = 0
  AND c.IsDeleted = 0
  AND u.IsDeleted = 0
  AND u.ClerkId LIKE N'seed_student_%'
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.UserCenters uc
      WHERE uc.UserId = cr.UserId
        AND uc.CenterId = c.CenterId
  );

SELECT
    COUNT(*) AS SoKhoaHocMucTieu
FROM dbo.Courses
WHERE IsDeleted = 0
  AND LicenseType IN (N'A1', N'A', N'B1', N'B', N'C1', N'C');

SELECT
    COUNT(*) AS SoDangKyHocVienSeed
FROM dbo.CourseRegistrations cr
JOIN dbo.Users u ON u.Id = cr.UserId
WHERE cr.IsDeleted = 0
  AND u.ClerkId LIKE N'seed_student_%';

COMMIT TRAN;
```

## Gợi ý chạy

Chạy theo thứ tự:

1. Script thêm `300` học viên và `10` giáo viên
2. Script thêm khóa học `A1, A, B1, B, C1, C` và đăng ký học viên

Nếu bạn muốn, mình có thể làm tiếp file thứ ba để tạo luôn:

- `Terms` tương ứng cho các khóa mới
- `Classes`
- `ClassStudents`
- `ExamBatches`
- `ExamRegistrations`
