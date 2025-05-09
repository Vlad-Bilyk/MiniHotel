﻿using MiniHotel.Domain.Enums;
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

        [Required, StringLength(500)]
        public required string Description { get; set; }

        public string? ImageUrl { get; set; }

        [NotMapped]
        public int RoomCount => Rooms.Count(r => r.RoomStatus != RoomStatus.UnderMaintenance);

        public ICollection<Room> Rooms { get; set; } = default!;
    }
}
