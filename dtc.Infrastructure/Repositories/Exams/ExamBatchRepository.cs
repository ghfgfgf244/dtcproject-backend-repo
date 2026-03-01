using dtc.Domain.Entities.Exams;
using dtc.Domain.Interfaces.Exams;
using dtc.Infrastructure.Pesistence.SQLServer;

namespace dtc.Infrastructure.Repositories.Exams
{
    public class ExamBatchRepository : GenericRepository<ExamBatch>, IExamBatchRepository
    {
        public ExamBatchRepository(SQLDBContext context) : base(context)
        {
        }
    }
}
