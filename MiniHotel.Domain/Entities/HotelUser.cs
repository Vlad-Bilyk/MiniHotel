using MiniHotel.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MiniHotel.Domain.Entities
{
    public class HotelUser
    {
        [Key]
        public required string UserId { get; set; }

        [StringLength(50), Required]
        public required string FirstName { get; set; }

        [StringLength(50), Required]
        public required string LastName { get; set; }

        [EmailAddress, Required]
        public required string Email { get; set; }

        [Phone, Required]
        public required string PhoneNumber { get; set; }

        [Required]
        public UserRole Role { get; set; } = UserRole.Customer;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
