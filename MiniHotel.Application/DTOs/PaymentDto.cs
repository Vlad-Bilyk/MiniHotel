using MiniHotel.Domain.Enums;

namespace MiniHotel.Application.DTOs
{
    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public int InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public DateTime PaidAt { get; set; }
        public string? ExternalId { get; set; }
    }
}
