using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Domain.Entities;
using MiniHotel.Infrastructure.Data;

namespace MiniHotel.Infrastructure.Repositories
{
    public class RoomTypeRepository : BaseRepository<RoomType>, IRoomTypeRepository
    {
        private readonly MiniHotelDbContext _context;

        public RoomTypeRepository(MiniHotelDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task CreateAsync(RoomType entity)
        {
            await _context.RoomTypes.AddAsync(entity);
            await SaveAsync();
        }

        public async Task<RoomType> UpdateAsync(RoomType entity)
        {
            _context.RoomTypes.Update(entity);
            await SaveAsync();
            return entity;
        }
    }
}
