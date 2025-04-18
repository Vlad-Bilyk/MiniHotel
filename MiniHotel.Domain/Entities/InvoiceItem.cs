using MiniHotel.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniHotel.Domain.Entities
{
    public class InvoiceItem
    {
        [Key]
        public int InvoiceItemId { get; set; }

        public int InvoiceId { get; set; }

        public int? ServiceId { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        [Range(1, 1000)]
        public int Quantity { get; set; }

        public InvoiceItemType ItemType { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(ServiceId))]
        public Service? Service { get; set; }

        [ForeignKey(nameof(InvoiceId))]
        public Invoice Invoice { get; set; } = null!;
    }
}
