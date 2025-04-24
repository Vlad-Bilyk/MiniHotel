using MiniHotel.Domain.Entities;
using MiniHotel.Domain.Enums;

namespace MiniHotel.Application.Interfaces.IRepository
{
    public interface IRoomRepository : IRepository<Room>
    {
        Task CreateAsync(Room entity);
        Task<Room> UpdateAsync(Room entity);
        Task<Room> UpdateStatusAsync(int id, RoomStatus status);
        Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime startDate, DateTime endDate);
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime startDate, DateTime endDate);
    }
}
