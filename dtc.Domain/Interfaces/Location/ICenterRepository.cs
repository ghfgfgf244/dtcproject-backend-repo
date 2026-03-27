using dtc.Domain.Entities.Location;

namespace dtc.Domain.Interfaces.Location
{
    public interface ICenterRepository : IGenericRepository<Center>
    {
        Task<IEnumerable<Guid>> GetUserIdsByCenterAsync(Guid centerId);
        Task AssignUsersToCenterAsync(Guid centerId, IEnumerable<Guid> userIds);
    }
}
