using dtc.Domain.Entities.Exams;
using dtc.Domain.Interfaces.Exams;
using dtc.Infrastructure.Pesistence.SQLServer;

namespace dtc.Infrastructure.Repositories.Exams
{
    public class ExamResultRepository : GenericRepository<ExamResult>, IExamResultRepository
    {
        public ExamResultRepository(SQLDBContext context) : base(context)
        {
        }
    }
}
