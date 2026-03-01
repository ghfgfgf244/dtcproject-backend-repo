using dtc.Domain.Entities.Location;
using dtc.Domain.Interfaces.Location;
using dtc.Infrastructure.Persistence.MongoDB;

namespace dtc.Infrastructure.Repositories.Location
{
    public class AddressRepository : MongoGenericRepository<Address>, IAddressRepository
    {
        public AddressRepository(MongoDBContext context) : base(context, "Addresses")
        {
        }
    }
}
