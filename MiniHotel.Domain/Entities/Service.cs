using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniHotel.Domain.Entities
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }

        [StringLength(100), Required]
        public required string Name { get; set; }

        [StringLength(500), Required]
        public required string Description { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Required]
        public bool IsAvailable { get; set; } = false;

        public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    }
}
