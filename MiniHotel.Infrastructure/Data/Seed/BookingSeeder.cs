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
                .Where(u => u.Role == UserRole.Customer)
                .ToListAsync();

            var rooms = await _db.Rooms
                .Include(r => r.RoomType)
                .ToListAsync();
            var service = await _db.Services.FirstAsync();

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
                    });
                }
            }

            _db.Bookings.AddRange(bookings);
            await _db.SaveChangesAsync();

            var invoices = bookings.Select(b => new Invoice
            {
                BookingId = b.BookingId,
                CreatedAt = DateTime.UtcNow,
                Status = InvoiceStatus.Pending,
                InvoiceItems =
                {
                    // Room
                    new InvoiceItem
                    {
                        ServiceId = null,
                        Description = $"Room charge {b.Room.RoomNumber} - { (b.EndDate - b.StartDate).Days } nights",
                        Quantity = (b.EndDate - b.StartDate).Days,
                        UnitPrice = b.Room.RoomType.PricePerNight
                    },

                    // Service
                    new InvoiceItem
                    {
                        ServiceId = service.ServiceId,
                        Quantity = 1,
                        UnitPrice = service.Price
                    }
                }

            }).ToList();

            _db.Invoices.AddRange(invoices);
            await _db.SaveChangesAsync();
        }
    }
}
