using dtc.Domain.Entities.Classes;
using dtc.Domain.Interfaces.Classes;
using dtc.Infrastructure.Persistence.MongoDB;

namespace dtc.Infrastructure.Repositories.Classes
{
    public class LearningLocationRepository : MongoGenericRepository<LearningLocation>, ILearningLocationRepository
    {
        public LearningLocationRepository(MongoDBContext context)
            : base(context, "LearningLocations")
        {
        }
    }
}
