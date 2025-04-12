using MiniHotel.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniHotel.Application.DTOs
{
    public class RoomTypeUpsertDto
    {
        [Required]
        public required RoomCategory Category { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PricePerNight { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
