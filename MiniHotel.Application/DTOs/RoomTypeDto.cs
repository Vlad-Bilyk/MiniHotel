namespace MiniHotel.Application.DTOs
{
    public class RoomTypeDto
    {
        public int RoomTypeId { get; set; }
        public string RoomCategory { get; set; } = default!;
        public decimal PricePerNight { get; set; }
        public string? Description { get; set; }
    }
}
