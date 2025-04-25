using MiniHotel.Domain.Entities;

namespace MiniHotel.Application.Interfaces.IRepository
{
    public interface IRoomTypeRepository : IBaseRepository<RoomType>
    {
        Task CreateAsync(RoomType entity);
        Task<RoomType> UpdateAsync(RoomType entity);
    }
}
