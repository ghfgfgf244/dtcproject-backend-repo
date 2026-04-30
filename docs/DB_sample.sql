SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRAN;

DECLARE @Now DATETIME2(7) = SYSUTCDATETIME();
DECLARE @Today DATE = CAST(@Now AS DATE);

DECLARE @CreatedBy UNIQUEIDENTIFIER =
(
    SELECT TOP (1) Id
    FROM dbo.Users
    WHERE IsDeleted = 0
    ORDER BY CreatedAt, Id
);

IF @CreatedBy IS NULL
BEGIN
    RAISERROR(N'Không tìm thấy user gốc để gán CreatedBy.', 16, 1);
    ROLLBACK TRAN;
    RETURN;
END;

/* =========================================================
   PHASE 1 - Chuẩn hóa 6 trung tâm Đà Nẵng
   ========================================================= */

DECLARE @CenterSeed TABLE
(
    SeedNo INT PRIMARY KEY,
    CenterName NVARCHAR(255) NOT NULL,
    Address NVARCHAR(500) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    NumberOfClasses INT NOT NULL,
    MaxStudentPerClass INT NOT NULL,
    TheoryAddressId INT NOT NULL,
    PracticeAddressId INT NOT NULL,
    ExamAddressId INT NOT NULL
);

INSERT INTO @CenterSeed
(
    SeedNo, CenterName, Address, Phone, Email, NumberOfClasses, MaxStudentPerClass,
    TheoryAddressId, PracticeAddressId, ExamAddressId
)
VALUES
(1, N'Trung tâm đào tạo lái xe ô tô, mô tô Đà Nẵng – STC', N'53 Phan Đăng Lưu, Phường Hải Châu, Thành phố Đà Nẵng', N'02363615209', N'stc@drivesafe.local', 24, 30, 1, 1, 1),
(2, N'Trung tâm giáo dục nghề nghiệp đào tạo lái xe 579', N'98 Núi Thành, Phường Hải Châu, Thành phố Đà Nẵng', N'02363246579', N'579@drivesafe.local', 24, 30, 2, 2, 2),
(3, N'Trung tâm đào tạo lái xe ô tô, mô tô Masco', N'113 Núi Thành, Phường Hải Châu, Thành phố Đà Nẵng', N'02363634488', N'masco@drivesafe.local', 24, 30, 3, 3, 3),
(4, N'Trung tâm đào tạo lái xe ô tô, mô tô Miền Trung', N'224 Lê Trọng Tấn, Phường Cẩm Lệ, Thành phố Đà Nẵng', N'02362487668', N'mientrung@drivesafe.local', 24, 30, 4, 4, 4),
(5, N'Trung tâm đào tạo lái xe ô tô, mô tô Sao Vàng', N'Lô 19 Khu D10 Nguyễn Sinh Sắc, Phường Liên Chiểu, Thành phố Đà Nẵng', N'02363738988', N'saovang@drivesafe.local', 24, 30, 5, 5, 5),
(6, N'Trung tâm đào tạo lái xe ô tô, mô tô Liên Chiểu', N'75 Nguyễn Lương Bằng, Phường Liên Chiểu, Thành phố Đà Nẵng', N'02363738988', N'lienchieu@drivesafe.local', 24, 30, 6, 6, 6);

MERGE dbo.Centers AS target
USING @CenterSeed AS source
ON target.CenterName = source.CenterName
WHEN MATCHED THEN
    UPDATE SET
        target.Address = source.Address,
        target.Phone = source.Phone,
        target.Email = source.Email,
        target.NumberOfClasses = source.NumberOfClasses,
        target.MaxStudentPerClass = source.MaxStudentPerClass,
        target.IsActive = 1,
        target.UpdatedAt = @Now,
        target.UpdatedBy = @CreatedBy,
        target.IsDeleted = 0
WHEN NOT MATCHED THEN
    INSERT
    (
        Id, CenterName, Address, Phone, Email, IsActive, NumberOfClasses, MaxStudentPerClass,
        CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
    )
    VALUES
    (
        NEWID(), source.CenterName, source.Address, source.Phone, source.Email, 1,
        source.NumberOfClasses, source.MaxStudentPerClass,
        @Now, @CreatedBy, @Now, @CreatedBy, 0
    );

DECLARE @Centers TABLE
(
    SeedNo INT PRIMARY KEY,
    CenterId UNIQUEIDENTIFIER NOT NULL,
    CenterName NVARCHAR(255) NOT NULL,
    TheoryAddressId INT NOT NULL,
    PracticeAddressId INT NOT NULL,
    ExamAddressId INT NOT NULL,
    MaxStudentPerClass INT NOT NULL
);

INSERT INTO @Centers
(
    SeedNo, CenterId, CenterName, TheoryAddressId, PracticeAddressId, ExamAddressId, MaxStudentPerClass
)
SELECT
    s.SeedNo,
    c.Id,
    c.CenterName,
    s.TheoryAddressId,
    s.PracticeAddressId,
    s.ExamAddressId,
    s.MaxStudentPerClass
FROM @CenterSeed s
JOIN dbo.Centers c
    ON c.CenterName = s.CenterName
WHERE c.IsDeleted = 0;

/* =========================================================
   PHASE 2 - Chuẩn hóa lại dữ liệu đào tạo cho 6 hạng bằng trọng tâm
   A1, A, B1, B, C1, C
   ========================================================= */

DECLARE @CourseTemplates TABLE
(
    RowNo INT PRIMARY KEY,
    LicenseType NVARCHAR(20) NOT NULL,
    DurationInWeeks INT NOT NULL,
    MaxStudents INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    DisplayName NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    HasSimulation BIT NOT NULL
);

INSERT INTO @CourseTemplates
(RowNo, LicenseType, DurationInWeeks, MaxStudents, Price, DisplayName, Description, HasSimulation)
VALUES
(1, N'A1',  2, 40,   850000, N'Khóa học GPLX hạng A1', N'Đào tạo xe máy dưới 175cc, tập trung phần lý thuyết và thực hành cơ bản để thi nhanh trong thời gian ngắn.', 0),
(2, N'A',   3, 32,  1900000, N'Khóa học GPLX hạng A',  N'Đào tạo mô tô phân khối lớn trên 175cc, phù hợp người học nâng hạng từ xe máy phổ thông lên mô tô công suất lớn.', 0),
(3, N'B1', 16, 28, 21000000, N'Khóa học GPLX hạng B1', N'Đào tạo ô tô số tự động không kinh doanh vận tải, gồm lý thuyết, sa hình và thực hành đường trường.', 1),
(4, N'B',  16, 28, 21500000, N'Khóa học GPLX hạng B',  N'Đào tạo ô tô số sàn, có thể phục vụ nhu cầu hành nghề lái xe và kinh doanh vận tải theo quy định hiện hành.', 1),
(5, N'C1', 22, 24, 25000000, N'Khóa học GPLX hạng C1', N'Đào tạo xe tải và xe chuyên dùng từ trên 3,5 tấn đến 7,5 tấn, chú trọng kỹ năng điều khiển an toàn và xử lý tình huống.', 1),
(6, N'C',  24, 22, 25500000, N'Khóa học GPLX hạng C',  N'Đào tạo xe tải nặng và xe chuyên dùng trên 7,5 tấn với lộ trình học dài hơn, phù hợp học viên định hướng lái xe chuyên nghiệp.', 1);

DECLARE @SeedCourseIds TABLE
(
    CourseId UNIQUEIDENTIFIER PRIMARY KEY
);

INSERT INTO @SeedCourseIds (CourseId)
SELECT c.Id
FROM dbo.Courses c
JOIN @Centers center_scope
    ON center_scope.CenterId = c.CenterId
WHERE c.IsDeleted = 0;

/* Dọn dữ liệu seed cũ để đảm bảo học phí, thời lượng, kỳ học, lớp và bài thi được tạo lại đồng bộ */
DELETE er
FROM dbo.ExamResults er
JOIN dbo.Exams e
    ON e.Id = er.ExamId
WHERE e.CourseId IN (SELECT CourseId FROM @SeedCourseIds);

DELETE a
FROM dbo.Attendance a
JOIN dbo.ClassSchedules cs
    ON cs.Id = a.ClassScheduleId
JOIN dbo.Classes c
    ON c.Id = cs.ClassId
JOIN dbo.Terms t
    ON t.Id = c.TermId
WHERE t.CourseId IN (SELECT CourseId FROM @SeedCourseIds);

DELETE cs_map
FROM dbo.ClassStudents cs_map
JOIN dbo.Classes c
    ON c.Id = cs_map.ClassId
JOIN dbo.Terms t
    ON t.Id = c.TermId
WHERE t.CourseId IN (SELECT CourseId FROM @SeedCourseIds);

DELETE cs
FROM dbo.ClassSchedules cs
JOIN dbo.Classes c
    ON c.Id = cs.ClassId
JOIN dbo.Terms t
    ON t.Id = c.TermId
WHERE t.CourseId IN (SELECT CourseId FROM @SeedCourseIds);

