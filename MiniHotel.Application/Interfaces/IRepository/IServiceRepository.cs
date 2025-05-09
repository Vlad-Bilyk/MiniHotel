﻿using MiniHotel.Domain.Entities;

namespace MiniHotel.Application.Interfaces.IRepository
{
    public interface IServiceRepository : IBaseRepository<Service>
    {
        Task CreateAsync(Service entity);
        Task<Service> UpdateAsync(Service entity);
        Task Deactivate(int id);
        Task Reactivate(int id);
    }
}
