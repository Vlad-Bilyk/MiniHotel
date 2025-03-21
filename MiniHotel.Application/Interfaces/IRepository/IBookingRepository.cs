using MiniHotel.Domain.Entities;

namespace MiniHotel.Application.Interfaces.IRepository
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task CreateAsync(Booking entity);
        Task<Booking> UpdateAsync(Booking entity);
    }
}
