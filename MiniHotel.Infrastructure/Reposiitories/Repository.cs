using Microsoft.EntityFrameworkCore;
using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Infrastructure.Data;
using System.Linq.Expressions;

namespace MiniHotel.Infrastructure.Reposiitories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly MiniHotelDbContext _context;
        internal DbSet<T> _dbSet;

        public Repository(MiniHotelDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task RemoveAsync(T entity)
        {
            _dbSet.Remove(entity);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
