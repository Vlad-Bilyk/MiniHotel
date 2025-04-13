using MiniHotel.Domain.Enums;

namespace MiniHotel.Application.DTOs
{
    public class UserBookingsDto
    {
        public int BookingId { get; set; }
        public string RoomNumber { get; set; } = default!;
        public RoomCategory RoomCategory { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public decimal Amount { get; set; }
    }
}
