using System.ComponentModel.DataAnnotations;

namespace MiniHotel.Domain.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = default!;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = default!;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = default!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [Phone]
        public string Phone { get; set; } = default!;

        [Required]
        public string Role { get; set; } = "customer";

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
