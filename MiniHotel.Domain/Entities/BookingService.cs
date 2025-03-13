using MiniHotel.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MiniHotel.Domain.Entities
{
    public class BookingService
    {
        public int BookingId { get; set; }
        public Booking Booking { get; set; } = default!;
        public int ServiceId { get; set; }
        public Service Service { get; set; } = default!;

        [Range(1, 100)]
        public int Quantity { get; set; }

        [Required]
        public BookingServiceStatus Status { get; set; } = default!;
    }
}
