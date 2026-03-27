using dtc.Domain.Entities.Location;
using dtc.Domain.Interfaces.Location;
using dtc.Infrastructure.Pesistence.SQLServer;
using Microsoft.EntityFrameworkCore;

namespace dtc.Infrastructure.Repositories.Location
{
    public class CenterRepository : GenericRepository<Center>, ICenterRepository
    {
        private readonly SQLDBContext _sqlContext;

        public CenterRepository(SQLDBContext context) : base(context)
        {
            _sqlContext = context;
        }

        public async Task<IEnumerable<Guid>> GetUserIdsByCenterAsync(Guid centerId)
        {
            return await _sqlContext.Set<Dictionary<string, object>>("UserCenters")
                .Where(uc => (Guid)uc["CenterId"] == centerId)
                .Select(uc => (Guid)uc["UserId"])
                .ToListAsync();
        }

        public async Task AssignUsersToCenterAsync(Guid centerId, IEnumerable<Guid> userIds)
        {
            var dbSet = _sqlContext.Set<Dictionary<string, object>>("UserCenters");
            
            var existing = await dbSet.Where(uc => (Guid)uc["CenterId"] == centerId).ToListAsync();
            dbSet.RemoveRange(existing);

            var newEntries = userIds.Distinct().Select(userId => new Dictionary<string, object>
            {
                { "CenterId", centerId },
                { "UserId", userId }
            });

            await dbSet.AddRangeAsync(newEntries);
        }
    }
}