DELETE c
FROM dbo.Classes c
JOIN dbo.Terms t
    ON t.Id = c.TermId
WHERE t.CourseId IN (SELECT CourseId FROM @SeedCourseIds);

DELETE erg
FROM dbo.ExamRegistrations erg
JOIN dbo.Users u
    ON u.Id = erg.StudentId
WHERE u.ClerkId LIKE N'seed_student_%';

DELETE e
FROM dbo.Exams e
WHERE e.CourseId IN (SELECT CourseId FROM @SeedCourseIds);

DELETE t
FROM dbo.Terms t
WHERE t.CourseId IN (SELECT CourseId FROM @SeedCourseIds);

DELETE rr
FROM dbo.ReferralRegistrations rr
JOIN dbo.Users u
    ON u.Id = rr.StudentId
WHERE u.ClerkId LIKE N'seed_student_%';

DELETE cc
FROM dbo.CollaboratorCommissions cc
JOIN dbo.Users u
    ON u.Id = cc.CollaboratorId
WHERE u.ClerkId LIKE N'seed_collaborator_%';

UPDATE rc
SET rc.UsedCount = 0,
    rc.UpdatedAt = @Now,
    rc.UpdatedBy = @CreatedBy
FROM dbo.ReferralCodes rc
JOIN dbo.Users u
    ON u.Id = rc.CollaboratorId
WHERE u.ClerkId LIKE N'seed_collaborator_%';

DELETE uc
FROM dbo.UserCenters uc
JOIN dbo.Users u
    ON u.Id = uc.UserId
WHERE u.ClerkId LIKE N'seed_student_%'
  AND uc.CenterId IN (SELECT CenterId FROM @Centers);

DELETE cr
FROM dbo.CourseRegistrations cr
JOIN dbo.Users u
    ON u.Id = cr.UserId
WHERE u.ClerkId LIKE N'seed_student_%'
  AND cr.CourseId IN (SELECT CourseId FROM @SeedCourseIds);

UPDATE c
SET c.IsDeleted = 1,
    c.IsActive = 0,
    c.UpdatedAt = @Now,
    c.UpdatedBy = @CreatedBy
FROM dbo.Courses c
JOIN @Centers center_scope
    ON center_scope.CenterId = c.CenterId
WHERE c.IsDeleted = 0
  AND c.LicenseType NOT IN (SELECT LicenseType FROM @CourseTemplates);

MERGE dbo.Courses AS target
USING
(
    SELECT
        c.CenterId,
        CONCAT(t.DisplayName, N' - ', c.CenterName) AS CourseName,
        t.LicenseType,
        t.DurationInWeeks,
        t.MaxStudents,
        NULL AS ThumbnailUrl,
        CONCAT(t.Description, N' Trung tâm đào tạo: ', c.CenterName, N'.') AS Description,
        t.Price
    FROM @Centers c
    CROSS JOIN @CourseTemplates t
) AS source
ON target.CenterId = source.CenterId
AND target.LicenseType = source.LicenseType
AND target.CourseName = source.CourseName
WHEN MATCHED THEN
    UPDATE SET
        target.DurationInWeeks = source.DurationInWeeks,
        target.MaxStudents = source.MaxStudents,
        target.ThumbnailUrl = source.ThumbnailUrl,
        target.Description = source.Description,
        target.Price = source.Price,
        target.IsActive = 1,
        target.IsDeleted = 0,
        target.UpdatedAt = @Now,
        target.UpdatedBy = @CreatedBy
WHEN NOT MATCHED THEN
    INSERT
    (
        Id, CenterId, CourseName, LicenseType, DurationInWeeks, MaxStudents, ThumbnailUrl,
        Description, Price, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
    )
    VALUES
    (
        NEWID(), source.CenterId, source.CourseName, source.LicenseType, source.DurationInWeeks, source.MaxStudents, source.ThumbnailUrl,
        source.Description, source.Price, 1, @Now, @CreatedBy, @Now, @CreatedBy, 0
    );

DECLARE @Courses TABLE
(
    RowNo INT PRIMARY KEY,
    CourseId UNIQUEIDENTIFIER NOT NULL,
    CenterId UNIQUEIDENTIFIER NOT NULL,
    CenterName NVARCHAR(255) NOT NULL,
    LicenseType NVARCHAR(20) NOT NULL,
    CourseName NVARCHAR(255) NOT NULL,
    DurationInWeeks INT NOT NULL,
    MaxStudents INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    TheoryAddressId INT NOT NULL,
    PracticeAddressId INT NOT NULL,
    ExamAddressId INT NOT NULL,
    HasSimulation BIT NOT NULL
);

INSERT INTO @Courses
(
    RowNo, CourseId, CenterId, CenterName, LicenseType, CourseName, DurationInWeeks,
    MaxStudents, Price, TheoryAddressId, PracticeAddressId, ExamAddressId, HasSimulation
)
SELECT
    ROW_NUMBER() OVER (ORDER BY c.CenterName, crs.LicenseType, crs.CourseName),
    crs.Id,
    c.CenterId,
    c.CenterName,
    crs.LicenseType,
    crs.CourseName,
    crs.DurationInWeeks,
    crs.MaxStudents,
    crs.Price,
    c.TheoryAddressId,
    c.PracticeAddressId,
    c.ExamAddressId,
    tpl.HasSimulation
FROM dbo.Courses crs
JOIN @Centers c
    ON c.CenterId = crs.CenterId
JOIN @CourseTemplates tpl
    ON tpl.LicenseType = crs.LicenseType
WHERE crs.IsDeleted = 0;

/* =========================================================
   PHASE 3 - Seed 10 giảng viên
   ========================================================= */

DECLARE @InstructorSeed TABLE
(
    Seq INT PRIMARY KEY,
    ClerkId NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    CenterSeedNo INT NOT NULL
);

INSERT INTO @InstructorSeed
(Seq, ClerkId, Email, FullName, Phone, CenterSeedNo)
VALUES
(1,  N'seed_instructor_01', N'giangvien01@drivesafe.local', N'Nguyễn Văn Hòa',    N'0905100001', 1),
(2,  N'seed_instructor_02', N'giangvien02@drivesafe.local', N'Trần Quốc Bảo',     N'0905100002', 1),
(3,  N'seed_instructor_03', N'giangvien03@drivesafe.local', N'Lê Minh Tâm',       N'0905100003', 2),
(4,  N'seed_instructor_04', N'giangvien04@drivesafe.local', N'Phạm Gia Hưng',     N'0905100004', 2),
(5,  N'seed_instructor_05', N'giangvien05@drivesafe.local', N'Võ Thanh Long',     N'0905100005', 3),
(6,  N'seed_instructor_06', N'giangvien06@drivesafe.local', N'Huỳnh Quang Khải',  N'0905100006', 3),
(7,  N'seed_instructor_07', N'giangvien07@drivesafe.local', N'Đặng Thị Phương',   N'0905100007', 4),
(8,  N'seed_instructor_08', N'giangvien08@drivesafe.local', N'Bùi Thị Hạnh',      N'0905100008', 4),
(9,  N'seed_instructor_09', N'giangvien09@drivesafe.local', N'Ngô Đức Toàn',      N'0905100009', 5),
(10, N'seed_instructor_10', N'giangvien10@drivesafe.local', N'Dương Khánh Linh',  N'0905100010', 6);

MERGE dbo.Users AS target
USING
(
    SELECT ClerkId, Email, FullName, Phone
    FROM @InstructorSeed
) AS source
ON target.ClerkId = source.ClerkId
WHEN MATCHED THEN
    UPDATE SET
        target.Email = source.Email,
        target.FullName = source.FullName,
        target.Phone = source.Phone,
        target.RoleId = 3,
        target.IsActive = 1,
        target.UpdatedAt = @Now,
        target.UpdatedBy = @CreatedBy,
        target.IsDeleted = 0
WHEN NOT MATCHED THEN
    INSERT
    (
        Id, ClerkId, Email, FullName, Phone, AvatarUrl, IsActive, LastLoginAt, RoleId,
        CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
    )
    VALUES
    (
        NEWID(), source.ClerkId, source.Email, source.FullName, source.Phone, NULL, 1, NULL, 3,
        @Now, @CreatedBy, @Now, @CreatedBy, 0
    );

/* =========================================================
   PHASE 4 - Seed 20 cộng tác viên, có liên kết trung tâm
   ========================================================= */

DECLARE @CollaboratorSeed TABLE
(
    Seq INT PRIMARY KEY,
    ClerkId NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    CenterSeedNo INT NOT NULL,
    ReferralCode NVARCHAR(50) NOT NULL
);

