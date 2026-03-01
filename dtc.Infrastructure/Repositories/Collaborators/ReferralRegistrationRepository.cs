using dtc.Domain.Entities.Collaborators;
using dtc.Domain.Interfaces.Collaborators;
using dtc.Infrastructure.Pesistence.SQLServer;

namespace dtc.Infrastructure.Repositories.Collaborators
{
    public class ReferralRegistrationRepository : GenericRepository<ReferralRegistration>, IReferralRegistrationRepository
    {
        public ReferralRegistrationRepository(SQLDBContext context) : base(context)
        {
        }
    }
}
