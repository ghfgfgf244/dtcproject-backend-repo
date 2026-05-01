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
using dtc.Application.Interfaces;
using dtc.Infrastructure.Services;
using dtc.Application.Features.Email.Interfaces;
using dtc.Infrastructure.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using dtc.Infrastructure.Configurations;
using dtc.Application.Features.AI.Interfaces;
using dtc.Infrastructure.AI;
using System;

namespace dtc.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<AiSettings>(opts => configuration.GetSection(AiSettings.SectionName).Bind(opts));
            services.Configure<GeminiSettings>(opts => configuration.GetSection(GeminiSettings.SectionName).Bind(opts));
            services.Configure<UpstashRedisSettings>(opts => configuration.GetSection(UpstashRedisSettings.SectionName).Bind(opts));
            services.Configure<UpstashVectorSettings>(opts => configuration.GetSection(UpstashVectorSettings.SectionName).Bind(opts));

            services.AddHttpClient<IGeminiClient, GeminiClient>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<GeminiSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(60);
            });

            services.AddHttpClient<IEmbeddingService, EmbeddingService>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<GeminiSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(60);
            });

            services.AddHttpClient<IVectorSearchService, UpstashVectorService>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<UpstashVectorSettings>>().Value;
                client.BaseAddress = new Uri(settings.Endpoint.TrimEnd('/'));
                client.Timeout = TimeSpan.FromSeconds(60);
            });

            // Register Generic Repositories - optional, normally injected via specific repos
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Register specific repositories
            services.AddScoped<IBlogRepository, BlogRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<IClassRepository, ClassRepository>();
            services.AddScoped<IClassStudentRepository, ClassStudentRepository>();
            services.AddScoped<IClassScheduleRepository, ClassScheduleRepository>();
            services.AddScoped<ILearningLocationRepository, LearningLocationRepository>();
            services.AddScoped<IInstructorLeaveRequestRepository, InstructorLeaveRequestRepository>();
            services.AddScoped<IStudentDrivingDistanceRepository, StudentDrivingDistanceRepository>();

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
            services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();

            services.AddScoped<dtc.Domain.Interfaces.Location.ICenterRepository, dtc.Infrastructure.Repositories.Location.CenterRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserCenterRepository, UserCenterRepository>();

            services.AddScoped<ICourseRegistrationRepository, CourseRegistrationRepository>();
            services.AddScoped<ITermRepository, TermRepository>();

            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ILearningRoadmapRepository, LearningRoadmapRepository>();
            services.AddScoped<IResourceLearningRepository, ResourceLearningRepository>();

            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register Cache Service
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IAiCacheService, UpstashRedisCacheService>();
            services.AddScoped<IApiKeyRotationStore, RedisApiKeyRotationStore>();
            services.AddScoped<IAiRouterService, AiRouterService>();

            // Register Email Service (SMTP)
            services.Configure<SmtpSettings>(opts => configuration.GetSection(SmtpSettings.SectionName).Bind(opts));
            services.AddScoped<IEmailService, SmtpEmailService>();

            // Register Cloudinary Service
            services.Configure<CloudinarySettings>(opts => configuration.GetSection(CloudinarySettings.SectionName).Bind(opts));
            services.AddScoped<ICloudinaryService, CloudinaryService>();

            // Authentication & Authorization (Clerk)
            services.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opts =>
            {
                opts.Authority = configuration["Clerk:Authority"]; // e.g. https://clerk.yourdomain.com
                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Clerk:Authority"],
                    ValidateAudience = false, // Clerk doesn't require audience for session tokens by default
                    ValidateLifetime = true,
                    // Allow small clock drift between Clerk-issued tokens and local/backend machine time.
                    // This prevents redirect/login loops caused by "token is not yet valid" during sign-in.
                    ClockSkew = TimeSpan.FromMinutes(5),
                    RoleClaimType = "role"
                };
            });

            // Register Claims Transformation for Clerk Roles
            services.AddScoped<Microsoft.AspNetCore.Authentication.IClaimsTransformation, dtc.Infrastructure.Auth.ClerkClaimsTransformer>();

            return services;
        }
    }
}
