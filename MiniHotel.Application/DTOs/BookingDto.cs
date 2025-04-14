using MiniHotel.Domain.Enums;

namespace MiniHotel.Application.DTOs
{
    public class BookingDto
    {
        public int BookingId { get; set; }
        public int InvoiceId { get; set; }
        public string UserId { get; set; } = default!;
        public string RoomNumber { get; set; } = default!;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public BookingStatus BookingStatus { get; set; }
    }
}
