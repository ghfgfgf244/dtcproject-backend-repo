using dtc.Domain.Entities.Exams;
using dtc.Domain.Interfaces.Exams;
using dtc.Infrastructure.Pesistence.SQLServer;

namespace dtc.Infrastructure.Repositories.Exams
{
    public class ExamRepository : GenericRepository<Exam>, IExamRepository
    {
        public ExamRepository(SQLDBContext context) : base(context)
        {
        }
    }
}
