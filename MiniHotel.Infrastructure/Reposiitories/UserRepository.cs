using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Domain.Entities;
using MiniHotel.Infrastructure.Data;

namespace MiniHotel.Infrastructure.Reposiitories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly MiniHotelDbContext _context;

        public UserRepository(MiniHotelDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User> UpdateAsync(User entity)
        {
            _context.HotelUsers.Update(entity);
            await SaveAsync();
            return entity;
        }
    }
}
