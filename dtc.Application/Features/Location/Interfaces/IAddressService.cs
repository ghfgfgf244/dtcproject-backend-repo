using dtc.Application.Features.Location.Interfaces;
using dtc.Application.Features.Location.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace dtc.Application.Features.Location.Interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<AddressResponseDto>> GetAllAsync();
    }
}
