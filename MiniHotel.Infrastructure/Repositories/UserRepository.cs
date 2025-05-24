using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Domain.Entities;
using MiniHotel.Infrastructure.Data;

namespace MiniHotel.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<HotelUser>, IUserRepository
    {
        private readonly MiniHotelDbContext _context;

        public UserRepository(MiniHotelDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<HotelUser> UpdateAsync(HotelUser entity)
        {
            _context.HotelUsers.Update(entity);
            await SaveAsync();
            return entity;
        }
    }
}
