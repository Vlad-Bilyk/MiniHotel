using System.ComponentModel.DataAnnotations;

namespace MiniHotel.Application.DTOs
{
    public class InvoiceItemCreateDto
    {
        [Required]
        public required string ServiceName { get; set; }

        [StringLength(200)]
        public required string Description { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
