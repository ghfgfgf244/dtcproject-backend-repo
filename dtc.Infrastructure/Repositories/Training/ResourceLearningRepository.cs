using dtc.Domain.Entities.Training;
using dtc.Domain.Interfaces.Training;
using dtc.Infrastructure.Persistence.MongoDB;

namespace dtc.Infrastructure.Repositories.Training
{
    public class ResourceLearningRepository : MongoGenericRepository<ResourceLearning>, IResourceLearningRepository
    {
        public ResourceLearningRepository(MongoDBContext context) : base(context, "ResourceLearnings")
        {
        }
    }
}
