using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dtc.Infrastructure.Persistence.SQLServer.Migrations
{
    [Migration("20260425020500_RepairRuntimeSchemaDrift")]
    public partial class RepairRuntimeSchemaDrift : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF COL_LENGTH('dbo.CourseRegistrations', 'OriginalFee') IS NULL
                BEGIN
                    ALTER TABLE dbo.CourseRegistrations
                    ADD OriginalFee decimal(18,2) NOT NULL
                        CONSTRAINT DF_CourseRegistrations_OriginalFee DEFAULT (0);
                END

                EXEC(N'UPDATE dbo.CourseRegistrations SET OriginalFee = TotalFee WHERE OriginalFee = 0');

                IF COL_LENGTH('dbo.ReferralRegistrations', 'CourseRegistrationId') IS NULL
                BEGIN
                    ALTER TABLE dbo.ReferralRegistrations
                    ADD CourseRegistrationId uniqueidentifier NULL;
                END

                IF NOT EXISTS
                (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = 'IX_ReferralRegistrations_CourseRegistrationId'
                      AND object_id = OBJECT_ID('dbo.ReferralRegistrations')
                )
                BEGIN
                    CREATE INDEX IX_ReferralRegistrations_CourseRegistrationId
                    ON dbo.ReferralRegistrations (CourseRegistrationId);
                END

                IF NOT EXISTS
                (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = 'FK_ReferralRegistrations_CourseRegistrations_CourseRegistrationId'
                )
                BEGIN
                    ALTER TABLE dbo.ReferralRegistrations WITH CHECK
                    ADD CONSTRAINT FK_ReferralRegistrations_CourseRegistrations_CourseRegistrationId
                    FOREIGN KEY (CourseRegistrationId)
                    REFERENCES dbo.CourseRegistrations (Id)
                    ON DELETE NO ACTION;
                END

                IF COL_LENGTH('dbo.CollaboratorCommissions', 'ReferralRegistrationId') IS NULL
                BEGIN
                    ALTER TABLE dbo.CollaboratorCommissions
                    ADD ReferralRegistrationId uniqueidentifier NULL;
                END

                IF NOT EXISTS
                (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = 'IX_CollaboratorCommissions_ReferralRegistrationId'
                      AND object_id = OBJECT_ID('dbo.CollaboratorCommissions')
                )
                BEGIN
                    CREATE INDEX IX_CollaboratorCommissions_ReferralRegistrationId
                    ON dbo.CollaboratorCommissions (ReferralRegistrationId);
                END

                IF NOT EXISTS
                (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = 'FK_CollaboratorCommissions_ReferralRegistrations_ReferralRegistrationId'
                )
                BEGIN
                    ALTER TABLE dbo.CollaboratorCommissions WITH CHECK
                    ADD CONSTRAINT FK_CollaboratorCommissions_ReferralRegistrations_ReferralRegistrationId
                    FOREIGN KEY (ReferralRegistrationId)
                    REFERENCES dbo.ReferralRegistrations (Id)
                    ON DELETE NO ACTION;
                END

                IF COL_LENGTH('dbo.ExamBatches', 'ScopeType') IS NULL
                BEGIN
                    ALTER TABLE dbo.ExamBatches
                    ADD ScopeType nvarchar(50) NOT NULL
                        CONSTRAINT DF_ExamBatches_ScopeType DEFAULT (N'National');
                END

                IF COL_LENGTH('dbo.ExamBatches', 'CenterId') IS NULL
                BEGIN
                    ALTER TABLE dbo.ExamBatches
                    ADD CenterId uniqueidentifier NULL;
                END

                IF NOT EXISTS
                (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = 'IX_ExamBatches_CenterId'
                      AND object_id = OBJECT_ID('dbo.ExamBatches')
                )
                BEGIN
                    CREATE INDEX IX_ExamBatches_CenterId
                    ON dbo.ExamBatches (CenterId);
                END

                IF NOT EXISTS
                (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = 'FK_ExamBatches_Centers_CenterId'
                )
                BEGIN
                    ALTER TABLE dbo.ExamBatches WITH CHECK
                    ADD CONSTRAINT FK_ExamBatches_Centers_CenterId
                    FOREIGN KEY (CenterId)
                    REFERENCES dbo.Centers (Id)
                    ON DELETE NO ACTION;
                END
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // This migration repairs schema drift in local/demo databases.
            // Down is intentionally left empty so rollback does not remove live data columns.
        }
    }
}
