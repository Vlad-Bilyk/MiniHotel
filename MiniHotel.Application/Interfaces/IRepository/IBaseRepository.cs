using System.Linq.Expressions;

namespace MiniHotel.Application.Interfaces.IRepository
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = "");
        Task<T> GetAsync(Expression<Func<T, bool>> filter, string? includeProperties = "");
        Task RemoveAsync(T entity);
        Task SaveAsync();
    }
}
