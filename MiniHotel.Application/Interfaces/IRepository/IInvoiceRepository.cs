using MiniHotel.Domain.Entities;

namespace MiniHotel.Application.Interfaces.IRepository
{
    public interface IInvoiceRepository : IRepository<Invoice>
    {
        Task CreateAsync(Invoice entity);
        Task<Invoice> UpdateAsync(Invoice entity);
    }
}
