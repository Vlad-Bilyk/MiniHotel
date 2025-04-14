using MiniHotel.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MiniHotel.Application.DTOs
{
    public class RoomUpsertDto
    {
        [Required]
        [StringLength(4)]
        public required string RoomNumber { get; set; }

        [Required]
        public required string RoomCategory { get; set; }

        [Required]
        public RoomStatus RoomStatus { get; set; } = default!;
    }
}
