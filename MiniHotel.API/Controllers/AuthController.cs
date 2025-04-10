using Microsoft.AspNetCore.Mvc;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Interfaces.IService;
namespace MiniHotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<AuthenticationResultDto> Register([FromBody] RegisterRequestDto model)
        {
            return await _authService.RegisterAsync(model);
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(AuthenticationResultDto), StatusCodes.Status200OK)]
        public async Task<AuthenticationResultDto> Login([FromBody] LoginRequestDto model)
        {
            return await _authService.LoginAsync(model);
        }
    }
}
