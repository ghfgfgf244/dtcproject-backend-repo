using dtc.Domain.Interfaces;
using dtc.Infrastructure.Pesistence.SQLServer;

namespace dtc.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SQLDBContext _context;

        public UnitOfWork(SQLDBContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
