using dtc.Domain.Entities.Permissions;
using dtc.Domain.Interfaces.Permissions;
using dtc.Infrastructure.Pesistence.SQLServer;

namespace dtc.Infrastructure.Repositories.Permissions
{
    public class UserCenterRepository : GenericRepository<UserCenter>, IUserCenterRepository
    {
        public UserCenterRepository(SQLDBContext context)
            : base(context)
        {
        }
    }
}
