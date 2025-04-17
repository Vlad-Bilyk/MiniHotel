using MiniHotel.Domain.Enums;

namespace MiniHotel.Application.DTOs
{
    public class BookingDto
    {
        public int BookingId { get; set; }
        public int InvoiceId { get; set; }
        public string FullName { get; set; } = default!;
        public string RoomNumber { get; set; } = default!;
        public string RoomCategory { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public BookingStatus BookingStatus { get; set; }
    }
}
