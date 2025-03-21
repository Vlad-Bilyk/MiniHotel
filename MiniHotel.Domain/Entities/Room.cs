using MiniHotel.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniHotel.Domain.Entities
{
    public class Room
    {
        [Key]
        public int RoomId { get; set; }

        [StringLength(4), Required]
        public required string RoomNumber { get; set; }

        [Required]
        public RoomType RoomType { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PricePerNight { get; set; }

        [Required]
        public RoomStatus RoomStatus { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
