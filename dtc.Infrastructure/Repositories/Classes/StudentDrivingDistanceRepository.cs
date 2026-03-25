using dtc.Domain.Entities.Classes;
using dtc.Domain.Interfaces.Classes;
using dtc.Infrastructure.Pesistence.SQLServer;

namespace dtc.Infrastructure.Repositories.Classes
{
    public class StudentDrivingDistanceRepository : GenericRepository<StudentDrivingDistance>, IStudentDrivingDistanceRepository
    {
        public StudentDrivingDistanceRepository(SQLDBContext context) : base(context)
        {
        }
    }
}
