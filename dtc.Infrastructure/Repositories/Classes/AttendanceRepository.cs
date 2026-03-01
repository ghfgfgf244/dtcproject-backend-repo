using dtc.Domain.Entities.Classes;
using dtc.Domain.Interfaces.Classes;
using dtc.Infrastructure.Pesistence.SQLServer;

namespace dtc.Infrastructure.Repositories.Classes
{
    public class AttendanceRepository : GenericRepository<Attendance>, IAttendanceRepository
    {
        public AttendanceRepository(SQLDBContext context) : base(context)
        {
        }
    }
}