INSERT INTO @CollaboratorSeed
(Seq, ClerkId, Email, FullName, Phone, CenterSeedNo, ReferralCode)
VALUES
(1,  N'seed_collaborator_01', N'ctv01@drivesafe.local', N'Nguyễn Thị Thu Hà',  N'0916200001', 1, N'REF-STC-01'),
(2,  N'seed_collaborator_02', N'ctv02@drivesafe.local', N'Trần Văn Duy',       N'0916200002', 1, N'REF-STC-02'),
(3,  N'seed_collaborator_03', N'ctv03@drivesafe.local', N'Lê Hoàng Nam',       N'0916200003', 1, N'REF-STC-03'),
(4,  N'seed_collaborator_04', N'ctv04@drivesafe.local', N'Phạm Ngọc Mai',      N'0916200004', 2, N'REF-579-01'),
(5,  N'seed_collaborator_05', N'ctv05@drivesafe.local', N'Vũ Minh Châu',       N'0916200005', 2, N'REF-579-02'),
(6,  N'seed_collaborator_06', N'ctv06@drivesafe.local', N'Đỗ Hữu Trí',         N'0916200006', 2, N'REF-579-03'),
(7,  N'seed_collaborator_07', N'ctv07@drivesafe.local', N'Hồ Thanh Trúc',      N'0916200007', 3, N'REF-MAS-01'),
(8,  N'seed_collaborator_08', N'ctv08@drivesafe.local', N'Ngô Gia Bảo',        N'0916200008', 3, N'REF-MAS-02'),
(9,  N'seed_collaborator_09', N'ctv09@drivesafe.local', N'Dương Kim Oanh',     N'0916200009', 3, N'REF-MAS-03'),
(10, N'seed_collaborator_10', N'ctv10@drivesafe.local', N'Bùi Quốc Cường',     N'0916200010', 4, N'REF-MTR-01'),
(11, N'seed_collaborator_11', N'ctv11@drivesafe.local', N'Phan Quỳnh Như',     N'0916200011', 4, N'REF-MTR-02'),
(12, N'seed_collaborator_12', N'ctv12@drivesafe.local', N'Hoàng Khánh Vy',     N'0916200012', 4, N'REF-MTR-03'),
(13, N'seed_collaborator_13', N'ctv13@drivesafe.local', N'Đặng Quốc Thịnh',    N'0916200013', 5, N'REF-SV-01'),
(14, N'seed_collaborator_14', N'ctv14@drivesafe.local', N'Võ Hải Yến',         N'0916200014', 5, N'REF-SV-02'),
(15, N'seed_collaborator_15', N'ctv15@drivesafe.local', N'Huỳnh Tuấn Phát',    N'0916200015', 5, N'REF-SV-03'),
(16, N'seed_collaborator_16', N'ctv16@drivesafe.local', N'Nguyễn Minh Khoa',   N'0916200016', 6, N'REF-LC-01'),
(17, N'seed_collaborator_17', N'ctv17@drivesafe.local', N'Trần Bảo Ngọc',      N'0916200017', 6, N'REF-LC-02'),
(18, N'seed_collaborator_18', N'ctv18@drivesafe.local', N'Lê Hoài Thương',     N'0916200018', 6, N'REF-LC-03'),
(19, N'seed_collaborator_19', N'ctv19@drivesafe.local', N'Phạm Đức Anh',       N'0916200019', 1, N'REF-STC-04'),
(20, N'seed_collaborator_20', N'ctv20@drivesafe.local', N'Vũ Thanh Ngân',      N'0916200020', 2, N'REF-579-04');

MERGE dbo.Users AS target
USING
(
    SELECT ClerkId, Email, FullName, Phone
    FROM @CollaboratorSeed
) AS source
ON target.ClerkId = source.ClerkId
WHEN MATCHED THEN
    UPDATE SET
        target.Email = source.Email,
        target.FullName = source.FullName,
        target.Phone = source.Phone,
        target.RoleId = 5,
        target.IsActive = 1,
        target.UpdatedAt = @Now,
        target.UpdatedBy = @CreatedBy,
        target.IsDeleted = 0
WHEN NOT MATCHED THEN
    INSERT
    (
        Id, ClerkId, Email, FullName, Phone, AvatarUrl, IsActive, LastLoginAt, RoleId,
        CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
    )
    VALUES
    (
        NEWID(), source.ClerkId, source.Email, source.FullName, source.Phone, NULL, 1, NULL, 5,
        @Now, @CreatedBy, @Now, @CreatedBy, 0
    );

/* =========================================================
   PHASE 5 - Gán UserCenters cho instructor và collaborator
   ========================================================= */

INSERT INTO dbo.UserCenters (UserId, CenterId)
SELECT DISTINCT
    u.Id,
    c.CenterId
FROM @InstructorSeed s
JOIN dbo.Users u
    ON u.ClerkId = s.ClerkId
JOIN @Centers c
    ON c.SeedNo = s.CenterSeedNo
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.UserCenters uc
    WHERE uc.UserId = u.Id
      AND uc.CenterId = c.CenterId
);

INSERT INTO dbo.UserCenters (UserId, CenterId)
SELECT DISTINCT
    u.Id,
    c.CenterId
FROM @CollaboratorSeed s
JOIN dbo.Users u
    ON u.ClerkId = s.ClerkId
JOIN @Centers c
    ON c.SeedNo = s.CenterSeedNo
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.UserCenters uc
    WHERE uc.UserId = u.Id
      AND uc.CenterId = c.CenterId
);

/* =========================================================
   PHASE 6 - ReferralCodes
   ========================================================= */

INSERT INTO dbo.ReferralCodes
(
    Id, Code, CollaboratorId, UsedCount, IsActive,
    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
)
SELECT
    NEWID(),
    s.ReferralCode,
    u.Id,
    0,
    1,
    @Now,
    @CreatedBy,
    @Now,
    @CreatedBy,
    0
FROM @CollaboratorSeed s
JOIN dbo.Users u
    ON u.ClerkId = s.ClerkId
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.ReferralCodes rc
    WHERE rc.Code = s.ReferralCode
      AND rc.IsDeleted = 0
);

/* =========================================================
   PHASE 7 - Tạo 600 student có tên tiếng Việt đúng dạng
   ========================================================= */

DECLARE @LastNames TABLE (Id INT IDENTITY(1,1) PRIMARY KEY, Val NVARCHAR(50));
DECLARE @MiddleNames TABLE (Id INT IDENTITY(1,1) PRIMARY KEY, Val NVARCHAR(50));
DECLARE @FirstNames TABLE (Id INT IDENTITY(1,1) PRIMARY KEY, Val NVARCHAR(50));

INSERT INTO @LastNames (Val)
VALUES
(N'Nguyễn'), (N'Trần'), (N'Lê'), (N'Phạm'), (N'Hoàng'), (N'Võ'), (N'Huỳnh'), (N'Phan'), (N'Bùi'), (N'Đặng'),
(N'Đỗ'), (N'Ngô'), (N'Dương'), (N'Lý'), (N'Trương');

INSERT INTO @MiddleNames (Val)
VALUES
(N'Văn'), (N'Thị'), (N'Ngọc'), (N'Minh'), (N'Gia'), (N'Đức'), (N'Bảo'), (N'Quốc'), (N'Thanh'), (N'Anh'),
(N'Hoàng'), (N'Tuấn'), (N'Khắc'), (N'Xuân'), (N'Thu'), (N'Quỳnh');

INSERT INTO @FirstNames (Val)
VALUES
(N'An'), (N'Bình'), (N'Châu'), (N'Đạt'), (N'Đức'), (N'Duy'), (N'Giang'), (N'Hà'), (N'Hạnh'), (N'Hiếu'),
(N'Hoàng'), (N'Hùng'), (N'Khánh'), (N'Khoa'), (N'Lâm'), (N'Lan'), (N'Long'), (N'Mai'), (N'Minh'), (N'Nam'),
(N'Ngân'), (N'Ngọc'), (N'Nhi'), (N'Oanh'), (N'Phát'), (N'Phúc'), (N'Phương'), (N'Quang'), (N'Quỳnh'), (N'Sơn'),
(N'Tâm'), (N'Thảo'), (N'Thiện'), (N'Thịnh'), (N'Thủy'), (N'Toàn'), (N'Trang'), (N'Trí'), (N'Trúc'), (N'Tuấn'),
(N'Tuyết'), (N'Vy'), (N'Yến');

;WITH Numbers AS
(
    SELECT TOP (600) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS Seq
    FROM sys.all_objects a
    CROSS JOIN sys.all_objects b
)
INSERT INTO dbo.Users
(
    Id, ClerkId, Email, FullName, Phone, AvatarUrl, IsActive, LastLoginAt, RoleId,
    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
)
SELECT
    NEWID(),
    CONCAT(N'seed_student_', RIGHT(CONCAT(N'0000', n.Seq), 4)),
    CONCAT(N'student', RIGHT(CONCAT(N'0000', n.Seq), 4), N'@drivesafe.local'),
    CONCAT(ln.Val, N' ', mn.Val, N' ', fn.Val),
    CONCAT(N'093', RIGHT(CONCAT(N'0000000', n.Seq), 7)),
    NULL,
    1,
    NULL,
    6,
    @Now,
    @CreatedBy,
    @Now,
    @CreatedBy,
    0
FROM Numbers n
JOIN @LastNames ln
    ON ln.Id = ((n.Seq - 1) % (SELECT COUNT(*) FROM @LastNames)) + 1
