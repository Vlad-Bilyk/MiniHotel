using Microsoft.AspNetCore.Identity;
using MiniHotel.Application.Interfaces;
using MiniHotel.Domain.Enums;

namespace MiniHotel.Infrastructure.Data.Seed
{
    public class RoleSeeder : ISeeder
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleSeeder(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            var roles = Enum.GetNames(typeof(UserRole));

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
