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

            var customers = await _db.HotelUsers
                .Where(u => u.Role == UserRole.Customer && u.Email != "offline_client@hotel.local")
                .ToListAsync();

            var rooms = await _db.Rooms
                .Include(r => r.RoomType)
                .ToListAsync();

            var baseDate = DateTime.UtcNow.Date.AddDays(-14);
            var bookings = new List<Booking>();

            foreach (var customer in customers)
            {
                for (int i = 0; i < 4; i++)
                {
                    var raw = customer.UserId.GetHashCode() + i;
                    var index = Math.Abs(raw) % rooms.Count;

                    var room = rooms[index];
                    bookings.Add(new Booking
                    {
                        UserId = customer.UserId,
                        RoomId = room.RoomId,
                        Room = room,
                        StartDate = baseDate.AddDays(i * 3),
                        EndDate = baseDate.AddDays(i * 3 + 2),
                        BookingStatus = BookingStatus.Confirmed,
                        PaymentMethod = PaymentMethod.OnSite,
                    });
                }
            }

            _db.Bookings.AddRange(bookings);
            await _db.SaveChangesAsync();
        }
    }
}
