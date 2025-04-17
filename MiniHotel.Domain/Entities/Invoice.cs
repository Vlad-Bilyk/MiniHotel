using MiniHotel.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniHotel.Domain.Entities
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        public int BookingId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PaidAmount { get; set; }

        [Required]
        public InvoiceStatus Status { get; set; }

        [NotMapped]
        public decimal TotalAmount => InvoiceItems.Sum(i => i.Quantity * i.UnitPrice);

        [NotMapped]
        public decimal AmountDue => TotalAmount - PaidAmount;


        [ForeignKey(nameof(BookingId))]
        public Booking Booking { get; set; } = null!;
        public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
