using Microsoft.EntityFrameworkCore;
using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Domain.Entities;
using MiniHotel.Infrastructure.Data;

namespace MiniHotel.Infrastructure.Reposiitories
{
    public class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
    {
        private readonly MiniHotelDbContext _context;

        public InvoiceRepository(MiniHotelDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Invoice> AddItemAsync(InvoiceItem item)
        {
            await _context.InvoiceItems.AddAsync(item);
            await SaveAsync();
            return await _context.Invoices
                .Include(i => i.InvoiceItems)
                .FirstAsync(i => i.InvoiceId == item.InvoiceId);
        }

        public async Task CreateAsync(Invoice invoice)
        {
            await _context.Invoices.AddAsync(invoice);
            await SaveAsync();
        }

        public async Task<int> RemoveItemAsync(int invoiceItemId)
        {
            var item = await _context.InvoiceItems.FindAsync(invoiceItemId)
                ?? throw new KeyNotFoundException("Invoice item not found");

            var invoiceId = item.InvoiceId;
            _context.InvoiceItems.Remove(item);
            await SaveAsync();
            return invoiceId; // Returns the invoice ID to allow recalculation after removing an item
        }

        public async Task UpdateAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            await SaveAsync();
        }
    }
}
