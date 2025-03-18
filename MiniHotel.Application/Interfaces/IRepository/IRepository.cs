using System.Linq.Expressions;

namespace MiniHotel.Application.Interfaces.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
        Task<T> GetAsync(Expression<Func<T, bool>> filter);
        Task RemoveAsync(T entity);
        Task SaveAsync();
    }
}
