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

        [Required]
        public RoomStatus RoomStatus { get; set; } = default!;
    }
}