JOIN @MiddleNames mn
    ON mn.Id = ((n.Seq - 1) % (SELECT COUNT(*) FROM @MiddleNames)) + 1
JOIN @FirstNames fn
    ON fn.Id = ((n.Seq - 1) % (SELECT COUNT(*) FROM @FirstNames)) + 1
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Users u
    WHERE u.ClerkId = CONCAT(N'seed_student_', RIGHT(CONCAT(N'0000', n.Seq), 4))
);

DECLARE @Students TABLE
(
    Seq INT PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    ClerkId NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(255) NOT NULL
);

INSERT INTO @Students
(Seq, UserId, ClerkId, FullName)
SELECT
    TRY_CONVERT(INT, SUBSTRING(u.ClerkId, LEN(N'seed_student_') + 1, 20)),
    u.Id,
    u.ClerkId,
    u.FullName
FROM dbo.Users u
WHERE u.ClerkId LIKE N'seed_student_%'
  AND u.IsDeleted = 0
  AND TRY_CONVERT(INT, SUBSTRING(u.ClerkId, LEN(N'seed_student_') + 1, 20)) BETWEEN 1 AND 600;

/* =========================================================
   PHASE 8 - CourseRegistrations
   Nhóm 1: pipeline
   Nhóm 2: approved để học, thi
   ========================================================= */

DECLARE @CourseCount INT = (SELECT COUNT(*) FROM @Courses);

DECLARE @StudentRegistrationSeed TABLE
(
    Seq INT PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    CourseId UNIQUEIDENTIFIER NOT NULL,
    RegistrationDate DATETIME2(7) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    OriginalFee DECIMAL(18,2) NOT NULL,
    TotalFee DECIMAL(18,2) NOT NULL,
    Notes NVARCHAR(500) NULL
);

INSERT INTO @StudentRegistrationSeed
(Seq, UserId, CourseId, RegistrationDate, Status, OriginalFee, TotalFee, Notes)
SELECT
    st.Seq,
    st.UserId,
    c.CourseId,
    CASE
        WHEN st.Seq BETWEEN 1 AND 300 THEN
            DATEADD(DAY, -1 * (((st.Seq - 1) % 120) + 1), @Now)
        ELSE
            DATEADD(DAY, -1 * (45 + ((st.Seq - 301) % 120)), @Now)
    END,
    CASE
        WHEN st.Seq BETWEEN 1 AND 300 THEN
            CASE st.Seq % 10
                WHEN 0 THEN N'Cancelled'
                WHEN 1 THEN N'Pending'
                WHEN 2 THEN N'Pending'
                WHEN 3 THEN N'Pending'
                WHEN 4 THEN N'Pending'
                WHEN 5 THEN N'Approved'
                WHEN 6 THEN N'Approved'
                WHEN 7 THEN N'Approved'
                WHEN 8 THEN N'Rejected'
                ELSE N'Rejected'
            END
        ELSE N'Approved'
    END,
    c.Price,
    CASE
        WHEN st.Seq BETWEEN 301 AND 360 THEN CAST(ROUND(c.Price * 0.95, 0) AS DECIMAL(18,2))
        ELSE c.Price
    END,
    CASE
        WHEN st.Seq BETWEEN 1 AND 300 THEN
            CASE st.Seq % 10
                WHEN 0 THEN N'Hồ sơ đã được học viên chủ động hủy sau khi tư vấn.'
                WHEN 8 THEN N'Hồ sơ bị từ chối do thiếu giấy khám sức khỏe hoặc hồ sơ tùy thân.'
                WHEN 9 THEN N'Hồ sơ bị từ chối do thông tin cá nhân chưa khớp.'
                WHEN 5 THEN N'Hồ sơ đã được duyệt và chờ xếp kỳ học phù hợp.'
                WHEN 6 THEN N'Hồ sơ đã được duyệt và đang chờ học viên bổ sung phí đầu vào.'
                WHEN 7 THEN N'Hồ sơ đã được duyệt sau khi xác minh đủ điều kiện.'
                ELSE N'Hồ sơ đang nằm trong pipeline tuyển sinh và chờ xử lý.'
            END
        WHEN st.Seq BETWEEN 301 AND 360 THEN N'Hồ sơ đã áp dụng mã giới thiệu cộng tác viên, đủ điều kiện xếp lớp và đăng ký thi.'
        ELSE N'Hồ sơ đã hoàn tất nhập học, được xếp lớp và sẵn sàng đăng ký thi.'
    END
FROM @Students st
JOIN @Courses c
    ON c.RowNo = ((st.Seq - 1) % @CourseCount) + 1;

INSERT INTO dbo.CourseRegistrations
(
    Id, CourseId, UserId, AssignedTermId, RegistrationDate, Status, OriginalFee, TotalFee, Notes,
    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
)
SELECT
    NEWID(),
    s.CourseId,
    s.UserId,
    NULL,
    s.RegistrationDate,
    s.Status,
    s.OriginalFee,
    s.TotalFee,
    s.Notes,
    @Now,
    @CreatedBy,
    @Now,
    @CreatedBy,
    0
FROM @StudentRegistrationSeed s
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.CourseRegistrations cr
    WHERE cr.UserId = s.UserId
      AND cr.CourseId = s.CourseId
      AND cr.IsDeleted = 0
);

/* =========================================================
   PHASE 9 - Đồng bộ UserCenters cho student theo CourseRegistration
   ========================================================= */

INSERT INTO dbo.UserCenters (UserId, CenterId)
SELECT DISTINCT
    cr.UserId,
    c.CenterId
FROM dbo.CourseRegistrations cr
JOIN dbo.Courses c
    ON c.Id = cr.CourseId
WHERE cr.IsDeleted = 0
  AND c.IsDeleted = 0
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.UserCenters uc
      WHERE uc.UserId = cr.UserId
        AND uc.CenterId = c.CenterId
  );

/* =========================================================
   PHASE 10 - ReferralRegistrations và CollaboratorCommissions
   Dùng 60 học viên thuộc nhóm 2
   ========================================================= */

DECLARE @ReferralStudents TABLE
(
    Seq INT PRIMARY KEY,
    StudentId UNIQUEIDENTIFIER NOT NULL,
    ReferralCodeId UNIQUEIDENTIFIER NOT NULL,
    CollaboratorId UNIQUEIDENTIFIER NOT NULL,
    CourseRegistrationId UNIQUEIDENTIFIER NOT NULL,
    RegisteredAt DATETIME2(7) NOT NULL
);

INSERT INTO @ReferralStudents
(Seq, StudentId, ReferralCodeId, CollaboratorId, CourseRegistrationId, RegisteredAt)
SELECT
    ROW_NUMBER() OVER (ORDER BY st.Seq),
    st.UserId,
    rc.Id,
    rc.CollaboratorId,
    cr.Id,
    DATEADD(DAY, 1, cr.RegistrationDate)
FROM @Students st
JOIN dbo.ReferralCodes rc
    ON ((st.Seq - 301) % 20) + 1 =
       (
           SELECT c.Seq
           FROM @CollaboratorSeed c
           WHERE c.ReferralCode = rc.Code
       )
JOIN dbo.CourseRegistrations cr
    ON cr.UserId = st.UserId
   AND cr.IsDeleted = 0
   AND cr.Status = N'Approved'
WHERE st.Seq BETWEEN 301 AND 360;

INSERT INTO dbo.ReferralRegistrations (Id, ReferralCodeId, StudentId, CourseRegistrationId, RegisteredAt)
SELECT
    NEWID(),
    rs.ReferralCodeId,
    rs.StudentId,
    rs.CourseRegistrationId,
    rs.RegisteredAt
FROM @ReferralStudents rs
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.ReferralRegistrations rr
    WHERE rr.ReferralCodeId = rs.ReferralCodeId
      AND rr.StudentId = rs.StudentId
      AND rr.CourseRegistrationId = rs.CourseRegistrationId
);

UPDATE rc
SET rc.UsedCount = usage_stats.UsedCount,
    rc.UpdatedAt = @Now,
    rc.UpdatedBy = @CreatedBy
FROM dbo.ReferralCodes rc
JOIN
(
    SELECT ReferralCodeId, COUNT(*) AS UsedCount
    FROM dbo.ReferralRegistrations
    GROUP BY ReferralCodeId
) usage_stats
    ON usage_stats.ReferralCodeId = rc.Id;

INSERT INTO dbo.CollaboratorCommissions
(Id, CollaboratorId, ReferralRegistrationId, Amount, Status, CreatedAt, PaidAt)
SELECT
    NEWID(),
    rs.CollaboratorId,
    rr.Id,
    CAST(ROUND(cr.OriginalFee * 0.05, 0) AS DECIMAL(18,2)),
    CASE WHEN ABS(CHECKSUM(rs.StudentId)) % 5 = 0 THEN N'Paid' ELSE N'Pending' END,
    DATEADD(DAY, 3, rs.RegisteredAt),
    CASE WHEN ABS(CHECKSUM(rs.StudentId)) % 5 = 0 THEN DATEADD(DAY, 14, rs.RegisteredAt) ELSE NULL END
