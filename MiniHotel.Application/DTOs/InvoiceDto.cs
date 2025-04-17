using MiniHotel.Domain.Enums;

namespace MiniHotel.Application.DTOs
{
    public class InvoiceDto
    {
        public int InvoiceId { get; set; }
        public int BookingId { get; set; }
        public DateTime CreatedAt { get; set; }
        public InvoiceStatus Status { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public IEnumerable<InvoiceItemDto> InvoiceItems { get; set; } = Enumerable.Empty<InvoiceItemDto>();
        public IEnumerable<PaymentDto> Payments { get; set; } = Enumerable.Empty<PaymentDto>();
    }
}
