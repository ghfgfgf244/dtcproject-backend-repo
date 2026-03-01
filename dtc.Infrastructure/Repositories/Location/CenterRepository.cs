using dtc.Domain.Entities.Permissions;
using dtc.Domain.Interfaces.Location;
using dtc.Infrastructure.Pesistence.SQLServer;

namespace dtc.Infrastructure.Repositories.Location
{
    public class CenterRepository : GenericRepository<Center>, ICenterRepository
    {
        public CenterRepository(SQLDBContext context) : base(context)
        {
        }
    }
}
