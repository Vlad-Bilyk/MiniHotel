using MiniHotel.Domain.Entities;

namespace MiniHotel.Application.Interfaces.IRepository
{
    public interface IRoomRepository : IRepository<Room>
    {
        Task CreateAsync(Room entity);
        Task<Room> UpdateAsync(Room entity);
        Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime startDate, DateTime endDate);
    }
}