FROM @ReferralStudents rs
JOIN dbo.CourseRegistrations cr
    ON cr.Id = rs.CourseRegistrationId
JOIN dbo.ReferralRegistrations rr
    ON rr.ReferralCodeId = rs.ReferralCodeId
   AND rr.StudentId = rs.StudentId
   AND rr.CourseRegistrationId = rs.CourseRegistrationId
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.CollaboratorCommissions cc
    WHERE cc.ReferralRegistrationId = rr.Id
);

/* =========================================================
   PHASE 11 - Tạo 3 term cho mỗi course
   ========================================================= */

DECLARE @TermSeed TABLE
(
    CourseId UNIQUEIDENTIFIER NOT NULL,
    TermNo INT NOT NULL,
    TermName NVARCHAR(255) NOT NULL,
    StartDate DATETIME2(7) NOT NULL,
    EndDate DATETIME2(7) NOT NULL,
    MaxStudents INT NOT NULL
);

INSERT INTO @TermSeed
(CourseId, TermNo, TermName, StartDate, EndDate, MaxStudents)
SELECT
    c.CourseId,
    t.TermNo,
    CONCAT(N'Đợt ', RIGHT(CONCAT(N'0', t.TermNo), 2), N'-', YEAR(DATEADD(MONTH, (t.TermNo - 1) * 4, CAST('2025-10-06' AS DATETIME2(7)))), N' ', c.LicenseType, N' - ', c.CenterName),
    DATEADD(MONTH, (t.TermNo - 1) * 4, CAST('2025-10-06' AS DATETIME2(7))),
    DATEADD(WEEK, c.DurationInWeeks, DATEADD(MONTH, (t.TermNo - 1) * 4, CAST('2025-10-06' AS DATETIME2(7)))),
    c.MaxStudents
FROM @Courses c
CROSS JOIN (VALUES (1), (2), (3)) t(TermNo);

INSERT INTO dbo.Terms
(
    Id, CourseId, TermName, StartDate, EndDate, CurrentStudents, MaxStudents, IsActive,
    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
)
SELECT
    NEWID(),
    s.CourseId,
    s.TermName,
    s.StartDate,
    s.EndDate,
    0,
    s.MaxStudents,
    1,
    @Now,
    @CreatedBy,
    @Now,
    @CreatedBy,
    0
FROM @TermSeed s
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Terms t
    WHERE t.CourseId = s.CourseId
      AND t.TermName = s.TermName
      AND t.IsDeleted = 0
);

DECLARE @Terms TABLE
(
    RowNo INT PRIMARY KEY,
    TermId UNIQUEIDENTIFIER NOT NULL,
    CourseId UNIQUEIDENTIFIER NOT NULL,
    CourseName NVARCHAR(255) NOT NULL,
    LicenseType NVARCHAR(20) NOT NULL,
    CenterId UNIQUEIDENTIFIER NOT NULL,
    CenterName NVARCHAR(255) NOT NULL,
    StartDate DATETIME2(7) NOT NULL,
    EndDate DATETIME2(7) NOT NULL,
    MaxStudents INT NOT NULL,
    MaxStudentPerClass INT NOT NULL,
    TheoryAddressId INT NOT NULL,
    PracticeAddressId INT NOT NULL
);

INSERT INTO @Terms
(
    RowNo, TermId, CourseId, CourseName, LicenseType, CenterId, CenterName, StartDate, EndDate,
    MaxStudents, MaxStudentPerClass, TheoryAddressId, PracticeAddressId
)
SELECT
    ROW_NUMBER() OVER (ORDER BY tr.StartDate, c.CenterName, crs.LicenseType),
    tr.Id,
    tr.CourseId,
    crs.CourseName,
    crs.LicenseType,
    c.CenterId,
    c.CenterName,
    tr.StartDate,
    tr.EndDate,
    tr.MaxStudents,
    c.MaxStudentPerClass,
    c.TheoryAddressId,
    c.PracticeAddressId
FROM dbo.Terms tr
JOIN @Courses crs
    ON crs.CourseId = tr.CourseId
JOIN @Centers c
    ON c.CenterId = crs.CenterId
WHERE tr.IsDeleted = 0;

/* =========================================================
   PHASE 12 - Gán term cho nhóm 2 và cập nhật AssignedTermId
   ========================================================= */

DECLARE @ApprovedStudentTerms TABLE
(
    UserId UNIQUEIDENTIFIER NOT NULL,
    CourseId UNIQUEIDENTIFIER NOT NULL,
    TermId UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (UserId, CourseId)
);

INSERT INTO @ApprovedStudentTerms
(UserId, CourseId, TermId)
SELECT
    s.UserId,
    cr.CourseId,
    term_pool.TermId
FROM @Students s
JOIN dbo.CourseRegistrations cr
    ON cr.UserId = s.UserId
   AND cr.IsDeleted = 0
   AND cr.Status = N'Approved'
JOIN
(
    SELECT
        t.CourseId,
        t.TermId,
        ROW_NUMBER() OVER (PARTITION BY t.CourseId ORDER BY t.StartDate, t.TermId) AS TermNo
    FROM @Terms t
) term_pool
    ON term_pool.CourseId = cr.CourseId
   AND term_pool.TermNo = ((ABS(CHECKSUM(s.UserId)) % 2) + 1)
WHERE s.Seq BETWEEN 301 AND 600;

UPDATE cr
SET cr.AssignedTermId = ast.TermId,
    cr.UpdatedAt = @Now,
    cr.UpdatedBy = @CreatedBy
FROM dbo.CourseRegistrations cr
JOIN @ApprovedStudentTerms ast
    ON ast.UserId = cr.UserId
   AND ast.CourseId = cr.CourseId
WHERE cr.IsDeleted = 0
  AND cr.Status = N'Approved';

UPDATE t
SET t.CurrentStudents = stats.StudentCount,
    t.UpdatedAt = @Now,
    t.UpdatedBy = @CreatedBy
FROM dbo.Terms t
JOIN
(
    SELECT AssignedTermId AS TermId, COUNT(*) AS StudentCount
    FROM dbo.CourseRegistrations
    WHERE IsDeleted = 0
      AND Status = N'Approved'
      AND AssignedTermId IS NOT NULL
    GROUP BY AssignedTermId
) stats
    ON stats.TermId = t.Id;

/* =========================================================
   PHASE 13 - Tạo 3 lớp Theory + 3 lớp Practice mỗi term
   ========================================================= */

DECLARE @ClassSeed TABLE
(
    TermId UNIQUEIDENTIFIER NOT NULL,
    InstructorId UNIQUEIDENTIFIER NOT NULL,
    ClassName NVARCHAR(255) NOT NULL,
    ClassType NVARCHAR(50) NOT NULL,
    MaxStudents INT NOT NULL
);

INSERT INTO @ClassSeed
(TermId, InstructorId, ClassName, ClassType, MaxStudents)
SELECT
    t.TermId,
    ip.UserId,
    CONCAT(
        t.LicenseType,
        N'-',
        RIGHT(CONCAT(N'0', ROW_NUMBER() OVER (PARTITION BY t.TermId, x.ClassType ORDER BY x.Seq)), 2),
        CASE WHEN x.ClassType = N'Theory' THEN N'-LT' ELSE N'-TH' END
    ),
    x.ClassType,
    CASE
        WHEN x.ClassType = N'Theory' THEN t.MaxStudentPerClass
        ELSE t.MaxStudentPerClass + CASE WHEN t.MaxStudentPerClass >= 25 THEN 2 ELSE 1 END
    END
FROM @Terms t
CROSS JOIN
(
    SELECT 1 AS Seq, N'Theory' AS ClassType UNION ALL
    SELECT 2, N'Theory' UNION ALL
    SELECT 3, N'Theory' UNION ALL
    SELECT 4, N'Practice' UNION ALL
    SELECT 5, N'Practice' UNION ALL
    SELECT 6, N'Practice'
) x
JOIN
(
    SELECT
        uc.CenterId,
        u.Id AS UserId,
        ROW_NUMBER() OVER (PARTITION BY uc.CenterId ORDER BY u.FullName, u.Id) AS CenterInstructorNo
    FROM dbo.Users u
    JOIN dbo.UserCenters uc
        ON uc.UserId = u.Id
    WHERE u.RoleId = 3
      AND u.IsDeleted = 0
) ip
    ON ip.CenterId = t.CenterId
   AND ip.CenterInstructorNo = ((x.Seq - 1) % 2) + 1;

INSERT INTO dbo.Classes
(
    Id, TermId, InstructorId, ClassName, CurrentStudents, MaxStudents, ClassType, Status,
    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
)
SELECT
    NEWID(),
    s.TermId,
    s.InstructorId,
    s.ClassName,
    0,
    s.MaxStudents,
    s.ClassType,
    N'Pending',
    @Now,
    @CreatedBy,
    @Now,
    @CreatedBy,
    0
