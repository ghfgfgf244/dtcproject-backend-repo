using dtc.Domain.Entities.Exams;
using dtc.Domain.Interfaces.Exams;
using dtc.Infrastructure.Pesistence.SQLServer;
using dtc.Infrastructure.Repositories;

namespace dtc.Infrastructure.Persistence.Repositories.Exams
{
    public class SampleExamResultRepository : GenericRepository<SampleExamResult>, ISampleExamResultRepository
    {
        public SampleExamResultRepository(SQLDBContext context) : base(context)
        {
        }
    }
}
