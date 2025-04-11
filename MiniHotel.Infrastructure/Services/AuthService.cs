using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Interfaces.IService;
using MiniHotel.Domain.Entities;
using MiniHotel.Domain.Enums;
using MiniHotel.Infrastructure.Data;
using MiniHotel.Infrastructure.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MiniHotel.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly MiniHotelDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager,
                           SignInManager<ApplicationUser> signInManager,
                           MiniHotelDbContext context,
                           IMapper mapper,
                           IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<AuthenticationResultDto> RegisterAsync(RegisterRequestDto request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser is not null)
            {
                return new AuthenticationResultDto
                {
                    Success = false,
                    Errors = new List<string> { "Користувач з таким email вже існує." }
                };
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var applicationUser = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email,
                PhoneNumber = request.PhoneNumber,
                FirstName = request.FirstName,
                LastName = request.LastName,
            };

            var createIdentityResult = await _userManager.CreateAsync(applicationUser, request.Password);

            if (!createIdentityResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return new AuthenticationResultDto
                {
                    Success = false,
                    Errors = createIdentityResult.Errors.Select(x => x.Description).ToList()
                };
            }

            if (!string.IsNullOrEmpty(request.Role) && Enum.TryParse<UserRole>(request.Role, out var result))
            {
                await _userManager.AddToRoleAsync(applicationUser, result.ToString());
            }

            var domainUser = _mapper.Map<HotelUser>(request);
            domainUser.UserId = applicationUser.Id;

            try
            {
                _context.HotelUsers.Add(domainUser);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                var token = await GenerateJwtToken(applicationUser);
                return new AuthenticationResultDto { Success = true, Token = token };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                await _userManager.DeleteAsync(applicationUser);
                return new AuthenticationResultDto
                {
                    Success = false,
                    Errors = ["не вдалося створити користувача. Спробуйте ще раз.", ex.Message]
                };
            }
        }

        public async Task<AuthenticationResultDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return new AuthenticationResultDto
                {
                    Success = false,
                    Errors = new List<string> { "Користувача з таким email не знайдено." }
                };
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!signInResult.Succeeded)
            {
                return new AuthenticationResultDto
                {
                    Success = false,
                    Errors = new List<string> { "Невірний пароль або email" }
                };
            }

            var token = await GenerateJwtToken(user);
            return new AuthenticationResultDto
            {
                Success = true,
                Token = token
            };
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null when generating JWT token.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
