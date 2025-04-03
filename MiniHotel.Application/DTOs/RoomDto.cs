using MiniHotel.Domain.Enums;

namespace MiniHotel.Application.DTOs
{
    public class RoomDto
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = default!;
        public RoomCategory RoomType { get; set; }
        public decimal PricePerDay { get; set; }
        public RoomStatus RoomStatus { get; set; }
    }
}
