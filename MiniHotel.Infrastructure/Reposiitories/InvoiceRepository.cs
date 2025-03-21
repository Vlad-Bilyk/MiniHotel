using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Domain.Entities;
using MiniHotel.Infrastructure.Data;

namespace MiniHotel.Infrastructure.Reposiitories
{
    public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
    {
        private readonly MiniHotelDbContext _context;

        public InvoiceRepository(MiniHotelDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task CreateAsync(Invoice entity)
        {
            await _context.Invoices.AddAsync(entity);
            await SaveAsync();
        }

        public async Task<Invoice> UpdateAsync(Invoice entity)
        {
            _context.Invoices.Update(entity);
            await SaveAsync();
            return entity;
        }
    }
}
