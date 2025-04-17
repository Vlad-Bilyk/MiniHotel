using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniHotel.Domain.Entities
{
    public class RoomType
    {
        [Key]
        public int RoomTypeId { get; set; }

        [Required]
        public required string RoomCategory { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PricePerNight { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
