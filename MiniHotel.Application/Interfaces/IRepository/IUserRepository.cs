using MiniHotel.Domain.Entities;

namespace MiniHotel.Application.Interfaces.IRepository
{
    public interface IUserRepository : IRepository<HotelUser>
    {
        Task<HotelUser> UpdateAsync(HotelUser entity);
    }
}
