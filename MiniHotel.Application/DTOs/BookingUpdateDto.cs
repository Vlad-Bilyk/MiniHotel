﻿namespace MiniHotel.Application.DTOs
{
    public class BookingUpdateDto
    {
        public int RoomId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
