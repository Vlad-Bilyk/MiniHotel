using MiniHotel.Domain.Enums;

namespace MiniHotel.Application.DTOs
{
    public class BookingDto
    {
        public int BookingId { get; set; }
        public string UserId { get; set; } = default!;
        public string RoomNumber { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public decimal FinalInvoiceAmount { get; set; }
        public bool IsFullPaid { get; set; }
    }
}
