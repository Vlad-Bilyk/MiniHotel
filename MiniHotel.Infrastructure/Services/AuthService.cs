using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Interfaces.IService;
using MiniHotel.Domain.Entities;
using MiniHotel.Domain.Enums;
using MiniHotel.Infrastructure.Data;
using MiniHotel.Infrastructure.Identity;

namespace MiniHotel.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly MiniHotelDbContext _context;
        private readonly IMapper _mapper;

        public AuthService(UserManager<ApplicationUser> userManager,
                           SignInManager<ApplicationUser> signInManager,
                           MiniHotelDbContext context,
                           IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _mapper = mapper;
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

            var domainUser = _mapper.Map<User>(request);
            domainUser.UserId = applicationUser.Id;

            try
            {
                _context.Users.Add(domainUser); // TODO: change to repository
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new AuthenticationResultDto { Success = true };
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

            return new AuthenticationResultDto
            {
                Success = true,
                Token = null // TODO: implement JWT token generation
            };
        }
    }
}
