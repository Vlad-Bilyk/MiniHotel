using MiniHotel.Domain.Entities;

namespace MiniHotel.Application.Interfaces.IRepository
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task CreateAsync(Payment entity);
        Task<Payment> UpdateAsync(Payment entity);
    }
}
