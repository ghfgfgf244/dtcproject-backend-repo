using dtc.Domain.Entities.Classes;
using dtc.Domain.Interfaces.Classes;
using dtc.Infrastructure.Pesistence.SQLServer;

namespace dtc.Infrastructure.Repositories.Classes
{
    public class ClassScheduleRepository : GenericRepository<ClassSchedule>, IClassScheduleRepository
    {
        public ClassScheduleRepository(SQLDBContext context) : base(context)
        {
        }
    }
}
