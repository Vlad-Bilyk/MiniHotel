﻿using Microsoft.EntityFrameworkCore;
using MiniHotel.Application.Interfaces;
using MiniHotel.Domain.Entities;
using MiniHotel.Domain.Enums;

namespace MiniHotel.Infrastructure.Data.Seed
{
    public class RoomSeeder : ISeeder
    {
        private readonly MiniHotelDbContext _db;

        public RoomSeeder(MiniHotelDbContext db)
        {
            _db = db;
        }

        public async Task SeedAsync()
        {
            if (await _db.Rooms.AnyAsync()) return;

            var single = await _db.RoomTypes.SingleAsync(rt => rt.RoomCategory == "Одномісний");
            var dbl = await _db.RoomTypes.SingleAsync(rt => rt.RoomCategory == "Двомісний");
            var family = await _db.RoomTypes.SingleAsync(rt => rt.RoomCategory == "Сімейний");
            var suite = await _db.RoomTypes.SingleAsync(rt => rt.RoomCategory == "Люкс");

            var rooms = new[]
            {
                new Room { RoomNumber = "101", RoomTypeId = single.RoomTypeId, RoomStatus = RoomStatus.Available },
                new Room { RoomNumber = "102", RoomTypeId = single.RoomTypeId, RoomStatus = RoomStatus.Available },
                new Room { RoomNumber = "103", RoomTypeId = dbl.RoomTypeId,    RoomStatus = RoomStatus.Available },
                new Room { RoomNumber = "104", RoomTypeId = dbl.RoomTypeId,    RoomStatus = RoomStatus.UnderMaintenance },
                new Room { RoomNumber = "105", RoomTypeId = dbl.RoomTypeId,    RoomStatus = RoomStatus.Available },
                new Room { RoomNumber = "201", RoomTypeId = family.RoomTypeId, RoomStatus = RoomStatus.Available },
                new Room { RoomNumber = "202", RoomTypeId = family.RoomTypeId, RoomStatus = RoomStatus.Available },
                new Room { RoomNumber = "203", RoomTypeId = single.RoomTypeId, RoomStatus = RoomStatus.Available },
                new Room { RoomNumber = "204", RoomTypeId = single.RoomTypeId, RoomStatus = RoomStatus.Available },
                new Room { RoomNumber = "301", RoomTypeId = suite.RoomTypeId,  RoomStatus = RoomStatus.Available },
                new Room { RoomNumber = "302", RoomTypeId = suite.RoomTypeId,  RoomStatus = RoomStatus.UnderMaintenance }
            };

            _db.Rooms.AddRange(rooms);
            await _db.SaveChangesAsync();
        }
    }
}
