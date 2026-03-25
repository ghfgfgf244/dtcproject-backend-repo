using dtc.Domain.Entities.Classes;
using dtc.Domain.Interfaces.Classes;
using dtc.Infrastructure.Pesistence.SQLServer;
using Microsoft.EntityFrameworkCore;

namespace dtc.Infrastructure.Repositories.Classes
{
    public class ClassRepository : GenericRepository<Class>, IClassRepository
    {
        private readonly SQLDBContext _sqlContext;

        public ClassRepository(SQLDBContext context) : base(context)
        {
            _sqlContext = context;
        }

        public async Task<bool> IsStudentInClassAsync(Guid classId, Guid studentId)
        {
            return await _sqlContext.Set<Dictionary<string, object>>("ClassStudents")
                .AnyAsync(cs => (Guid)cs["ClassId"] == classId && (Guid)cs["StudentId"] == studentId);
        }

        public async Task<IEnumerable<Guid>> GetStudentIdsByClassAsync(Guid classId)
        {
            return await _sqlContext.Set<Dictionary<string, object>>("ClassStudents")
                .Where(cs => (Guid)cs["ClassId"] == classId)
                .Select(cs => (Guid)cs["StudentId"])
                .ToListAsync();
        }

        public async Task AssignStudentsToClassAsync(Guid classId, IEnumerable<Guid> studentIds)
        {
            var dbSet = _sqlContext.Set<Dictionary<string, object>>("ClassStudents");
            
            var existing = await dbSet.Where(cs => (Guid)cs["ClassId"] == classId).ToListAsync();
            dbSet.RemoveRange(existing);

            var newEntries = studentIds.Select(studentId => new Dictionary<string, object>
            {
                { "ClassId", classId },
                { "StudentId", studentId }
            });

            await dbSet.AddRangeAsync(newEntries);
        }
    }
}
