using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniHotel.Application.DTOs
{
    public class RoomTypeUpsertDto
    {
        [Required]
        public required string RoomCategory { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PricePerNight { get; set; }

        [Required, StringLength(500)]
        public string Description { get; set; } = default!;

        public string? ImageUrl { get; set; }
    }
}
