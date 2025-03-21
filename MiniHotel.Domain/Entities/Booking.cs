using MiniHotel.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniHotel.Domain.Entities
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        public string UserId { get; set; } = default!;

        [ForeignKey("UserId")]
        public User User { get; set; } = default!;

        public int RoomId { get; set; }

        [ForeignKey("RoomId")]
        public Room Room { get; set; } = default!;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public BookingStatus BookingStatus { get; set; }

        [Required]
        public bool IsFullPaid { get; set; }

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

        public ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();
    }
}
