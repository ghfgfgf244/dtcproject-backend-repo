using dtc.Domain.Entities.Exams;
using dtc.Domain.Interfaces.Exams;
using dtc.Infrastructure.Pesistence.SQLServer;

namespace dtc.Infrastructure.Repositories.Exams
{
    public class ExamRegistrationRepository : GenericRepository<ExamRegistration>, IExamRegistrationRepository
    {
        public ExamRegistrationRepository(SQLDBContext context) : base(context)
        {
        }
    }
}
