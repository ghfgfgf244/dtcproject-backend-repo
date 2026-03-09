using dtc.Domain.Entities;
using dtc.Domain.Entities.Classes;
using dtc.Domain.Entities.Collaborators;
using dtc.Domain.Entities.Exams;
using dtc.Domain.Entities.Permissions;
using dtc.Domain.Entities.Training;
using dtc.Domain.Entities.Terms;
using dtc.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
        public DbSet<Role> Roles => Set<Role>();

        public DbSet<Center> Centers => Set<Center>();
        public DbSet<Course> Courses => Set<Course>();
        
        // Term & Registrations
        public DbSet<Term> Terms => Set<Term>();
        public DbSet<CourseRegistration> CourseRegistrations => Set<CourseRegistration>();

        public DbSet<Class> Classes => Set<Class>();
        public DbSet<ClassSchedule> ClassSchedules => Set<ClassSchedule>();
        public DbSet<Attendance> Attendances => Set<Attendance>();

        // Exams
        public DbSet<ExamBatch> ExamBatches => Set<ExamBatch>();
        public DbSet<Exam> Exams => Set<Exam>();
        public DbSet<ExamRegistration> ExamRegistrations => Set<ExamRegistration>();
        public DbSet<ExamResult> ExamResults => Set<ExamResult>();

        // Collaborators
        public DbSet<ReferralCode> ReferralCodes => Set<ReferralCode>();
        public DbSet<ReferralRegistration> ReferralRegistrations => Set<ReferralRegistration>();
        public DbSet<CollaboratorCommission> CollaboratorCommissions => Set<CollaboratorCommission>();

        // Permissions
        public DbSet<Document> Documents => Set<Document>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureBaseEntity(modelBuilder);

            ConfigureUser(modelBuilder);
            ConfigureRole(modelBuilder);
            ConfigureUserRole(modelBuilder);

            ConfigureCenter(modelBuilder);
            ConfigureUserCenter(modelBuilder);

            ConfigureCourse(modelBuilder);

            ConfigureTerm(modelBuilder);
            ConfigureCourseRegistration(modelBuilder);

            ConfigureClass(modelBuilder);
            ConfigureClassSchedule(modelBuilder);
            ConfigureAttendance(modelBuilder);

            ConfigureExamBatch(modelBuilder);
            ConfigureExam(modelBuilder);
            ConfigureExamRegistration(modelBuilder);
            ConfigureExamResult(modelBuilder);

            ConfigureReferral(modelBuilder);
            ConfigureCollaboratorCommission(modelBuilder);

            ConfigureDocument(modelBuilder);

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

                e.Property(x => x.Email)
                    .HasConversion(v => v.Value, v => Email.Create(v))
                    .HasMaxLength(255)
                    .IsRequired();

                e.HasIndex(x => x.Email).IsUnique();

                e.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
                e.Property(x => x.FullName).HasMaxLength(255);

                e.Property(x => x.Phone)
                    .HasConversion(v => v.Value, v => PhoneNumber.Create(v))
                    .HasMaxLength(20);

                e.Property(x => x.AvatarUrl).HasMaxLength(500);
                e.Property(x => x.IsActive);
                e.Property(x => x.LastLoginAt);
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

        private static void ConfigureUserRole(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "UserRoles",

                    r => r.HasOne<Role>()
                          .WithMany()
                          .HasForeignKey("RoleId")
                          .OnDelete(DeleteBehavior.Cascade),

                    l => l.HasOne<User>()
                          .WithMany()
                          .HasForeignKey("UserId")
                          .OnDelete(DeleteBehavior.Cascade),

                    j =>
                    {
                        j.ToTable("UserRoles");
                        j.HasKey("UserId", "RoleId");

                        j.Property<Guid>("UserId");
                        j.Property<int>("RoleId");
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
                // Configuration based on future properties of Document
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
                    .HasConversion(v => v.Value, v => Email.Create(v))
                    .HasMaxLength(255);

                e.Property(x => x.IsActive);
            });
        }
        
        private static void ConfigureUserCenter(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany<Center>()
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "UserCenters",

                    r => r.HasOne<Center>()
                          .WithMany()
                          .HasForeignKey("CenterId")
                          .OnDelete(DeleteBehavior.Cascade),

                    l => l.HasOne<User>()
                          .WithMany()
                          .HasForeignKey("UserId")
                          .OnDelete(DeleteBehavior.Cascade),

                    j =>
                    {
                        j.ToTable("UserCenters");
                        j.HasKey("UserId", "CenterId");

                        j.Property<Guid>("UserId");
                        j.Property<Guid>("CenterId");
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

                e.HasOne<Center>()
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
                
                e.Property(x => x.Status)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                // Links to Term per ERD
                e.HasOne<Term>()
                    .WithMany()
                    .HasForeignKey(x => x.TermId);
            });

            // Map User (Student) to Class many-to-many relationship
            modelBuilder.Entity<User>()
                .HasMany<Class>()
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "ClassStudents",

                    r => r.HasOne<Class>()
                          .WithMany()
                          .HasForeignKey("ClassId")
                          .OnDelete(DeleteBehavior.Cascade),

                    l => l.HasOne<User>()
                          .WithMany()
                          .HasForeignKey("StudentId")
                          .OnDelete(DeleteBehavior.Cascade),

                    j =>
                    {
                        j.ToTable("ClassStudents");
                        j.HasKey("StudentId", "ClassId");

                        j.Property<Guid>("StudentId");
                        j.Property<Guid>("ClassId");
                    });
        }

        private static void ConfigureClassSchedule(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClassSchedule>(e =>
            {
                e.ToTable("ClassSchedules");
                e.HasKey(x => x.Id);

                e.Property(x => x.Location).HasMaxLength(255);

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

                e.HasOne<Course>()
                    .WithMany()
                    .HasForeignKey(x => x.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);
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
    }
}
