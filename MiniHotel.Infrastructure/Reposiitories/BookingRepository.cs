using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Domain.Entities;
using MiniHotel.Infrastructure.Data;

namespace MiniHotel.Infrastructure.Reposiitories
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        private readonly MiniHotelDbContext _context;

        public BookingRepository(MiniHotelDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task CreateAsync(Booking entity)
        {
            await _context.Bookings.AddAsync(entity);
            await SaveAsync();
        }

        public async Task<Booking> UpdateAsync(Booking entity)
        {
            _context.Bookings.Update(entity);
            await SaveAsync();
            return entity;
        }
    }
}
