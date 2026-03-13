using System.Threading.Tasks;
using dtc.Application.Features.Auth.Interfaces;
using dtc.Application.Features.Auth.DTOs;

namespace dtc.Application.Features.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    }
}
