using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Domain.Entities;
using MiniHotel.Infrastructure.Data;

namespace MiniHotel.Infrastructure.Reposiitories
{
    public class BookingServiceRepository : Repository<BookingService>, IBookingServiceRepository
    {
        private readonly MiniHotelDbContext _context;

        public BookingServiceRepository(MiniHotelDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task CreateAsync(BookingService entity)
        {
            await _context.BookingServices.AddAsync(entity);
            await SaveAsync();
        }

        public async Task<BookingService> UpdateAsync(BookingService entity)
        {
            _context.BookingServices.Update(entity);
            await SaveAsync();
            return entity;
        }
    }
}
