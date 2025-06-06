﻿using MiniHotel.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MiniHotel.Application.DTOs
{
    public class BookingCreateDto
    {
        [Required]
        [StringLength(4)]
        public required string RoomNumber { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }
    }
}
