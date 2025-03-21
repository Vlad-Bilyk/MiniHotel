using MiniHotel.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MiniHotel.Domain.Entities
{
    public class Room
    {
        [Key]
        public int RoomId { get; set; }

        [Required]
        [StringLength(4)]
        public string RoomNumber { get; set; } = default!;

        [Required]
        public RoomType RoomType { get; set; }

        [Range(0, 999999.99)]
        public decimal PricePerDay { get; set; }

        [Required]
        public RoomStatus RoomStatus { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
