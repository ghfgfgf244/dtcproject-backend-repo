using dtc.Domain.Entities.Classes;
using dtc.Domain.Interfaces.Classes;
using dtc.Infrastructure.Pesistence.SQLServer;

namespace dtc.Infrastructure.Repositories.Classes
{
    public class InstructorLeaveRequestRepository : GenericRepository<InstructorLeaveRequest>, IInstructorLeaveRequestRepository
    {
        public InstructorLeaveRequestRepository(SQLDBContext context) : base(context)
        {
        }
    }
}
