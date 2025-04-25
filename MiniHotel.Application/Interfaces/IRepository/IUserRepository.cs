using MiniHotel.Domain.Entities;

namespace MiniHotel.Application.Interfaces.IRepository
{
    public interface IUserRepository : IBaseRepository<HotelUser>
    {
        Task<HotelUser> UpdateAsync(HotelUser entity);
    }
}
