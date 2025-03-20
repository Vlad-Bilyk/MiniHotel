using System.ComponentModel.DataAnnotations;

namespace MiniHotel.Application.DTOs
{
    public class ServiceUpsertDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = default!;

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(0, 999999.99)]
        public decimal Price { get; set; }

        [Required]
        public bool IsAvailable { get; set; } = false;
    }
}
