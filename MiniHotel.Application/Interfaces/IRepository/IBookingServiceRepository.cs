using MiniHotel.Domain.Entities;

namespace MiniHotel.Application.Interfaces.IRepository
{
    public interface IBookingServiceRepository : IRepository<BookingService>
    {
        Task CreateAsync(BookingService entity);
        Task<BookingService> UpdateAsync(BookingService entity);
    }
}