FROM @ClassSeed s
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Classes c
    WHERE c.TermId = s.TermId
      AND c.ClassName = s.ClassName
      AND c.IsDeleted = 0
);

DECLARE @Classes TABLE
(
    RowNo INT PRIMARY KEY,
    ClassId UNIQUEIDENTIFIER NOT NULL,
    TermId UNIQUEIDENTIFIER NOT NULL,
    InstructorId UNIQUEIDENTIFIER NOT NULL,
    ClassName NVARCHAR(255) NOT NULL,
    ClassType NVARCHAR(50) NOT NULL,
    CenterId UNIQUEIDENTIFIER NOT NULL,
    StartDate DATETIME2(7) NOT NULL,
    EndDate DATETIME2(7) NOT NULL,
    DurationInWeeks INT NOT NULL,
    TheoryAddressId INT NOT NULL,
    PracticeAddressId INT NOT NULL
);

INSERT INTO @Classes
(
    RowNo, ClassId, TermId, InstructorId, ClassName, ClassType, CenterId, StartDate, EndDate,
    DurationInWeeks, TheoryAddressId, PracticeAddressId
)
SELECT
    ROW_NUMBER() OVER (ORDER BY t.StartDate, c.ClassName),
    c.Id,
    c.TermId,
    c.InstructorId,
    c.ClassName,
    c.ClassType,
    t.CenterId,
    t.StartDate,
    t.EndDate,
    crs.DurationInWeeks,
    t.TheoryAddressId,
    t.PracticeAddressId
FROM dbo.Classes c
JOIN @Terms t
    ON t.TermId = c.TermId
JOIN @Courses crs
    ON crs.CourseId = t.CourseId
WHERE c.IsDeleted = 0;

/* =========================================================
   PHASE 14 - ClassStudents
   Mỗi học viên nhóm 2 được gán 1 lớp Theory + 1 lớp Practice
   ========================================================= */

DECLARE @TargetClassAssignments TABLE
(
    StudentId UNIQUEIDENTIFIER NOT NULL,
    ClassId UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (StudentId, ClassId)
);

INSERT INTO @TargetClassAssignments
(StudentId, ClassId)
SELECT
    ast.UserId,
    class_pool.ClassId
FROM @ApprovedStudentTerms ast
JOIN
(
    SELECT
        c.TermId,
        c.ClassType,
        c.ClassId,
        ROW_NUMBER() OVER (PARTITION BY c.TermId, c.ClassType ORDER BY c.ClassName) AS ClassNo
    FROM @Classes c
) class_pool
    ON class_pool.TermId = ast.TermId
   AND class_pool.ClassType = N'Theory'
   AND class_pool.ClassNo = ((ABS(CHECKSUM(ast.UserId)) % 3) + 1)

UNION ALL

SELECT
    ast.UserId,
    class_pool.ClassId
FROM @ApprovedStudentTerms ast
JOIN
(
    SELECT
        c.TermId,
        c.ClassType,
        c.ClassId,
        ROW_NUMBER() OVER (PARTITION BY c.TermId, c.ClassType ORDER BY c.ClassName) AS ClassNo
    FROM @Classes c
) class_pool
    ON class_pool.TermId = ast.TermId
   AND class_pool.ClassType = N'Practice'
   AND class_pool.ClassNo = ((ABS(CHECKSUM(ast.UserId, 99)) % 3) + 1);

INSERT INTO dbo.ClassStudents (ClassId, StudentId)
SELECT
    tca.ClassId,
    tca.StudentId
FROM @TargetClassAssignments tca
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.ClassStudents cs
    WHERE cs.ClassId = tca.ClassId
      AND cs.StudentId = tca.StudentId
);

UPDATE c
SET c.CurrentStudents = stats.StudentCount,
    c.Status = N'InProgress'
FROM dbo.Classes c
JOIN
(
    SELECT ClassId, COUNT(*) AS StudentCount
    FROM dbo.ClassStudents
    GROUP BY ClassId
) stats
    ON stats.ClassId = c.Id;

/* =========================================================
   PHASE 15 - ClassSchedules
   Mỗi lớp mỗi tuần 2 tiết
   ========================================================= */

;WITH WeekNumbers AS
(
    SELECT TOP (30) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS WeekNo
    FROM sys.all_objects
),
ScheduleSource AS
(
    SELECT
        c.ClassId,
        c.InstructorId,
        CASE
            WHEN c.ClassType = N'Theory' THEN c.TheoryAddressId
            ELSE c.PracticeAddressId
        END AS AddressId,
        CASE
            WHEN c.ClassType = N'Theory'
                THEN DATEADD(DAY, 2 + (w.WeekNo * 7), CAST(c.StartDate AS DATETIME2(7)))
            ELSE DATEADD(DAY, 6 + (w.WeekNo * 7), CAST(c.StartDate AS DATETIME2(7)))
        END AS SessionDate1,
        CASE
            WHEN c.ClassType = N'Theory'
                THEN DATEADD(DAY, 4 + (w.WeekNo * 7), CAST(c.StartDate AS DATETIME2(7)))
            ELSE DATEADD(DAY, 7 + (w.WeekNo * 7), CAST(c.StartDate AS DATETIME2(7)))
        END AS SessionDate2,
        c.ClassType,
        c.EndDate
    FROM @Classes c
    JOIN WeekNumbers w
        ON w.WeekNo < c.DurationInWeeks
)
INSERT INTO dbo.ClassSchedules
(
    Id, ClassId, InstructorId, StartTime, EndTime, AddressId,
    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
)
SELECT
    NEWID(),
    s.ClassId,
    s.InstructorId,
    CASE WHEN s.ClassType = N'Theory' THEN DATEADD(HOUR, 18, s.SessionDate1) ELSE DATEADD(HOUR, 7, s.SessionDate1) END,
    CASE WHEN s.ClassType = N'Theory' THEN DATEADD(HOUR, 20, s.SessionDate1) ELSE DATEADD(HOUR, 9, s.SessionDate1) END,
    s.AddressId,
    @Now,
    @CreatedBy,
    @Now,
    @CreatedBy,
    0
FROM ScheduleSource s
WHERE s.SessionDate1 <= s.EndDate
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.ClassSchedules cs
      WHERE cs.ClassId = s.ClassId
        AND cs.StartTime = CASE WHEN s.ClassType = N'Theory' THEN DATEADD(HOUR, 18, s.SessionDate1) ELSE DATEADD(HOUR, 7, s.SessionDate1) END
  );

;WITH WeekNumbers AS
(
    SELECT TOP (30) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS WeekNo
    FROM sys.all_objects
),
ScheduleSource AS
(
    SELECT
        c.ClassId,
        c.InstructorId,
        CASE
            WHEN c.ClassType = N'Theory' THEN c.TheoryAddressId
            ELSE c.PracticeAddressId
        END AS AddressId,
        CASE
            WHEN c.ClassType = N'Theory'
                THEN DATEADD(DAY, 4 + (w.WeekNo * 7), CAST(c.StartDate AS DATETIME2(7)))
            ELSE DATEADD(DAY, 7 + (w.WeekNo * 7), CAST(c.StartDate AS DATETIME2(7)))
        END AS SessionDate2,
        c.ClassType,
        c.EndDate
    FROM @Classes c
    JOIN WeekNumbers w
        ON w.WeekNo < c.DurationInWeeks
)
INSERT INTO dbo.ClassSchedules
(
    Id, ClassId, InstructorId, StartTime, EndTime, AddressId,
    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
)
SELECT
    NEWID(),
    s.ClassId,
    s.InstructorId,
    CASE WHEN s.ClassType = N'Theory' THEN DATEADD(HOUR, 18, s.SessionDate2) ELSE DATEADD(HOUR, 14, s.SessionDate2) END,
    CASE WHEN s.ClassType = N'Theory' THEN DATEADD(HOUR, 20, s.SessionDate2) ELSE DATEADD(HOUR, 16, s.SessionDate2) END,
    s.AddressId,
    @Now,
    @CreatedBy,
    @Now,
    @CreatedBy,
    0
FROM ScheduleSource s
WHERE s.SessionDate2 <= s.EndDate
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.ClassSchedules cs
      WHERE cs.ClassId = s.ClassId
        AND cs.StartTime = CASE WHEN s.ClassType = N'Theory' THEN DATEADD(HOUR, 18, s.SessionDate2) ELSE DATEADD(HOUR, 14, s.SessionDate2) END
  );

/* =========================================================
   PHASE 16 - Attendance đầy đủ, đảm bảo nhóm 2 đủ điều kiện thi
   ========================================================= */

INSERT INTO dbo.Attendance
(
    Id, ClassScheduleId, StudentId, IsPresent, CheckedAt,
    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
)
SELECT
    NEWID(),
    src.ClassScheduleId,
    src.StudentId,
    CASE WHEN ABS(CHECKSUM(src.StudentId, src.ClassScheduleId)) % 20 IN (0, 1) THEN 0 ELSE 1 END,
    DATEADD(MINUTE, 15, src.StartTime),
    @Now,
    @CreatedBy,
    @Now,
    @CreatedBy,
    0
