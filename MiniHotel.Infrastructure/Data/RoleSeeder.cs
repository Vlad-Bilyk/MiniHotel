using Microsoft.AspNetCore.Identity;
using MiniHotel.Domain.Enums;

namespace MiniHotel.Infrastructure.Data
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = Enum.GetNames(typeof(UserRole));

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
