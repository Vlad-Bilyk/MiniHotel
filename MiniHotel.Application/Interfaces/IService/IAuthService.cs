using MiniHotel.Application.DTOs;

namespace MiniHotel.Application.Interfaces.IService
{
    public interface IAuthService
    {
        Task<AuthenticationResultDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthenticationResultDto> LoginAsync(LoginRequestDto request);
    }
}
