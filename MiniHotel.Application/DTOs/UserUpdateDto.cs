using MiniHotel.Domain.Enums;

namespace MiniHotel.Application.DTOs
{
    public class UserUpdateDto
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public UserRole Role { get; set; }
    }
}
