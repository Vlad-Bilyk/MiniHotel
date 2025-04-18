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

            var bookings = await _db.Bookings
                .Include(b => b.Room).ThenInclude(r => r.RoomType)
                .Include(b => b.Invoice).ThenInclude(i => i.InvoiceItems)
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
                        ItemType = InvoiceItemType.RoomBooking,
                        Description = $"Бронювання номеру {b.Room.RoomNumber} - { (b.EndDate - b.StartDate).Days } ночей",
                        Quantity = (b.EndDate - b.StartDate).Days,
                        UnitPrice = b.Room.RoomType.PricePerNight,
                        CreatedAt = DateTime.UtcNow
                    }
                }
            });

            _db.Invoices.AddRange(invoices);
            await _db.SaveChangesAsync();
        }
    }
}
