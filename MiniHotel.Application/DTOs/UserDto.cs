using MiniHotel.Domain.Enums;

namespace MiniHotel.Application.DTOs
{
    public class UserDto
    {
        public string UserId { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public UserRole Role { get; set; }
    }
}
