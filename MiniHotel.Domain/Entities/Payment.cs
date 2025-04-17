using MiniHotel.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniHotel.Domain.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int InvoiceId { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        public PaymentMethod Method { get; set; }
        public DateTime PaidAt { get; set; }
        public string? ExternalId { get; set; } // LiqPay transaction ID or other payment system

        [ForeignKey("InvoiceId")]
        public Invoice Invoice { get; set; } = null!;
    }
}
