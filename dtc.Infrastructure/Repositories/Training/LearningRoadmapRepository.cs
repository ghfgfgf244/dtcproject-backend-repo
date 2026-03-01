using dtc.Domain.Entities.Training;
using dtc.Domain.Interfaces.Training;
using dtc.Infrastructure.Persistence.MongoDB;

namespace dtc.Infrastructure.Repositories.Training
{
    public class LearningRoadmapRepository : MongoGenericRepository<LearningRoadmap>, ILearningRoadmapRepository
    {
        public LearningRoadmapRepository(MongoDBContext context) : base(context, "LearningRoadmaps")
        {
        }
    }
}
