using Microsoft.EntityFrameworkCore.Storage;
using MiniHotel.Application.Common;
using MiniHotel.Domain.Entities;
using System.Linq.Expressions;

namespace MiniHotel.Application.Interfaces.IRepository
{
    public interface IBookingRepository : IBaseRepository<Booking>
    {
        Task CreateAsync(Booking entity);
        Task<Booking> UpdateAsync(Booking entity);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<PagedResult<Booking>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<Booking, bool>>? filter = null, string? includeProperties = "");
    }
}
