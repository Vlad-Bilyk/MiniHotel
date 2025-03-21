using MiniHotel.Domain.Entities;

namespace MiniHotel.Application.Interfaces.IRepository
{
    public interface IInvoiceRepository : IRepository<Invoice>
    {
        Task<Invoice?> GetByBookingIdAsync(int bookingId);
        Task<Invoice> AddItemAsync(InvoiceItem item);
        Task UpdateAsync(Invoice invoice);
        Task CreateAsync(Invoice invoice);
        Task RemoveItemAsync(int invoiceItemId);
    }
}
