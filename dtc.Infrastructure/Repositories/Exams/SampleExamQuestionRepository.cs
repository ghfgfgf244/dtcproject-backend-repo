using dtc.Domain.Entities.Exams;
using dtc.Domain.Interfaces.Exams;
using dtc.Infrastructure.Persistence.MongoDB;

namespace dtc.Infrastructure.Repositories.Exams
{
    public class SampleExamQuestionRepository : MongoGenericRepository<SampleExamQuestion>, ISampleExamQuestionRepository
    {
        public SampleExamQuestionRepository(MongoDBContext context) : base(context, "SampleExamQuestions")
        {
        }
    }
}
