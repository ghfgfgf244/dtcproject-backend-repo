using dtc.Domain.Entities.Blogs;
using dtc.Domain.Interfaces.Blogs;
using dtc.Infrastructure.Persistence.MongoDB;

namespace dtc.Infrastructure.Repositories.Blogs
{
    public class BlogRepository : MongoGenericRepository<Blog>, IBlogRepository
    {
        public BlogRepository(MongoDBContext context) : base(context, "Blogs")
        {
        }
    }
}
