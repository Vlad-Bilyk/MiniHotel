using Microsoft.EntityFrameworkCore;
using MiniHotel.Application.Interfaces;
using MiniHotel.Domain.Entities;

namespace MiniHotel.Infrastructure.Data.Seed
{
    public class RoomTypeSeeder : ISeeder
    {
        private readonly MiniHotelDbContext _db;

        public RoomTypeSeeder(MiniHotelDbContext db)
        {
            _db = db;
        }

        public async Task SeedAsync()
        {
            if (await _db.RoomTypes.AnyAsync()) return;

            var types = new[]
            {
                new RoomType { RoomCategory = "Single", PricePerNight = 500m, Description = "Одномісний номер із базовими зручностями" },
                new RoomType { RoomCategory = "Double", PricePerNight = 750m, Description = "Двомісний номер із двоспальним ліжком" },
                new RoomType { RoomCategory = "Family", PricePerNight = 1000m, Description = "Сімейний номер для 3–4 гостей" },
                new RoomType { RoomCategory = "Suite", PricePerNight = 1500m, Description = "Розкішний номер з окремою вітальнею" }
            };

            _db.RoomTypes.AddRange(types);
            await _db.SaveChangesAsync();
        }
    }
}
