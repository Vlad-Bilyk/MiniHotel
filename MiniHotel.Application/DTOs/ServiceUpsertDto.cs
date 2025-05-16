using System.ComponentModel.DataAnnotations;

namespace MiniHotel.Application.DTOs
{
    public class ServiceUpsertDto
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(0, 999999.99)]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; } = false;
    }
}
