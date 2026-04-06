using dtc.Domain.Entities;
using dtc.Domain.Entities.Classes;
using dtc.Domain.Entities.Collaborators;
using dtc.Domain.Entities.Exams;
using dtc.Domain.Entities.Permissions;
using dtc.Domain.Entities.Training;
using dtc.Domain.Entities.Terms;
using dtc.Domain.ValueObjects;
using dtc.Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using EmailVO = dtc.Domain.ValueObjects.Email;
using dtc.Domain.Entities.Location;

namespace dtc.Infrastructure.Pesistence.SQLServer
{
    public class SQLDBContext : DbContext
    {
        public SQLDBContext(DbContextOptions<SQLDBContext> options)
            : base(options) { }

        // =========================
        // DbSets (SQL Server Only)
        // =========================
        public DbSet<User> Users => Set<User>();
        public DbSet<UserCenter> UserCenters => Set<UserCenter>();
        public DbSet<Role> Roles => Set<Role>();

        public DbSet<Center> Centers => Set<Center>();
        public DbSet<Course> Courses => Set<Course>();
        
        // Term & Registrations
        public DbSet<Term> Terms => Set<Term>();
        public DbSet<CourseRegistration> CourseRegistrations => Set<CourseRegistration>();

        public DbSet<Class> Classes => Set<Class>();
        public DbSet<ClassStudent> ClassStudents => Set<ClassStudent>();
        public DbSet<ClassSchedule> ClassSchedules => Set<ClassSchedule>();
        public DbSet<Attendance> Attendances => Set<Attendance>();
        public DbSet<InstructorLeaveRequest> InstructorLeaveRequests => Set<InstructorLeaveRequest>();
        public DbSet<StudentDrivingDistance> StudentDrivingDistances => Set<StudentDrivingDistance>();

        // Exams
        public DbSet<ExamBatch> ExamBatches => Set<ExamBatch>();
        public DbSet<Exam> Exams => Set<Exam>();
        public DbSet<ExamRegistration> ExamRegistrations => Set<ExamRegistration>();
        public DbSet<ExamResult> ExamResults => Set<ExamResult>();
        // public DbSet<SampleExamResult> SampleExamResults => Set<SampleExamResult>();

        // Collaborators
        public DbSet<ReferralCode> ReferralCodes => Set<ReferralCode>();
        public DbSet<ReferralRegistration> ReferralRegistrations => Set<ReferralRegistration>();
        public DbSet<CollaboratorCommission> CollaboratorCommissions => Set<CollaboratorCommission>();

        // Permissions
        public DbSet<Document> Documents => Set<Document>();

        // Training Extras
        // public DbSet<StudentEvaluation> StudentEvaluations => Set<StudentEvaluation>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureBaseEntity(modelBuilder);

            ConfigureUser(modelBuilder);
            ConfigureRole(modelBuilder);

            ConfigureCenter(modelBuilder);
            ConfigureUserCenter(modelBuilder);

            ConfigureCourse(modelBuilder);

            ConfigureTerm(modelBuilder);
            ConfigureCourseRegistration(modelBuilder);

            ConfigureClass(modelBuilder);
            ConfigureClassStudent(modelBuilder);
            ConfigureClassSchedule(modelBuilder);
            ConfigureAttendance(modelBuilder);
            ConfigureInstructorLeaveRequest(modelBuilder);
            ConfigureStudentDrivingDistance(modelBuilder);

            ConfigureExamBatch(modelBuilder);
            ConfigureExam(modelBuilder);
            ConfigureExamRegistration(modelBuilder);
            ConfigureExamResult(modelBuilder);
            // ConfigureSampleExamResult(modelBuilder);

            ConfigureReferral(modelBuilder);
            ConfigureCollaboratorCommission(modelBuilder);

            ConfigureDocument(modelBuilder);
            // ConfigureStudentEvaluation(modelBuilder);

            ApplySeedData(modelBuilder);

