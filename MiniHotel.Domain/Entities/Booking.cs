using MiniHotel.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniHotel.Domain.Entities
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        public required string UserId { get; set; }

        public int RoomId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [StringLength(100)]
        public string? FullName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.OnSite;

        [Required]
        public BookingStatus BookingStatus { get; set; }

        [ForeignKey(nameof(UserId))]
        public HotelUser? User { get; set; }

        [ForeignKey(nameof(RoomId))]
        public Room Room { get; set; } = null!;

        public Invoice Invoice { get; set; } = null!;
    }
}
