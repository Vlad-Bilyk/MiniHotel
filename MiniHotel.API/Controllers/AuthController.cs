using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Interfaces.IService;

namespace MiniHotel.API.Controllers
{
    /// <summary>
    /// Controller responsible for user authentication operations such as registration and login.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user based on provided registration details.
        /// </summary>
        /// <param name="registerDto">The registration request data containing user information.</param>
        /// <returns>An <see cref="AuthenticationResultDto"/> indicating the success status and errors if any.</returns>
        [HttpPost("register")]
        public async Task<AuthenticationResultDto> Register([FromBody] RegisterRequestDto registerDto)
        {
            return await _authService.RegisterAsync(registerDto);
        }

        /// <summary>
        /// Authenticates an existing user using email and password.
        /// </summary>
        /// <param name="loginDto">The login request data containing email and password.</param>
        /// <returns>
        /// An <see cref="AuthenticationResultDto"/> which includes a JWT token on successful authentication.
        /// </returns>
        /// <response code="200">Returns the authentication result with JWT token.</response>
        /// <response code="400">If the request data is invalid.</response>
        /// <response code="401">If authentication fails due to wrong credentials.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(AuthenticationResultDto), StatusCodes.Status200OK)]
        public async Task<AuthenticationResultDto> Login([FromBody] LoginRequestDto loginDto)
        {
            return await _authService.LoginAsync(loginDto);
        }
    }
}
