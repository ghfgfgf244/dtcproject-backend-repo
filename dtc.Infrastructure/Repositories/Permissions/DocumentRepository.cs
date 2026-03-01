using dtc.Domain.Entities.Permissions;
using dtc.Domain.Interfaces.Permissions;
using dtc.Infrastructure.Pesistence.SQLServer;

namespace dtc.Infrastructure.Repositories.Permissions
{
    public class DocumentRepository : GenericRepository<Document>, IDocumentRepository
    {
        public DocumentRepository(SQLDBContext context) : base(context)
        {
        }
    }
}