            foreach (var fk in modelBuilder.Model
                 .GetEntityTypes()
                 .SelectMany(e => e.GetForeignKeys())
                 .Where(fk => fk.PrincipalEntityType.ClrType == typeof(User)))
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        // =========================
        // BaseEntity (Soft delete)
        // =========================
        private static void ConfigureBaseEntity(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var isDeletedProperty = Expression.Call(
                        typeof(EF),
                        nameof(EF.Property),
                        new[] { typeof(bool) },
                        parameter,
                        Expression.Constant("IsDeleted")
                    );

                    var compareExpression = Expression.Equal(
                        isDeletedProperty,
                        Expression.Constant(false)
                    );

                    var lambda = Expression.Lambda(compareExpression, parameter);

                    modelBuilder.Entity(entityType.ClrType)
                        .HasQueryFilter(lambda);
                }
            }
        }

        // =========================
        // User & Role
        // =========================
        private static void ConfigureUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("Users");
                e.HasKey(x => x.Id);

                e.Property(x => x.ClerkId)
                    .HasMaxLength(255)
                    .IsRequired();
                e.HasIndex(x => x.ClerkId).IsUnique();

                e.Property(x => x.Email)
                    .HasConversion(v => v.Value, v => EmailVO.Create(v))
                    .HasMaxLength(255)
                    .IsRequired();

                e.HasIndex(x => x.Email).IsUnique();

                e.Property(x => x.FullName).HasMaxLength(255);

                e.Property(x => x.Phone)
                    .HasConversion(v => v.Value, v => PhoneNumber.Create(v))
                    .HasMaxLength(20);

                e.Property(x => x.AvatarUrl).HasMaxLength(500);
                e.Property(x => x.IsActive);
                e.Property(x => x.LastLoginAt);

                e.Property(x => x.RoleId).HasConversion<int>();
            });
        }

        private static void ConfigureRole(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(e =>
            {
                e.ToTable("Roles");
                e.HasKey(x => x.Id);

                e.Property(x => x.RoleName)
                    .HasConversion<string>()
                    .HasMaxLength(50)
                    .IsRequired();
            });
        }


        // =========================
        // Permissions
        // =========================
        private static void ConfigureDocument(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>(e =>
            {
                e.ToTable("Documents");
                e.HasKey(x => x.Id);

                e.Property(x => x.ProviderPublicId).HasMaxLength(255).IsRequired();
                e.Property(x => x.Version).HasMaxLength(50).IsRequired();
                e.Property(x => x.ResourceType).HasMaxLength(20).IsRequired();
                e.Property(x => x.FileName).HasMaxLength(255).IsRequired();
                e.Property(x => x.Extension).HasMaxLength(10).IsRequired();
                e.Property(x => x.Size).IsRequired();
                e.Property(x => x.IsVerified).IsRequired();

                e.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        // =========================
        // Center
        // =========================
        private static void ConfigureCenter(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Center>(e =>
            {
                e.ToTable("Centers");
                e.HasKey(x => x.Id);

                e.Property(x => x.CenterName).HasMaxLength(255).IsRequired();
                e.Property(x => x.Address).HasMaxLength(500);

                e.Property(x => x.Phone)
                    .HasConversion(v => v.Value, v => PhoneNumber.Create(v))
                    .HasMaxLength(20);

                e.Property(x => x.Email)
                    .HasConversion(v => v.Value, v => EmailVO.Create(v))
                    .HasMaxLength(255);

                e.Property(x => x.IsActive);
                e.Property(x => x.NumberOfClasses);
                e.Property(x => x.MaxStudentPerClass);
            });
        }
        
        private static void ConfigureUserCenter(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserCenter>(e =>
            {
                e.ToTable("UserCenters");
                e.HasKey(x => new { x.UserId, x.CenterId });

                e.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne<Center>()
                    .WithMany()
                    .HasForeignKey(x => x.CenterId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        // =========================
        // Course & Terms & Registrations
        // =========================
        private static void ConfigureCourse(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>(e =>
            {
                e.ToTable("Courses");
                e.HasKey(x => x.Id);

                e.Property(x => x.CourseName).HasMaxLength(255).IsRequired();

                e.Property(x => x.LicenseType)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                e.Property(x => x.DurationInWeeks);
                e.Property(x => x.MaxStudents);

                e.Property(x => x.ThumbnailUrl).HasMaxLength(500);
                e.Property(x => x.Description);
                e.Property(x => x.Price).HasPrecision(18, 2);
                e.Property(x => x.IsActive);

                e.HasOne(x => x.Center)
                    .WithMany()
                    .HasForeignKey(x => x.CenterId);
            });
        }

        private static void ConfigureTerm(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Term>(e =>
            {
                e.ToTable("Terms");
                e.HasKey(x => x.Id);

                e.Property(x => x.TermName).HasMaxLength(255).IsRequired();
                e.Property(x => x.StartDate);
                e.Property(x => x.EndDate);
                e.Property(x => x.CurrentStudents);
                e.Property(x => x.MaxStudents);
                e.Property(x => x.IsActive);

                e.HasOne<Course>()
                    .WithMany()
                    .HasForeignKey(x => x.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureCourseRegistration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CourseRegistration>(e =>
            {
                e.ToTable("CourseRegistrations");
                e.HasKey(x => x.Id);

                e.Property(x => x.RegistrationDate);
                e.Property(x => x.TotalFee).HasPrecision(18,2);
                e.Property(x => x.Notes).HasMaxLength(1000);
                
                e.Property(x => x.Status)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                e.HasOne<Course>()
                    .WithMany()
                    .HasForeignKey(x => x.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        // =========================
        // Class & Schedule
        // =========================
        private static void ConfigureClass(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Class>(e =>
            {
                e.ToTable("Classes");
                e.HasKey(x => x.Id);

                e.Property(x => x.ClassName).HasMaxLength(255).IsRequired();
                e.Property(x => x.CurrentStudents);
                e.Property(x => x.MaxStudents);
                e.Property(x => x.ClassType)
                    .HasConversion<string>()
                    .HasMaxLength(50)
                    .IsRequired();
                
                e.Property(x => x.Status)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                // Links to Term per ERD
                e.HasOne<Term>()
                    .WithMany()
                    .HasForeignKey(x => x.TermId);

                // Một lớp một giáo viên chủ nhiệm; một user có thể chủ nhiệm nhiều lớp.
                e.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(x => x.InstructorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureClassStudent(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClassStudent>(e =>
            {
                e.ToTable("ClassStudents");
                e.HasKey(x => new { x.ClassId, x.StudentId });

                e.HasOne<Class>()
                    .WithMany()
                    .HasForeignKey(x => x.ClassId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(x => x.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void ConfigureClassSchedule(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClassSchedule>(e =>
            {
                e.ToTable("ClassSchedules");
                e.HasKey(x => x.Id);

                e.Property(x => x.AddressId).IsRequired();

                e.HasOne<Class>()
                    .WithMany()
                    .HasForeignKey(x => x.ClassId);

                e.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(x => x.InstructorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureAttendance(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attendance>(e =>
            {
                e.ToTable("Attendance");
                e.HasKey(x => x.Id);

                e.HasIndex(x => new { x.ClassScheduleId, x.StudentId }).IsUnique();

                e.Property(x => x.IsPresent);
                e.Property(x => x.CheckedAt);

                e.HasOne<ClassSchedule>()
                    .WithMany()
                    .HasForeignKey(x => x.ClassScheduleId);

                e.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(x => x.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureInstructorLeaveRequest(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InstructorLeaveRequest>(e =>
            {
                e.ToTable("InstructorLeaveRequests");
                e.HasKey(x => x.Id);

                e.Property(x => x.Reason).HasMaxLength(1000).IsRequired();
                e.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);

                e.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(x => x.InstructorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureStudentDrivingDistance(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentDrivingDistance>(e =>
            {
                e.ToTable("StudentDrivingDistances");
                e.HasKey(x => x.Id);

                e.Property(x => x.MorningDistanceKm);
                e.Property(x => x.EveningDistanceKm);
                e.Property(x => x.MaxMorningDistanceKm);
                e.Property(x => x.MaxEveningDistanceKm);

                e.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(x => x.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        // =========================
        // Exams & Registrations
        // =========================
        private static void ConfigureExamBatch(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExamBatch>(e =>
            {
                e.ToTable("ExamBatches");
                e.HasKey(x => x.Id);

                e.Property(x => x.BatchName).HasMaxLength(255).IsRequired();
                e.Property(x => x.RegistrationStartDate);
                e.Property(x => x.RegistrationEndDate);
                e.Property(x => x.ExamStartDate);
                
                e.Property(x => x.Status)
                    .HasConversion<string>()
                    .HasMaxLength(50);
            });
        }

        private static void ConfigureExamRegistration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExamRegistration>(e =>
            {
                e.ToTable("ExamRegistrations");
                e.HasKey(x => x.Id);

                e.Property(x => x.RegistrationDate);
                e.Property(x => x.IsPaid);
                
                e.Property(x => x.Status)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                e.HasOne<ExamBatch>()
                    .WithMany()
                    .HasForeignKey(x => x.ExamBatchId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(x => x.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }



        private static void ConfigureExam(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Exam>(e =>
            {
                e.ToTable("Exams");
                e.HasKey(x => x.Id);

                e.Property(x => x.ExamName).HasMaxLength(255);
                e.Property(x => x.ExamDate);
                e.Property(x => x.AddressId).IsRequired();
                
                e.Property(x => x.ExamType)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                e.Property(x => x.Status)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                e.Property(x => x.DurationMinutes);
                e.Property(x => x.TotalScore);
                e.Property(x => x.PassScore);

                // Links to ExamBatch per ERD
                e.HasOne<ExamBatch>()
                    .WithMany()
                    .HasForeignKey(x => x.ExamBatchId);

                e.HasOne<Course>()
                    .WithMany()
                    .HasForeignKey(x => x.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureExamResult(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExamResult>(e =>
            {
                e.ToTable("ExamResults");
                e.HasKey(x => x.Id);

                e.Property(x => x.AttemptNo);
                e.Property(x => x.Score);
                e.Property(x => x.IsPassed);

                e.HasOne<Exam>()
                    .WithMany()
                    .HasForeignKey(x => x.ExamId);

                e.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(x => x.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureSampleExamResult(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SampleExamResult>(e =>
            {
                e.ToTable("SampleExamResults");
                e.HasKey(x => x.Id);

                e.Property(x => x.TotalScore);
                e.Property(x => x.DurationSeconds);
                e.Property(x => x.IsPassed);
                e.Property(x => x.UserAnswersJson).HasColumnType("nvarchar(max)");

                e.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(x => x.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        // =========================
        // Referral & Commission
        // =========================
        private static void ConfigureReferral(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReferralCode>(e =>
            {
                e.ToTable("ReferralCodes");
                e.HasKey(x => x.Id);

                e.Property(x => x.Code).HasMaxLength(50).IsRequired();
                e.HasIndex(x => x.Code).IsUnique();

                e.Property(x => x.UsedCount);
                e.Property(x => x.IsActive);

                e.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(x => x.CollaboratorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ReferralRegistration>(entity =>
            {
                entity.ToTable("ReferralRegistrations");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.RegisteredAt);

                entity.HasOne<ReferralCode>()
                    .WithMany()
                    .HasForeignKey(x => x.ReferralCodeId)
                    .OnDelete(DeleteBehavior.Restrict); 

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(x => x.StudentId)
                    .OnDelete(DeleteBehavior.Restrict); 
            });

        }

        private static void ConfigureCollaboratorCommission(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CollaboratorCommission>(e =>
            {
                e.ToTable("CollaboratorCommissions");
                e.HasKey(x => x.Id);

                e.Property(x => x.Amount).HasPrecision(18, 2);

                e.Property(x => x.Status)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                e.Property(x => x.PaidAt);

                e.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(x => x.CollaboratorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureStudentEvaluation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentEvaluation>(e =>
            {
                e.ToTable("StudentEvaluations");
                e.HasKey(x => x.Id);

                e.Property(x => x.PunctualityScore);
                e.Property(x => x.SkillLevel);
                e.Property(x => x.Note).HasColumnType("nvarchar(max)");
                e.Property(x => x.EvaluationDate);

                e.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(x => x.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(x => x.InstructorId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne<Class>()
                    .WithMany()
                    .HasForeignKey(x => x.ClassId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }

        private static void ApplySeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(SqlSeedData.Roles);
            modelBuilder.Entity<User>().HasData(SqlSeedData.Users);
            modelBuilder.Entity<Center>().HasData(SqlSeedData.Centers);
            modelBuilder.Entity<UserCenter>().HasData(SqlSeedData.UserCenters);
            modelBuilder.Entity<Course>().HasData(SqlSeedData.Courses);
            modelBuilder.Entity<Term>().HasData(SqlSeedData.Terms);
            modelBuilder.Entity<CourseRegistration>().HasData(SqlSeedData.CourseRegistrations);
            modelBuilder.Entity<Class>().HasData(SqlSeedData.Classes);
            modelBuilder.Entity<ClassStudent>().HasData(SqlSeedData.ClassStudents);
            modelBuilder.Entity<ClassSchedule>().HasData(SqlSeedData.ClassSchedules);
            modelBuilder.Entity<Attendance>().HasData(SqlSeedData.Attendances);
            modelBuilder.Entity<InstructorLeaveRequest>().HasData(SqlSeedData.InstructorLeaveRequests);
            modelBuilder.Entity<StudentDrivingDistance>().HasData(SqlSeedData.StudentDrivingDistances);
            modelBuilder.Entity<ExamBatch>().HasData(SqlSeedData.ExamBatches);
            modelBuilder.Entity<Exam>().HasData(SqlSeedData.Exams);
            modelBuilder.Entity<ExamRegistration>().HasData(SqlSeedData.ExamRegistrations);
            modelBuilder.Entity<ExamResult>().HasData(SqlSeedData.ExamResults);
            modelBuilder.Entity<ReferralCode>().HasData(SqlSeedData.ReferralCodes);
            modelBuilder.Entity<ReferralRegistration>().HasData(SqlSeedData.ReferralRegistrations);
            modelBuilder.Entity<CollaboratorCommission>().HasData(SqlSeedData.CollaboratorCommissions);
            modelBuilder.Entity<Document>().HasData(SqlSeedData.Documents);
            modelBuilder.Entity<StudentEvaluation>().HasData(SqlSeedData.StudentEvaluations);
            modelBuilder.Entity<SampleExamResult>().HasData(SqlSeedData.SampleExamResults);
        }
    }
}
