using MiniHotel.Domain.Enums;

namespace MiniHotel.Application.DTOs
{
    public class BookingUpdateDto
    {
        public int BookingId { get; set; }
        public string UserId { get; set; } = default!;
        public int RoomId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public BookingStatus BookingStatus { get; set; }
    }
}
