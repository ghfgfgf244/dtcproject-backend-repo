using dtc.Application.Features.Location.DTOs;
using dtc.Application.Features.Location.Interfaces;
using dtc.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.Application.Features.Location.Services
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddressService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<AddressResponseDto>> GetAllAsync()
        {
            var addresses = await _unitOfWork.Addresses.GetAllAsync();
            return addresses
                .OrderBy(a => a.AddressName)
                .Select(a => new AddressResponseDto
                {
                    Id = a.Id,
                    AddressName = a.AddressName
                });
        }
    }
}
