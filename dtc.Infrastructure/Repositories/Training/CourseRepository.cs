using dtc.Domain.Entities.Training;
using dtc.Domain.Interfaces.Training;
using dtc.Infrastructure.Pesistence.SQLServer;

namespace dtc.Infrastructure.Repositories.Training
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(SQLDBContext context) : base(context)
        {
        }
    }
}
