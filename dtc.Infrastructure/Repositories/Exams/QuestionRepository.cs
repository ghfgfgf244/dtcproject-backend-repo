using dtc.Domain.Entities.Exams;
using dtc.Domain.Interfaces.Exams;
using dtc.Infrastructure.Persistence.MongoDB;

namespace dtc.Infrastructure.Repositories.Exams
{
    public class QuestionRepository : MongoGenericRepository<Question>, IQuestionRepository
    {
        public QuestionRepository(MongoDBContext context) : base(context, "Questions")
        {
        }
    }
}
