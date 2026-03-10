using dtc.Application.Interfaces;
using dtc.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace dtc.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<dtc.Application.Interfaces.Training.IScheduleService, dtc.Application.Services.Training.ScheduleService>();
            services.AddScoped<dtc.Application.Interfaces.Training.IAttendanceService, dtc.Application.Services.Training.AttendanceService>();
            services.AddScoped<dtc.Application.Interfaces.Exams.IExamBatchService, dtc.Application.Services.Exams.ExamBatchService>();
            services.AddScoped<dtc.Application.Interfaces.Exams.IExamRegistrationService, dtc.Application.Services.Exams.ExamRegistrationService>();
            services.AddScoped<dtc.Application.Interfaces.Exams.IExamService, dtc.Application.Services.Exams.ExamService>();
            services.AddScoped<dtc.Application.Interfaces.Notifications.INotificationService, dtc.Application.Services.Notifications.NotificationService>();
            services.AddScoped<dtc.Application.Interfaces.Training.ICourseService, dtc.Application.Services.Training.CourseService>();
            services.AddScoped<dtc.Application.Interfaces.Training.ITermService, dtc.Application.Services.Training.TermService>();
            services.AddScoped<dtc.Application.Interfaces.Training.IClassService, dtc.Application.Services.Training.ClassService>();
            services.AddScoped<dtc.Application.Interfaces.Training.ICourseRegistrationService, dtc.Application.Services.Training.CourseRegistrationService>();
            services.AddScoped<dtc.Application.Interfaces.Location.ICenterService, dtc.Application.Services.Location.CenterService>();

            return services;
        }
    }
}
