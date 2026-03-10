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
using dtc.Infrastructure.Repositories;
using dtc.Infrastructure.Repositories.Blogs;
using dtc.Infrastructure.Repositories.Classes;
using dtc.Infrastructure.Repositories.Collaborators;
using dtc.Infrastructure.Repositories.Exams;
using dtc.Infrastructure.Persistence.Repositories.Exams;
using dtc.Infrastructure.Repositories.Location;
using dtc.Infrastructure.Repositories.Notifications;
using dtc.Infrastructure.Repositories.Permissions;
using dtc.Infrastructure.Repositories.Terms;
using dtc.Infrastructure.Repositories.Training;
using dtc.Infrastructure.Persistence.Repositories.Training;
using Microsoft.Extensions.DependencyInjection;

namespace dtc.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Register Generic Repositories - optional, normally injected via specific repos
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Register specific repositories
            services.AddScoped<IBlogRepository, BlogRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<IClassRepository, ClassRepository>();
            services.AddScoped<IClassScheduleRepository, ClassScheduleRepository>();

            services.AddScoped<ICollaboratorCommissionRepository, CollaboratorCommissionRepository>();
            services.AddScoped<IReferralCodeRepository, ReferralCodeRepository>();
            services.AddScoped<IReferralRegistrationRepository, ReferralRegistrationRepository>();

            services.AddScoped<IExamBatchRepository, ExamBatchRepository>();
            services.AddScoped<IExamRegistrationRepository, ExamRegistrationRepository>();
            services.AddScoped<IExamRepository, ExamRepository>();
            services.AddScoped<IExamResultRepository, ExamResultRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<ISampleExamQuestionRepository, SampleExamQuestionRepository>();
            services.AddScoped<ISampleExamRepository, SampleExamRepository>();
            services.AddScoped<ISampleExamResultRepository, SampleExamResultRepository>();

            services.AddScoped<IStudentEvaluationRepository, StudentEvaluationRepository>();

            services.AddScoped<IAddressRepository, AddressRepository>();
            
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<INotificationRoleRepository, NotificationRoleRepository>();
            services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();

            services.AddScoped<dtc.Domain.Interfaces.Location.ICenterRepository, dtc.Infrastructure.Repositories.Location.CenterRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<ICourseRegistrationRepository, CourseRegistrationRepository>();
            services.AddScoped<ITermRepository, TermRepository>();

            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ILearningRoadmapRepository, LearningRoadmapRepository>();
            services.AddScoped<IResourceLearningRepository, ResourceLearningRepository>();

            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
