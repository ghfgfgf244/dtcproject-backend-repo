using dtc.Domain.Entities.Collaborators;
using dtc.Domain.Interfaces.Collaborators;
using dtc.Infrastructure.Pesistence.SQLServer;

namespace dtc.Infrastructure.Repositories.Collaborators
{
    public class CollaboratorCommissionRepository : GenericRepository<CollaboratorCommission>, ICollaboratorCommissionRepository
    {
        public CollaboratorCommissionRepository(SQLDBContext context) : base(context)
        {
        }
    }
}
