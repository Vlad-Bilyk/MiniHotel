using MiniHotel.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MiniHotel.Application.DTOs
{
    public class RoomUpsertDto
    {
        [Required]
        [StringLength(4)]
        public string RoomNumber { get; set; } = default!;

        [Required]
        public RoomCategory RoomType { get; set; } = default!;

        [Range(0, 999999.99)]
        public decimal PricePerDay { get; set; }

        [Required]
        public RoomStatus RoomStatus { get; set; } = default!;
    }
}
