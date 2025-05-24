using Microsoft.EntityFrameworkCore;
using MiniHotel.Application.Exceptions;
using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Infrastructure.Data;
using System.Linq.Expressions;

namespace MiniHotel.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly MiniHotelDbContext _context;
        internal DbSet<T> DbSet;

        public BaseRepository(MiniHotelDbContext context)
        {
            _context = context;
            DbSet = _context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = "")
        {
            IQueryable<T> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (string.IsNullOrWhiteSpace(includeProperties)) return await query.ToListAsync();

            query = includeProperties
                .Split([','], StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(query, (current, includeProp) =>
                    current.Include(includeProp));

            return await query.ToListAsync();
        }

        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> filter, string? includeProperties = "")
        {
            IQueryable<T> query = DbSet;
            query = query.Where(filter);

            if (string.IsNullOrWhiteSpace(includeProperties))
                return await query.FirstOrDefaultAsync()
                       ?? throw new NotFoundException($"{typeof(T).Name} not found");

            query = includeProperties
                .Split([','], StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(query, (current, includeProp) =>
                    current.Include(includeProp));

            return await query.FirstOrDefaultAsync()
                   ?? throw new NotFoundException($"{typeof(T).Name} not found");
        }

        public async Task RemoveAsync(T entity)
        {
            DbSet.Remove(entity);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