FROM
(
    SELECT
        sched.Id AS ClassScheduleId,
        sched.StartTime,
        cs.StudentId
    FROM dbo.ClassSchedules sched
    JOIN dbo.ClassStudents cs
        ON cs.ClassId = sched.ClassId
    JOIN @Students st
        ON st.UserId = cs.StudentId
    WHERE st.Seq BETWEEN 301 AND 600
) src
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Attendance a
    WHERE a.ClassScheduleId = src.ClassScheduleId
      AND a.StudentId = src.StudentId
);

/* =========================================================
   PHASE 17 - Tạo 10 ExamBatches
   ========================================================= */

DECLARE @ExamBatchSeed TABLE
(
    BatchNo INT PRIMARY KEY,
    ScopeType NVARCHAR(50) NOT NULL,
    CenterId UNIQUEIDENTIFIER NULL,
    BatchName NVARCHAR(255) NOT NULL,
    RegistrationStartDate DATETIME2(7) NOT NULL,
    RegistrationEndDate DATETIME2(7) NOT NULL,
    ExamStartDate DATETIME2(7) NOT NULL,
    MaxCandidates INT NOT NULL,
    Status NVARCHAR(50) NOT NULL
);

INSERT INTO @ExamBatchSeed
(BatchNo, ScopeType, CenterId, BatchName, RegistrationStartDate, RegistrationEndDate, ExamStartDate, MaxCandidates, Status)
VALUES
(1,  N'National', NULL, N'Kỳ thi sát hạch quốc gia tháng 01/2026', '2025-12-01', '2025-12-20', '2026-01-10', 240, N'Completed'),
(2,  N'National', NULL, N'Kỳ thi sát hạch quốc gia tháng 02/2026', '2026-01-01', '2026-01-20', '2026-02-10', 240, N'Completed'),
(3,  N'National', NULL, N'Kỳ thi sát hạch quốc gia tháng 03/2026', '2026-02-01', '2026-02-20', '2026-03-10', 240, N'Completed'),
(4,  N'National', NULL, N'Kỳ thi sát hạch quốc gia tháng 04/2026', '2026-03-01', '2026-03-20', '2026-04-10', 240, N'Completed'),
(5,  N'National', NULL, N'Kỳ thi sát hạch quốc gia tháng 05/2026', '2026-04-01', '2026-04-20', '2026-05-10', 240, N'Completed'),
(6,  N'National', NULL, N'Kỳ thi sát hạch quốc gia tháng 06/2026', '2026-05-01', '2026-05-20', '2026-06-10', 240, N'ClosedForRegistration'),
(7,  N'National', NULL, N'Kỳ thi sát hạch quốc gia tháng 07/2026', '2026-06-01', '2026-06-20', '2026-07-10', 240, N'OpenForRegistration'),
(8,  N'National', NULL, N'Kỳ thi sát hạch quốc gia tháng 08/2026', '2026-07-01', '2026-07-20', '2026-08-10', 240, N'Pending'),
(9,  N'National', NULL, N'Kỳ thi sát hạch quốc gia tháng 09/2026', '2026-08-01', '2026-08-20', '2026-09-10', 240, N'Pending'),
(10, N'National', NULL, N'Kỳ thi sát hạch quốc gia tháng 10/2026', '2026-09-01', '2026-09-20', '2026-10-10', 240, N'Pending');

UPDATE eb
SET eb.IsDeleted = 1,
    eb.UpdatedAt = @Now,
    eb.UpdatedBy = @CreatedBy
FROM dbo.ExamBatches eb
WHERE eb.BatchName LIKE N'Đợt thi sát hạch tháng %/2026'
  AND eb.IsDeleted = 0;

MERGE dbo.ExamBatches AS target
USING @ExamBatchSeed AS source
ON target.BatchName = source.BatchName
WHEN MATCHED THEN
    UPDATE SET
        target.ScopeType = source.ScopeType,
        target.CenterId = source.CenterId,
        target.RegistrationStartDate = source.RegistrationStartDate,
        target.RegistrationEndDate = source.RegistrationEndDate,
        target.ExamStartDate = source.ExamStartDate,
        target.CurrentCandidates = 0,
        target.MaxCandidates = source.MaxCandidates,
        target.Status = source.Status,
        target.UpdatedAt = @Now,
        target.UpdatedBy = @CreatedBy,
        target.IsDeleted = 0
WHEN NOT MATCHED THEN
    INSERT
    (
        Id, ScopeType, CenterId, BatchName, RegistrationStartDate, RegistrationEndDate, ExamStartDate,
        CurrentCandidates, MaxCandidates, Status,
        CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
    )
    VALUES
    (
        NEWID(), source.ScopeType, source.CenterId, source.BatchName, source.RegistrationStartDate, source.RegistrationEndDate, source.ExamStartDate,
        0, source.MaxCandidates, source.Status,
        @Now, @CreatedBy, @Now, @CreatedBy, 0
    );

DECLARE @ExamBatches TABLE
(
    BatchNo INT PRIMARY KEY,
    ExamBatchId UNIQUEIDENTIFIER NOT NULL,
    BatchName NVARCHAR(255) NOT NULL,
    ExamStartDate DATETIME2(7) NOT NULL
);

INSERT INTO @ExamBatches
(BatchNo, ExamBatchId, BatchName, ExamStartDate)
SELECT
    s.BatchNo,
    eb.Id,
    eb.BatchName,
    eb.ExamStartDate
FROM @ExamBatchSeed s
JOIN dbo.ExamBatches eb
    ON eb.BatchName = s.BatchName
WHERE eb.IsDeleted = 0;

/* =========================================================
   PHASE 18 - Tạo Exams cho mỗi course trong một batch
   Mỗi course: Theory + Practice + Simulation nếu có
   ========================================================= */

DECLARE @ExamCourseBatchMap TABLE
(
    TermId UNIQUEIDENTIFIER NOT NULL,
    CourseId UNIQUEIDENTIFIER NOT NULL,
    ExamBatchId UNIQUEIDENTIFIER NOT NULL,
    ExamDate DATETIME2(7) NOT NULL,
    AddressId INT NOT NULL,
    LicenseType NVARCHAR(20) NOT NULL,
    CourseName NVARCHAR(255) NOT NULL,
    CenterName NVARCHAR(255) NOT NULL,
    HasSimulation BIT NOT NULL,
    PRIMARY KEY (TermId, CourseId, ExamBatchId)
);

INSERT INTO @ExamCourseBatchMap
(TermId, CourseId, ExamBatchId, ExamDate, AddressId, LicenseType, CourseName, CenterName, HasSimulation)
SELECT
    term_plan.TermId,
    term_plan.CourseId,
    eb_choice.ExamBatchId,
    DATEADD(DAY, (ABS(CHECKSUM(term_plan.TermId, term_plan.CourseId)) % 3), eb_choice.ExamStartDate),
    term_plan.ExamAddressId,
    term_plan.LicenseType,
    term_plan.CourseName,
    term_plan.CenterName,
    term_plan.HasSimulation
FROM
(
    SELECT DISTINCT
        ast.TermId,
        ast.CourseId,
        t.EndDate,
        c.ExamAddressId,
        c.LicenseType,
        c.CourseName,
        c.CenterName,
        c.HasSimulation
    FROM @ApprovedStudentTerms ast
    JOIN @Terms t
        ON t.TermId = ast.TermId
    JOIN @Courses c
        ON c.CourseId = ast.CourseId
) term_plan
CROSS APPLY
(
    SELECT TOP (1)
        eb.ExamBatchId,
        eb.ExamStartDate
    FROM @ExamBatches eb
    WHERE eb.ExamStartDate >= DATEADD(DAY, 7, term_plan.EndDate)
    ORDER BY eb.ExamStartDate, eb.ExamBatchId
) eb_choice;

INSERT INTO dbo.Exams
(
    Id, ExamBatchId, CourseId, AddressId, ExamName, ExamDate, ExamType, DurationMinutes,
    TotalScore, PassScore, Status,
    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
)
SELECT
    NEWID(),
    m.ExamBatchId,
    m.CourseId,
    m.AddressId,
    CONCAT(N'Thi lý thuyết GPLX hạng ', m.LicenseType, N' - ', m.CenterName),
    DATEADD(HOUR, 8, m.ExamDate),
    N'Theory',
    CASE WHEN m.LicenseType IN (N'A1', N'A') THEN 20 ELSE 30 END,
    CASE WHEN m.LicenseType IN (N'A1', N'A') THEN 25 ELSE 35 END,
    CASE WHEN m.LicenseType IN (N'A1', N'A') THEN 21 ELSE 32 END,
    CASE WHEN DATEADD(HOUR, 8, m.ExamDate) < @Now THEN N'Finished' ELSE N'Scheduled' END,
    @Now,
    @CreatedBy,
    @Now,
    @CreatedBy,
    0
FROM @ExamCourseBatchMap m
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Exams e
    WHERE e.ExamBatchId = m.ExamBatchId
      AND e.CourseId = m.CourseId
      AND e.ExamType = N'Theory'
      AND e.IsDeleted = 0
);

