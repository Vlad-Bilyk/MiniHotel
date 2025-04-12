using MiniHotel.Domain.Entities;

namespace MiniHotel.Application.Interfaces.IRepository
{
    public interface IRoomTypeRepository : IRepository<RoomType>
    {
        Task CreateAsync(RoomType entity);
        Task<RoomType> UpdateAsync(RoomType entity);
    }
}
