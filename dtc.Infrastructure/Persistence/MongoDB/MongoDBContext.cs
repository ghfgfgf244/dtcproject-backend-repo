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

        public IMongoCollection<Blog> Blogs => _database.GetCollection<Blog>("Blogs");
        public IMongoCollection<Category> Categories => _database.GetCollection<Category>("Categories");
        
        public IMongoCollection<SampleExam> SampleExams => _database.GetCollection<SampleExam>("SampleExams");
        public IMongoCollection<SampleExamQuestion> SampleExamQuestions => _database.GetCollection<SampleExamQuestion>("SampleExamQuestions");
        public IMongoCollection<Question> Questions => _database.GetCollection<Question>("Questions");
        
        public IMongoCollection<Address> Addresses => _database.GetCollection<Address>("Addresses");
        
        public IMongoCollection<Notification> Notifications => _database.GetCollection<Notification>("Notifications");
        public IMongoCollection<NotificationRole> NotificationRoles => _database.GetCollection<NotificationRole>("NotificationRoles");
        public IMongoCollection<UserNotification> UserNotifications => _database.GetCollection<UserNotification>("UserNotifications");
        
        public IMongoCollection<LearningRoadmap> LearningRoadmaps => _database.GetCollection<LearningRoadmap>("LearningRoadmaps");
        public IMongoCollection<ResourceLearning> ResourceLearnings => _database.GetCollection<ResourceLearning>("ResourceLearnings");
    }
}
