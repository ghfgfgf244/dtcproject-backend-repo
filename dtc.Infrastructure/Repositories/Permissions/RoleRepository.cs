using dtc.Domain.Entities.Permissions;
using dtc.Domain.Interfaces.Permissions;
using dtc.Infrastructure.Pesistence.SQLServer;

namespace dtc.Infrastructure.Repositories.Permissions
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(SQLDBContext context) : base(context)
        {
        }
    }
}
