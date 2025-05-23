﻿namespace MiniHotel.Application.DTOs
{
    public class RoomTypeDto
    {
        public int RoomTypeId { get; set; }
        public string RoomCategory { get; set; } = default!;
        public decimal PricePerNight { get; set; }
        public string Description { get; set; } = default!;
        public string? ImageUrl { get; set; } = default!;
        public int RoomCount { get; set; }

    }
}
