using MiniHotel.Domain.Entities;

namespace MiniHotel.Application.Interfaces.IRepository
{
    public interface IServiceRepository : IRepository<Service>
    {
        Task CreateAsync(Service entity);
        Task<Service> UpdateAsync(Service entity);
    }
}
