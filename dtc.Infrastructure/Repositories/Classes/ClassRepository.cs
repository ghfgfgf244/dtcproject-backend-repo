using dtc.Domain.Entities.Classes;
using dtc.Domain.Interfaces.Classes;
using dtc.Infrastructure.Pesistence.SQLServer;

namespace dtc.Infrastructure.Repositories.Classes
{
    public class ClassRepository : GenericRepository<Class>, IClassRepository
    {
        public ClassRepository(SQLDBContext context) : base(context)
        {
        }
    }
}
