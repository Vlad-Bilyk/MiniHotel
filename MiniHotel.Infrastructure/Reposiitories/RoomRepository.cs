using Microsoft.EntityFrameworkCore;
using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Domain.Entities;
using MiniHotel.Domain.Enums;
using MiniHotel.Infrastructure.Data;

namespace MiniHotel.Infrastructure.Reposiitories
{
    public class RoomRepository : Repository<Room>, IRoomRepository
    {
        private readonly MiniHotelDbContext _context;

        public RoomRepository(MiniHotelDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task CreateAsync(Room entity)
        {
            await _context.Rooms.AddAsync(entity);
            await SaveAsync();
        }

        public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime startDate, DateTime endDate)
        {
            var boookedRooms = await _context.Bookings
                .Where(b => !(b.EndDate <= startDate || b.StartDate >= endDate)
                       && b.BookingStatus != BookingStatus.Cancelled)
                .Select(b => b.RoomId)
                .Distinct()
                .ToListAsync();

            return await _context.Rooms
                .Where(r => !boookedRooms.Contains(r.RoomId)
                       && r.RoomStatus != RoomStatus.UnderMaintenance)
                .ToListAsync();
        }

        public async Task<Room> UpdateAsync(Room entity)
        {
            _context.Rooms.Update(entity);
            await SaveAsync();
            return entity;
        }
    }
}
