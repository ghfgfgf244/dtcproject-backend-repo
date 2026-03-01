using dtc.Domain.Entities.Blogs;
using dtc.Domain.Interfaces.Blogs;
using dtc.Infrastructure.Persistence.MongoDB;

namespace dtc.Infrastructure.Repositories.Blogs
{
    public class CategoryRepository : MongoGenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(MongoDBContext context) : base(context, "Categories")
        {
        }
    }
}
