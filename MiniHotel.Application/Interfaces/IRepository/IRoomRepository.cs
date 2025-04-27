using MiniHotel.Domain.Entities;
using MiniHotel.Domain.Enums;

namespace MiniHotel.Application.Interfaces.IRepository
{
    public interface IRoomRepository : IBaseRepository<Room>
    {
        Task CreateAsync(Room entity);
        Task<Room> UpdateAsync(Room entity);
        Task<Room> UpdateStatusAsync(int id, RoomStatus status);
        Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime startDate, DateTime endDate, int? ignoreBookingId = null);
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime startDate, DateTime endDate, int? ignoreBookingId = null);
    }
}
