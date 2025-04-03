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

        public int RoomTypeId { get; set; }

        [Required]
        public RoomStatus RoomStatus { get; set; }

        [ForeignKey(nameof(RoomTypeId))]
        public RoomType RoomType { get; set; } = null!;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
