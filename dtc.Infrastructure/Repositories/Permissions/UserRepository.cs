using dtc.Domain.Entities.Permissions;
using dtc.Domain.Interfaces.Permissions;
using dtc.Infrastructure.Pesistence.SQLServer;

namespace dtc.Infrastructure.Repositories.Permissions
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(SQLDBContext context) : base(context)
        {
        }
    }
}
