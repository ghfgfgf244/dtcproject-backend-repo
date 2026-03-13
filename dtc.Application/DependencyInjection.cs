using dtc.Application.Features.Auth.Interfaces;
using dtc.Application.Features.Auth.Services;
using dtc.Application.Features.Users.Interfaces;
using dtc.Application.Features.Users.Services;
using dtc.Application.Features.Exams.Interfaces;
using dtc.Application.Features.Exams.Services;
using dtc.Application.Features.Training.Interfaces;
using dtc.Application.Features.Training.Services;
using dtc.Application.Features.Collaborators.Interfaces;
using dtc.Application.Features.Collaborators.Services;
using dtc.Application.Features.Dashboards.Interfaces;
using dtc.Application.Features.Dashboards.Services;
using dtc.Application.Features.Location.Interfaces;
using dtc.Application.Features.Location.Services;
using dtc.Application.Features.Notifications.Interfaces;
using dtc.Application.Features.Notifications.Services;
using dtc.Application.Features.Permissions.Interfaces;
using dtc.Application.Features.Permissions.Services;
using Microsoft.Extensions.DependencyInjection;

namespace dtc.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Feature: Auth
            services.AddScoped<IAuthService, AuthService>();

            // Feature: Users
            services.AddScoped<IUserService, UserService>();

            // Feature: Exams
            services.AddScoped<IExamBatchService, ExamBatchService>();
            services.AddScoped<IExamRegistrationService, ExamRegistrationService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<ISampleExamService, SampleExamService>();

            // Feature: Training
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ILearningRoadmapService, LearningRoadmapService>();
            services.AddScoped<IResourceLearningService, ResourceLearningService>();
            services.AddScoped<IStudentEvaluationService, StudentEvaluationService>();
            services.AddScoped<ITermService, TermService>();
            services.AddScoped<IClassService, ClassService>();
            services.AddScoped<ICourseRegistrationService, CourseRegistrationService>();

            // Feature: Collaborators
            services.AddScoped<ICollaboratorService, CollaboratorService>();

            // Feature: Dashboards
            services.AddScoped<IDashboardService, DashboardService>();

            // Feature: Location
            services.AddScoped<ICenterService, CenterService>();

            // Feature: Notifications
            services.AddScoped<INotificationService, NotificationService>();

            // Feature: Permissions
            services.AddScoped<IDocumentService, DocumentService>();

            return services;
        }
    }
}
