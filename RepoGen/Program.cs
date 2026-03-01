using System;
using System.IO;
using System.Collections.Generic;

namespace RepositoryImplGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var sqlEntities = new Dictionary<string, (string Namespace, string ClassName)>
            {
                { "Classes/ClassRepository.cs", ("Classes", "Class") },
                { "Classes/ClassScheduleRepository.cs", ("Classes", "ClassSchedule") },
                { "Classes/AttendanceRepository.cs", ("Classes", "Attendance") },
                
                { "Collaborators/ReferralCodeRepository.cs", ("Collaborators", "ReferralCode") },
                { "Collaborators/ReferralRegistrationRepository.cs", ("Collaborators", "ReferralRegistration") },
                { "Collaborators/CollaboratorCommissionRepository.cs", ("Collaborators", "CollaboratorCommission") },
                
                { "Exams/ExamRepository.cs", ("Exams", "Exam") },
                { "Exams/ExamBatchRepository.cs", ("Exams", "ExamBatch") },
                { "Exams/ExamRegistrationRepository.cs", ("Exams", "ExamRegistration") },
                { "Exams/ExamResultRepository.cs", ("Exams", "ExamResult") },
                
                { "Location/CenterRepository.cs", ("Location", "Center") },
                
                { "Permissions/UserRepository.cs", ("Permissions", "User") },
                { "Permissions/RoleRepository.cs", ("Permissions", "Role") },
                { "Permissions/DocumentRepository.cs", ("Permissions", "Document") },
                
                { "Terms/TermRepository.cs", ("Terms", "Term") },
                { "Terms/CourseRegistrationRepository.cs", ("Terms", "CourseRegistration") },
                
                { "Training/CourseRepository.cs", ("Training", "Course") }
            };

            var mongoEntities = new Dictionary<string, (string Namespace, string ClassName)>
            {
                { "Blogs/BlogRepository.cs", ("Blogs", "Blog") },
                { "Blogs/CategoryRepository.cs", ("Blogs", "Category") },
                
                { "Exams/QuestionRepository.cs", ("Exams", "Question") },
                { "Exams/SampleExamRepository.cs", ("Exams", "SampleExam") },
                { "Exams/SampleExamQuestionRepository.cs", ("Exams", "SampleExamQuestion") },
                
                { "Location/AddressRepository.cs", ("Location", "Address") },
                
                { "Notifications/NotificationRepository.cs", ("Notifications", "Notification") },
                { "Notifications/NotificationRoleRepository.cs", ("Notifications", "NotificationRole") },
                { "Notifications/UserNotificationRepository.cs", ("Notifications", "UserNotification") },
                
                { "Training/LearningRoadmapRepository.cs", ("Training", "LearningRoadmap") },
                { "Training/ResourceLearningRepository.cs", ("Training", "ResourceLearning") }
            };

            string baseDir = @"d:\Project_Sample\driving-training-centers-project-v1\repo-backend\dtcproject\dtc.Infrastructure\Repositories";

            // Create directories if they don't exist
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            // ==========================================
            // 1. GENERATE SQL GENERIC REPO
            // ==========================================
            string genericRepoPath = Path.Combine(baseDir, "GenericRepository.cs");
            string genericContent = @"using dtc.Domain.Interfaces;
using dtc.Infrastructure.Pesistence.SQLServer;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace dtc.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly SQLDBContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(SQLDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(T entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }
    }
}
";
            File.WriteAllText(genericRepoPath, genericContent);
            
            // ==========================================
            // 2. GENERATE MONGO GENERIC REPO
            // ==========================================
            string mongoRepoPath = Path.Combine(baseDir, "MongoGenericRepository.cs");
            string mongoContent = @"using dtc.Domain.Interfaces;
using dtc.Infrastructure.Persistence.MongoDB;
using MongoDB.Driver;
using System.Linq.Expressions;
using dtc.Domain.Entities; // for BaseEntity if using Guid Id

namespace dtc.Infrastructure.Repositories
{
    public class MongoGenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly IMongoCollection<T> _collection;

