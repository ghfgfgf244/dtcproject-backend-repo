using MongoDB.Driver;
using dtc.Domain.Entities.Blogs;
using dtc.Domain.Entities.Exams;
using dtc.Domain.Entities.Location;
using dtc.Domain.Entities.Notifications;
using dtc.Domain.Entities.Training;
using Microsoft.Extensions.Configuration;

namespace dtc.Infrastructure.Persistence.MongoDB
{
    public class MongoDBContext
    {
        private readonly IMongoDatabase _database;

        public MongoDBContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDbConnection");
            var mongoUrl = new MongoUrl(connectionString);
            var mongoClient = new MongoClient(mongoUrl);

            _database = mongoClient.GetDatabase(mongoUrl.DatabaseName ?? "dtcproject");
        }

        public virtual IMongoCollection<Blog> Blogs => _database.GetCollection<Blog>("Blogs");
        public virtual IMongoCollection<Category> Categories => _database.GetCollection<Category>("Categories");
        
        public virtual IMongoCollection<SampleExam> SampleExams => _database.GetCollection<SampleExam>("SampleExams");
        public virtual IMongoCollection<SampleExamQuestion> SampleExamQuestions => _database.GetCollection<SampleExamQuestion>("SampleExamQuestions");
        public virtual IMongoCollection<Question> Questions => _database.GetCollection<Question>("Questions");
        
        public virtual IMongoCollection<Address> Addresses => _database.GetCollection<Address>("Addresses");
        
        public virtual IMongoCollection<Notification> Notifications => _database.GetCollection<Notification>("Notifications");
        public virtual IMongoCollection<NotificationRole> NotificationRoles => _database.GetCollection<NotificationRole>("NotificationRoles");
        public virtual IMongoCollection<UserNotification> UserNotifications => _database.GetCollection<UserNotification>("UserNotifications");
        
        public virtual IMongoCollection<LearningRoadmap> LearningRoadmaps => _database.GetCollection<LearningRoadmap>("LearningRoadmaps");
        public virtual IMongoCollection<ResourceLearning> ResourceLearnings => _database.GetCollection<ResourceLearning>("ResourceLearnings");
    }
}
