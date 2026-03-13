using dtc.Domain.Interfaces;
using dtc.Domain.Interfaces.Blogs;
using dtc.Domain.Interfaces.Classes;
using dtc.Domain.Interfaces.Collaborators;
using dtc.Domain.Interfaces.Exams;
using dtc.Domain.Interfaces.Location;
using dtc.Domain.Interfaces.Notifications;
using dtc.Domain.Interfaces.Permissions;
using dtc.Domain.Interfaces.Terms;
using dtc.Domain.Interfaces.Training;
using dtc.Infrastructure.Pesistence.SQLServer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace dtc.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SQLDBContext _context;
        private readonly IServiceProvider _serviceProvider;

        public UnitOfWork(SQLDBContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
        }

        // Blogs
        public IBlogRepository Blogs => _serviceProvider.GetRequiredService<IBlogRepository>();
        public ICategoryRepository Categories => _serviceProvider.GetRequiredService<ICategoryRepository>();

        // Classes
        public IAttendanceRepository Attendances => _serviceProvider.GetRequiredService<IAttendanceRepository>();
        public IClassRepository Classes => _serviceProvider.GetRequiredService<IClassRepository>();
        public IClassScheduleRepository ClassSchedules => _serviceProvider.GetRequiredService<IClassScheduleRepository>();

        // Collaborators
        public ICollaboratorCommissionRepository CollaboratorCommissions => _serviceProvider.GetRequiredService<ICollaboratorCommissionRepository>();
        public IReferralCodeRepository ReferralCodes => _serviceProvider.GetRequiredService<IReferralCodeRepository>();
        public IReferralRegistrationRepository ReferralRegistrations => _serviceProvider.GetRequiredService<IReferralRegistrationRepository>();

        // Exams
        public IExamBatchRepository ExamBatches => _serviceProvider.GetRequiredService<IExamBatchRepository>();
        public IExamRegistrationRepository ExamRegistrations => _serviceProvider.GetRequiredService<IExamRegistrationRepository>();
        public IExamRepository Exams => _serviceProvider.GetRequiredService<IExamRepository>();
        public IExamResultRepository ExamResults => _serviceProvider.GetRequiredService<IExamResultRepository>();
        public IQuestionRepository Questions => _serviceProvider.GetRequiredService<IQuestionRepository>();
        public ISampleExamQuestionRepository SampleExamQuestions => _serviceProvider.GetRequiredService<ISampleExamQuestionRepository>();
        public ISampleExamRepository SampleExams => _serviceProvider.GetRequiredService<ISampleExamRepository>();
        public ISampleExamResultRepository SampleExamResults => _serviceProvider.GetRequiredService<ISampleExamResultRepository>();

        // Location
        public IAddressRepository Addresses => _serviceProvider.GetRequiredService<IAddressRepository>();
        
        // Notifications
        public INotificationRepository Notifications => _serviceProvider.GetRequiredService<INotificationRepository>();
        public INotificationRoleRepository NotificationRoles => _serviceProvider.GetRequiredService<INotificationRoleRepository>();
        public IUserNotificationRepository UserNotifications => _serviceProvider.GetRequiredService<IUserNotificationRepository>();

        // Permissions
        public dtc.Domain.Interfaces.Location.ICenterRepository Centers => _serviceProvider.GetRequiredService<dtc.Domain.Interfaces.Location.ICenterRepository>();
        public IDocumentRepository Documents => _serviceProvider.GetRequiredService<IDocumentRepository>();
        public IRoleRepository Roles => _serviceProvider.GetRequiredService<IRoleRepository>();
        public IUserRepository Users => _serviceProvider.GetRequiredService<IUserRepository>();

        // Terms
        public ICourseRegistrationRepository CourseRegistrations => _serviceProvider.GetRequiredService<ICourseRegistrationRepository>();
        public ITermRepository Terms => _serviceProvider.GetRequiredService<ITermRepository>();

        // Training
        public ICourseRepository Courses => _serviceProvider.GetRequiredService<ICourseRepository>();
        public ILearningRoadmapRepository LearningRoadmaps => _serviceProvider.GetRequiredService<ILearningRoadmapRepository>();
        public IResourceLearningRepository ResourceLearnings => _serviceProvider.GetRequiredService<IResourceLearningRepository>();
        public IStudentEvaluationRepository StudentEvaluations => _serviceProvider.GetRequiredService<IStudentEvaluationRepository>();

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            if (_context.Database.CurrentTransaction == null)
            {
                await _context.Database.BeginTransactionAsync();
            }
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync();
                if (_context.Database.CurrentTransaction != null)
                {
                    await _context.Database.CommitTransactionAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_context.Database.CurrentTransaction != null)
            {
                await _context.Database.RollbackTransactionAsync();
            }
        }

        public void Dispose()
        {
            _context.Database.CurrentTransaction?.Dispose();
            _context.Dispose();
        }
    }
}
