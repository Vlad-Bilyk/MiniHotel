using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Interfaces;
using MiniHotel.Application.Interfaces.IService;
using MiniHotel.Domain.Enums;
using MiniHotel.Infrastructure.Identity;

namespace MiniHotel.Infrastructure.Data.Seed
{
    public class HotelUserSeeder : ISeeder
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MiniHotelDbContext _db;

        public HotelUserSeeder(IAuthService authService, MiniHotelDbContext db,
                               UserManager<ApplicationUser> userManager)
        {
            _authService = authService;
            _userManager = userManager;
            _db = db;
        }

        public async Task SeedAsync()
        {
            if (await _db.HotelUsers.AnyAsync()) return;

            var users = new[]
            {
                // client account for all offline bookings, dont delete
                new RegisterRequestDto { FirstName = "Offline", LastName = "Client", Email = "offline_client@hotel.local", PhoneNumber = "0000000000", Password = "Password1!", ConfirmPassword = "Password1!", Role = UserRole.Client },
                new RegisterRequestDto { FirstName = "John", LastName = "Doe", Email = "john.doe@gmail.com", PhoneNumber = "+380501234567", Password = "P@ssw0rd1!", ConfirmPassword = "P@ssw0rd1!", Role = UserRole.Client },
                new RegisterRequestDto { FirstName = "Jane", LastName = "Smith", Email = "jane.smith@gmail.com", PhoneNumber = "+380501234568", Password = "P@ssw0rd2!", ConfirmPassword = "P@ssw0rd2!", Role = UserRole.Client },
                new RegisterRequestDto { FirstName = "Manager", LastName = "User", Email = "manager@hotel.com", PhoneNumber = "+380501234569", Password = "P@ssw0rd3!", ConfirmPassword = "P@ssw0rd3!", Role = UserRole.Manager },
                new RegisterRequestDto { FirstName = "Receptionist", LastName = "User", Email = "receptionist@hotel.com", PhoneNumber = "+380501234570", Password = "P@ssw0rd4!", ConfirmPassword = "P@ssw0rd4!", Role = UserRole.Receptionist }
            };

            foreach (var user in users)
            {
                var result = await _authService.RegisterAsync(user);
                if (!result.Success)
                {
                    Console.WriteLine($"Seed user {user.Email} failed: {string.Join(", ", result.Errors)}");
                }

                // Block default offline user
                if (user.Email == "offline_client@hotel.local")
                {
                    var offlineIdentity = await _userManager.FindByEmailAsync(user.Email);
                    if (offlineIdentity != null)
                    {
                        await _userManager.SetLockoutEnabledAsync(offlineIdentity, true);
                        await _userManager.SetLockoutEndDateAsync(offlineIdentity, DateTimeOffset.MaxValue);
                    }
                }
            }
        }
    }
}
