using dtc.Domain.Entities.Classes;
using dtc.Domain.Interfaces.Classes;
using dtc.Infrastructure.Pesistence.SQLServer;

namespace dtc.Infrastructure.Repositories.Classes
{
    public class ClassStudentRepository : GenericRepository<ClassStudent>, IClassStudentRepository
    {
        public ClassStudentRepository(SQLDBContext context)
            : base(context)
        {
        }
    }
}
