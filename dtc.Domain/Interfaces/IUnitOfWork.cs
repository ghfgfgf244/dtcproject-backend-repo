using System;
using System.Threading.Tasks;
using dtc.Domain.Interfaces.Blogs;
using dtc.Domain.Interfaces.Classes;
using dtc.Domain.Interfaces.Collaborators;
using dtc.Domain.Interfaces.Exams;
using dtc.Domain.Interfaces.Location;
using dtc.Domain.Interfaces.Notifications;
using dtc.Domain.Interfaces.Permissions;
using dtc.Domain.Interfaces.Terms;
using dtc.Domain.Interfaces.Training;

namespace dtc.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Blogs
        IBlogRepository Blogs { get; }
        ICategoryRepository Categories { get; }

        // Classes
        IAttendanceRepository Attendances { get; }
        IClassRepository Classes { get; }
        IClassScheduleRepository ClassSchedules { get; }

        // Collaborators
        ICollaboratorCommissionRepository CollaboratorCommissions { get; }
        IReferralCodeRepository ReferralCodes { get; }
        IReferralRegistrationRepository ReferralRegistrations { get; }

        // Exams
        IExamBatchRepository ExamBatches { get; }
        IExamRegistrationRepository ExamRegistrations { get; }
        IExamRepository Exams { get; }
        IExamResultRepository ExamResults { get; }
        IQuestionRepository Questions { get; }
        ISampleExamQuestionRepository SampleExamQuestions { get; }
        ISampleExamRepository SampleExams { get; }

        // Location
        IAddressRepository Addresses { get; }
        
        // Notifications
        INotificationRepository Notifications { get; }
        INotificationRoleRepository NotificationRoles { get; }
        IUserNotificationRepository UserNotifications { get; }

        // Permissions
        dtc.Domain.Interfaces.Location.ICenterRepository Centers { get; } // Specify namespace if ambiguous
        IDocumentRepository Documents { get; }
        IRoleRepository Roles { get; }
        IUserRepository Users { get; }

        // Terms
        ICourseRegistrationRepository CourseRegistrations { get; }
        ITermRepository Terms { get; }

        // Training
        ICourseRepository Courses { get; }
        ILearningRoadmapRepository LearningRoadmaps { get; }
        IResourceLearningRepository ResourceLearnings { get; }

        Task<int> SaveChangesAsync();
    }
}
