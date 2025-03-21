using MiniHotel.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MiniHotel.Domain.Entities
{
    public class User
    {
        [Key]
        public string UserId { get; set; } = default!;

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

        public UserRole Role { get; set; } = UserRole.Customer;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
