using MiniHotel.Application.DTOs;

namespace MiniHotel.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthenticationResultDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthenticationResultDto> LoginAsync(LoginRequestDto request);
    }
}
