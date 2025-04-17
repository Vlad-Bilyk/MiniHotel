using System.ComponentModel.DataAnnotations;

namespace MiniHotel.Application.DTOs
{
    public class InvoiceItemCreateDto
    {
        [Required]
        public string? ServiceName { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