INSERT INTO dbo.Exams
(
    Id, ExamBatchId, CourseId, AddressId, ExamName, ExamDate, ExamType, DurationMinutes,
    TotalScore, PassScore, Status,
    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
)
SELECT
    NEWID(),
    m.ExamBatchId,
    m.CourseId,
    m.AddressId,
    CONCAT(N'Thi thực hành GPLX hạng ', m.LicenseType, N' - ', m.CenterName),
    DATEADD(HOUR, 13, m.ExamDate),
    N'Practice',
    CASE WHEN m.LicenseType IN (N'A1', N'A') THEN 15 ELSE 30 END,
    100,
    80,
    CASE WHEN DATEADD(HOUR, 13, m.ExamDate) < @Now THEN N'Finished' ELSE N'Scheduled' END,
    @Now,
    @CreatedBy,
    @Now,
    @CreatedBy,
    0
FROM @ExamCourseBatchMap m
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Exams e
    WHERE e.ExamBatchId = m.ExamBatchId
      AND e.CourseId = m.CourseId
      AND e.ExamType = N'Practice'
      AND e.IsDeleted = 0
);

INSERT INTO dbo.Exams
(
    Id, ExamBatchId, CourseId, AddressId, ExamName, ExamDate, ExamType, DurationMinutes,
    TotalScore, PassScore, Status,
    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
)
SELECT
    NEWID(),
    m.ExamBatchId,
    m.CourseId,
    m.AddressId,
    CONCAT(N'Thi mô phỏng GPLX hạng ', m.LicenseType, N' - ', m.CenterName),
    DATEADD(HOUR, 10, m.ExamDate),
    N'Simulation',
    20,
    50,
    35,
    CASE WHEN DATEADD(HOUR, 10, m.ExamDate) < @Now THEN N'Finished' ELSE N'Scheduled' END,
    @Now,
    @CreatedBy,
    @Now,
    @CreatedBy,
    0
FROM @ExamCourseBatchMap m
WHERE m.HasSimulation = 1
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.Exams e
      WHERE e.ExamBatchId = m.ExamBatchId
        AND e.CourseId = m.CourseId
        AND e.ExamType = N'Simulation'
        AND e.IsDeleted = 0
  );

/* =========================================================
   PHASE 19 - ExamRegistrations cho nhóm 2, mặc định đủ điều kiện thi
   ========================================================= */

INSERT INTO dbo.ExamRegistrations
(
    Id, ExamBatchId, StudentId, RegistrationDate, IsPaid, Status,
    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
)
SELECT
    NEWID(),
    map.ExamBatchId,
    ast.UserId,
    DATEADD(DAY, -14, map.ExamDate),
    1,
    N'Approved',
    @Now,
    @CreatedBy,
    @Now,
    @CreatedBy,
    0
FROM @ApprovedStudentTerms ast
JOIN @ExamCourseBatchMap map
    ON map.CourseId = ast.CourseId
   AND map.TermId = ast.TermId
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.ExamRegistrations er
    WHERE er.ExamBatchId = map.ExamBatchId
      AND er.StudentId = ast.UserId
      AND er.IsDeleted = 0
);

UPDATE eb
SET eb.CurrentCandidates = stats.CurrentCandidates,
    eb.UpdatedAt = @Now,
    eb.UpdatedBy = @CreatedBy
FROM dbo.ExamBatches eb
JOIN
(
    SELECT ExamBatchId, COUNT(*) AS CurrentCandidates
    FROM dbo.ExamRegistrations
    WHERE IsDeleted = 0
      AND Status = N'Approved'
    GROUP BY ExamBatchId
) stats
    ON stats.ExamBatchId = eb.Id;

/* =========================================================
   PHASE 20 - ExamResults cho toàn bộ nhóm 2
   ========================================================= */

INSERT INTO dbo.ExamResults
(Id, ExamId, StudentId, AttemptNo, Score, IsPassed)
SELECT
    NEWID(),
    e.Id,
    ast.UserId,
    1,
    score_seed.Score,
    CASE WHEN score_seed.Score >= e.PassScore THEN 1 ELSE 0 END
FROM @ApprovedStudentTerms ast
JOIN @ExamCourseBatchMap map
    ON map.CourseId = ast.CourseId
   AND map.TermId = ast.TermId
JOIN dbo.Exams e
    ON e.ExamBatchId = map.ExamBatchId
   AND e.CourseId = ast.CourseId
   AND e.IsDeleted = 0
CROSS APPLY
(
    SELECT ABS(CHECKSUM(ast.UserId, e.Id)) % 100 AS SeedValue
) seed
CROSS APPLY
(
    SELECT CAST(
        CASE
            WHEN e.ExamType = N'Theory' THEN
                CASE
                    WHEN seed.SeedValue < 12 THEN e.PassScore - 1 - (seed.SeedValue % 4)
                    WHEN seed.SeedValue < 45 THEN e.PassScore + (seed.SeedValue % (e.TotalScore - e.PassScore + 1))
                    ELSE e.TotalScore - (seed.SeedValue % 3)
                END
            WHEN e.ExamType = N'Practice' THEN
                CASE
                    WHEN seed.SeedValue < 15 THEN 70 + (seed.SeedValue % 10)
                    WHEN seed.SeedValue < 55 THEN 80 + (seed.SeedValue % 12)
                    ELSE 92 + (seed.SeedValue % 8)
                END
            ELSE
                CASE
                    WHEN seed.SeedValue < 18 THEN 25 + (seed.SeedValue % 10)
                    WHEN seed.SeedValue < 60 THEN 35 + (seed.SeedValue % 8)
                    ELSE 43 + (seed.SeedValue % 7)
                END
        END AS FLOAT
    ) AS Score
) score_seed
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.ExamResults er
    WHERE er.ExamId = e.Id
      AND er.StudentId = ast.UserId
)
  AND e.ExamDate < @Now;

/* =========================================================
   PHASE 21 - Query tổng kết nhanh
   ========================================================= */

SELECT COUNT(*) AS SoTrungTamSeed
FROM dbo.Centers
WHERE IsDeleted = 0
  AND CenterName IN
  (
      N'Trung tâm đào tạo lái xe ô tô, mô tô Đà Nẵng – STC',
      N'Trung tâm giáo dục nghề nghiệp đào tạo lái xe 579',
      N'Trung tâm đào tạo lái xe ô tô, mô tô Masco',
      N'Trung tâm đào tạo lái xe ô tô, mô tô Miền Trung',
      N'Trung tâm đào tạo lái xe ô tô, mô tô Sao Vàng',
      N'Trung tâm đào tạo lái xe ô tô, mô tô Liên Chiểu'
  );

SELECT LicenseType, COUNT(*) AS SoCourse
FROM dbo.Courses
WHERE IsDeleted = 0
GROUP BY LicenseType
ORDER BY LicenseType;

SELECT COUNT(*) AS SoGiangVienSeed
FROM dbo.Users
WHERE IsDeleted = 0
  AND ClerkId LIKE N'seed_instructor_%';

SELECT COUNT(*) AS SoCongTacVienSeed
FROM dbo.Users
WHERE IsDeleted = 0
  AND ClerkId LIKE N'seed_collaborator_%';

SELECT COUNT(*) AS SoHocVienSeed
FROM dbo.Users
WHERE IsDeleted = 0
  AND ClerkId LIKE N'seed_student_%';

SELECT Status, COUNT(*) AS SoCourseRegistrations
FROM dbo.CourseRegistrations cr
JOIN dbo.Users u
    ON u.Id = cr.UserId
WHERE cr.IsDeleted = 0
  AND u.ClerkId LIKE N'seed_student_%'
GROUP BY Status
ORDER BY Status;

SELECT COUNT(*) AS SoReferralRegistrations
FROM dbo.ReferralRegistrations;

SELECT Status, COUNT(*) AS SoCommission
FROM dbo.CollaboratorCommissions
GROUP BY Status;

SELECT COUNT(*) AS SoTermsSeed
FROM dbo.Terms
WHERE IsDeleted = 0;

SELECT ClassType, COUNT(*) AS SoClasses
FROM dbo.Classes
WHERE IsDeleted = 0
GROUP BY ClassType;

SELECT COUNT(*) AS SoClassSchedules
FROM dbo.ClassSchedules
WHERE IsDeleted = 0;

SELECT COUNT(*) AS SoAttendance
FROM dbo.Attendance
WHERE IsDeleted = 0;

SELECT COUNT(*) AS SoExamBatches
FROM dbo.ExamBatches
WHERE IsDeleted = 0;

SELECT ExamType, COUNT(*) AS SoExams
FROM dbo.Exams
WHERE IsDeleted = 0
GROUP BY ExamType;

SELECT Status, COUNT(*) AS SoExamRegistrations
FROM dbo.ExamRegistrations
WHERE IsDeleted = 0
GROUP BY Status;

SELECT COUNT(*) AS SoExamResults
FROM dbo.ExamResults;

COMMIT TRAN;
