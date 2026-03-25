using dtc.Domain.Entities.Classes;

namespace dtc.Domain.Interfaces.Classes
{
    public interface IClassRepository : IGenericRepository<Class>
    {
        Task<bool> IsStudentInClassAsync(Guid classId, Guid studentId);
        Task<IEnumerable<Guid>> GetStudentIdsByClassAsync(Guid classId);
        Task AssignStudentsToClassAsync(Guid classId, IEnumerable<Guid> studentIds);
    }
}
