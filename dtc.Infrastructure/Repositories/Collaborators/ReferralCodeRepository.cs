using dtc.Domain.Entities.Collaborators;
using dtc.Domain.Interfaces.Collaborators;
using dtc.Infrastructure.Pesistence.SQLServer;

namespace dtc.Infrastructure.Repositories.Collaborators
{
    public class ReferralCodeRepository : GenericRepository<ReferralCode>, IReferralCodeRepository
    {
        public ReferralCodeRepository(SQLDBContext context) : base(context)
        {
        }
    }
}