        public MongoGenericRepository(MongoDBContext context, string collectionName)
        {
            // Reflection mapping based on context, or pass direct collection
            _collection = (IMongoCollection<T>)context.GetType().GetProperty(collectionName)?.GetValue(context)!;
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            if (id is Guid guidId && typeof(BaseEntity).IsAssignableFrom(typeof(T)))
            {
                var filter = Builders<T>.Filter.Eq(""Id"", guidId);
                return await _collection.Find(filter).FirstOrDefaultAsync();
            }
            // For integer Ids like Address
            var filterInt = Builders<T>.Filter.Eq(""Id"", id);
            return await _collection.Find(filterInt).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            var idProperty = entity.GetType().GetProperty(""Id"");
            var idValue = idProperty?.GetValue(entity);
            if (idValue != null)
            {
                var filter = Builders<T>.Filter.Eq(""Id"", idValue);
                await _collection.ReplaceOneAsync(filter, entity);
            }
        }

        public async Task RemoveAsync(T entity)
        {
            var idProperty = entity.GetType().GetProperty(""Id"");
            var idValue = idProperty?.GetValue(entity);
            if (idValue != null)
            {
                var filter = Builders<T>.Filter.Eq(""Id"", idValue);
                await _collection.DeleteOneAsync(filter);
            }
        }
    }
}
";
            File.WriteAllText(mongoRepoPath, mongoContent);

            // ==========================================
            // 3. GENERATE UNIT OF WORK (SQL)
            // ==========================================
            string uowPath = Path.Combine(baseDir, "UnitOfWork.cs");
            string uowContent = @"using dtc.Domain.Interfaces;
using dtc.Infrastructure.Pesistence.SQLServer;

namespace dtc.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SQLDBContext _context;

        public UnitOfWork(SQLDBContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
";
            File.WriteAllText(uowPath, uowContent);

            // ==========================================
            // 4. GENERATE INDIVIDUAL SQL REPOSITORIES
            // ==========================================
            foreach (var kvp in sqlEntities)
            {
                string filePath = Path.Combine(baseDir, kvp.Key);
                string folderPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                string content = $@"using dtc.Domain.Entities.{kvp.Value.Namespace};
using dtc.Domain.Interfaces.{kvp.Value.Namespace};
using dtc.Infrastructure.Pesistence.SQLServer;

namespace dtc.Infrastructure.Repositories.{kvp.Value.Namespace}
{{
    public class {kvp.Value.ClassName}Repository : GenericRepository<{kvp.Value.ClassName}>, I{kvp.Value.ClassName}Repository
    {{
        public {kvp.Value.ClassName}Repository(SQLDBContext context) : base(context)
        {{
        }}
    }}
}}
";
                File.WriteAllText(filePath, content);
            }

            // ==========================================
            // 5. GENERATE INDIVIDUAL MONGO REPOSITORIES
            // ==========================================
            foreach (var kvp in mongoEntities)
            {
                string filePath = Path.Combine(baseDir, kvp.Key);
                string folderPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                // For generic collection name convention, map singular class to plural collection (e.g. Blog -> Blogs, Category -> Categories)
                string collectionName = kvp.Value.ClassName;
                if (collectionName.EndsWith("y")) collectionName = collectionName.Substring(0, collectionName.Length - 1) + "ies";
                else if (collectionName.EndsWith("s")) collectionName = collectionName + "es";
                else collectionName = collectionName + "s";

                string content = $@"using dtc.Domain.Entities.{kvp.Value.Namespace};
using dtc.Domain.Interfaces.{kvp.Value.Namespace};
using dtc.Infrastructure.Persistence.MongoDB;

namespace dtc.Infrastructure.Repositories.{kvp.Value.Namespace}
{{
    public class {kvp.Value.ClassName}Repository : MongoGenericRepository<{kvp.Value.ClassName}>, I{kvp.Value.ClassName}Repository
    {{
        public {kvp.Value.ClassName}Repository(MongoDBContext context) : base(context, ""{collectionName}"")
        {{
        }}
    }}
}}
";
                File.WriteAllText(filePath, content);
            }

            Console.WriteLine("Infrastructure Repositories Generated Successfully!");
        }
    }
}
