using Microsoft.EntityFrameworkCore;
using MiniHotel.Application.Interfaces;
using MiniHotel.Domain.Entities;
using MiniHotel.Domain.Enums;

namespace MiniHotel.Infrastructure.Data.Seed
{
    public class InvoiceSeeder : ISeeder
    {
        private readonly MiniHotelDbContext _db;
        public InvoiceSeeder(MiniHotelDbContext db)
        {
            _db = db;
        }
        public async Task SeedAsync()
        {
            if (await _db.Invoices.AnyAsync()) return;

            var roomService = await _db.Services.SingleAsync(s => s.Name == "Бронювання");
            var bookings = await _db.Bookings
                .Include(b => b.Room).ThenInclude(r => r.RoomType)
                .ToListAsync();

            var invoices = bookings.Select(b => new Invoice
            {
                BookingId = b.BookingId,
                CreatedAt = DateTime.UtcNow,
                Status = InvoiceStatus.Pending,
                InvoiceItems = new List<InvoiceItem>
                {
                    new InvoiceItem
                    {
                        ServiceId = roomService.ServiceId,
                        Description = $"Бронювання номеру {b.Room.RoomNumber} - { (b.EndDate - b.StartDate).Days } ночей",
                        Quantity = (b.EndDate - b.StartDate).Days,
                        UnitPrice = b.Room.RoomType.PricePerNight,
                    }
                }
            });

            _db.Invoices.AddRange(invoices);
            await _db.SaveChangesAsync();
        }
    }
}
