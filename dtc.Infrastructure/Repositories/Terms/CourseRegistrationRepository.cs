using dtc.Domain.Entities.Terms;
using dtc.Domain.Interfaces.Terms;
using dtc.Infrastructure.Pesistence.SQLServer;

namespace dtc.Infrastructure.Repositories.Terms
{
    public class CourseRegistrationRepository : GenericRepository<CourseRegistration>, ICourseRegistrationRepository
    {
        public CourseRegistrationRepository(SQLDBContext context) : base(context)
        {
        }
    }
}
