using MiniHotel.Domain.Enums;

namespace MiniHotel.Application.DTOs
{
    public class RoomCreateDto
    {
        public string RoomNumber { get; set; } = default!;
        public RoomType RoomType { get; set; } = default!;
        public decimal PricePerDay { get; set; }
        public RoomStatus RoomStatus { get; set; } = default!;
    }
}
