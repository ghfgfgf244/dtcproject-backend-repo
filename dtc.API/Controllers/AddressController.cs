using dtc.Application.Features.Location.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    public class AddressController : BaseApiController
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager,Instructor")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _addressService.GetAllAsync();
            return Ok(response);
        }
    }
}
