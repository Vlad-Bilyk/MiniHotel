using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Domain.Entities;
using MiniHotel.Infrastructure.Data;

namespace MiniHotel.Infrastructure.Reposiitories
{
    public class ServiceRepository : Repository<Service>, IServiceRepository
    {
        private readonly MiniHotelDbContext _context;

        public ServiceRepository(MiniHotelDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task CreateAsync(Service entity)
        {
            await _context.Services.AddAsync(entity);
            await SaveAsync();
        }

        public async Task<Service> UpdateAsync(Service entity)
        {
            _context.Services.Update(entity);
            await SaveAsync();
            return entity;
        }

        public Task Deactivate(int id)
            => SetAvailability(id, false);

        public Task Reactivate(int id)
            => SetAvailability(id, true);

        private async Task SetAvailability(int id, bool isAvailable)
        {
            var service = await _context.Services.FindAsync(id)
                          ?? throw new KeyNotFoundException("Service not found");
            service.IsAvailable = isAvailable;
            await SaveAsync();
        }
    }
}
