using dtc.Domain.Entities.Training;
using dtc.Domain.Interfaces.Training;
using dtc.Infrastructure.Pesistence.SQLServer;
using dtc.Infrastructure.Repositories;

namespace dtc.Infrastructure.Persistence.Repositories.Training
{
    public class StudentEvaluationRepository : GenericRepository<StudentEvaluation>, IStudentEvaluationRepository
    {
        public StudentEvaluationRepository(SQLDBContext context) : base(context)
        {
        }
    }
}
