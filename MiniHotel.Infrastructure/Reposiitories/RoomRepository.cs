using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Domain.Entities;
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

        public async Task<Room> UpdateAsync(Room entity)
        {
            _context.Rooms.Update(entity);
            await SaveAsync();
            return entity;
        }
    }
}
