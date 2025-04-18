using MiniHotel.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MiniHotel.Application.DTOs
{
    public class RegisterRequestDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = default!;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = default!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = default!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Паролі не співпадають.")]
        public string ConfirmPassword { get; set; } = default!;

        public UserRole Role { get; set; } = UserRole.Client;

    }
}
