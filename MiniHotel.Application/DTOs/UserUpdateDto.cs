using System.ComponentModel.DataAnnotations;

namespace MiniHotel.Application.DTOs
{
    public class UserUpdateDto
    {
        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required]
        public required string Email { get; set; }

        [Required]
        public required string PhoneNumber { get; set; }
    }
}
