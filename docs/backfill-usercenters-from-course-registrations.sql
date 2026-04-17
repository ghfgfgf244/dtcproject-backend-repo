SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRAN;

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
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.UserCenters uc
      WHERE uc.UserId = cr.UserId
        AND uc.CenterId = c.CenterId
  );

SELECT @@ROWCOUNT AS InsertedUserCenterRows;

COMMIT TRAN;
