using Microsoft.EntityFrameworkCore;
using MiniHotel.Application.Interfaces;
using MiniHotel.Domain.Entities;
using MiniHotel.Domain.Enums;

namespace MiniHotel.Infrastructure.Data.Seed
{
    public class BookingSeeder : ISeeder
    {
        private readonly MiniHotelDbContext _db;

        public BookingSeeder(MiniHotelDbContext db)
        {
            _db = db;
        }

        public async Task SeedAsync()
        {
            if (await _db.Bookings.AnyAsync()) return;

            var clients = await _db.HotelUsers
                .Where(u => u.Role == UserRole.Client && u.Email != "offline_client@hotel.local")
                .ToListAsync();

            var rooms = await _db.Rooms
                .Include(r => r.RoomType)
                .ToListAsync();

            var baseDate = DateTime.UtcNow.Date.AddDays(-14);

            var bookings = clients
                .SelectMany(client => Enumerable.Range(0, 4)
                    .Select(i =>
                    {
                        var raw = client.UserId.GetHashCode() + i;
                        var index = Math.Abs(raw) % rooms.Count;
                        var room = rooms[index];

                        return new Booking
                        {
                            UserId = client.UserId,
                            RoomId = room.RoomId,
                            Room = room,
                            StartDate = baseDate.AddDays(i * 3),
                            EndDate = baseDate.AddDays(i * 3 + 2),
                            BookingStatus = BookingStatus.Confirmed,
                            PaymentMethod = PaymentMethod.OnSite,
                        };
                    })
                )
                .ToList();

            _db.Bookings.AddRange(bookings);
            await _db.SaveChangesAsync();
        }
    }
}
