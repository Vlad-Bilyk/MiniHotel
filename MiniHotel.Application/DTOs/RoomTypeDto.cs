using MiniHotel.Domain.Enums;

namespace MiniHotel.Application.DTOs
{
    public class RoomTypeDto
    {
        public int RoomTypeId { get; set; }
        public required RoomCategory Category { get; set; }
        public decimal PricePerNight { get; set; }
        public string? Description { get; set; }
    }
}
