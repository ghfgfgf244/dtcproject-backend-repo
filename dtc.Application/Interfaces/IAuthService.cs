using dtc.Application.DTOs.Auth;
using System.Threading.Tasks;

namespace dtc.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    }
}
