using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Domain.Entities;
using MiniHotel.Infrastructure.Data;

namespace MiniHotel.Infrastructure.Reposiitories
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        private readonly MiniHotelDbContext _context;

        public PaymentRepository(MiniHotelDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task CreateAsync(Payment entity)
        {
            await _context.Payments.AddAsync(entity);
            await SaveAsync();
        }

        public async Task<Payment> UpdateAsync(Payment entity)
        {
            _context.Payments.Update(entity);
            await SaveAsync();
            return entity;
        }
    }
}
