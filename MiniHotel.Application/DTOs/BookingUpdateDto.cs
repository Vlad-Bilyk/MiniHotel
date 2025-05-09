﻿using System.ComponentModel.DataAnnotations;

namespace MiniHotel.Application.DTOs
{
    public class BookingUpdateDto
    {
        [Required]
        [StringLength(4)]
        public string RoomNumber { get; set; } = default!;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
