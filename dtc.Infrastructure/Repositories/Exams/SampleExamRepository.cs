using dtc.Domain.Entities.Exams;
using dtc.Domain.Interfaces.Exams;
using dtc.Infrastructure.Persistence.MongoDB;

namespace dtc.Infrastructure.Repositories.Exams
{
    public class SampleExamRepository : MongoGenericRepository<SampleExam>, ISampleExamRepository
    {
        public SampleExamRepository(MongoDBContext context) : base(context, "SampleExams")
        {
        }
    }
}
