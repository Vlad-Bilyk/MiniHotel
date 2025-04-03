using MiniHotel.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniHotel.Domain.Entities
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        public string? UserId { get; set; }

        public int RoomId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public BookingStatus BookingStatus { get; set; }

        [ForeignKey(nameof(UserId))]
        public HotelUser? User { get; set; }

        [ForeignKey(nameof(RoomId))]
        public Room Room { get; set; } = null!;

        public Invoice? Invoice { get; set; }
    }
}
