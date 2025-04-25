using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MiniHotel.Application.Common;
using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Domain.Entities;
using MiniHotel.Infrastructure.Data;
using System.Linq.Expressions;

namespace MiniHotel.Infrastructure.Reposiitories
{
    public class BookingRepository : BaseRepository<Booking>, IBookingRepository
    {
        private readonly MiniHotelDbContext _context;

        public BookingRepository(MiniHotelDbContext context) : base(context)
        {
            _context = context;
        }

        public override Task<IEnumerable<Booking>> GetAllAsync(Expression<Func<Booking, bool>>? filter = null, string? includeProperties = "")
        {
            throw new NotSupportedException("Use the paginated version of GetAllAsync instead.");
        }

        public async Task<PagedResult<Booking>> GetPagedAsync(int pageNumber, int pageSize,
                        Expression<Func<Booking, bool>>? filter = null, string? includeProperties = "")
        {
            IQueryable<Booking> query = _context.Bookings;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = query.OrderByDescending(b => b.StartDate);

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split([','], StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Booking>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items
            };
        }

        public Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return _context.Database.BeginTransactionAsync();
        }

        public async Task CreateAsync(Booking entity)
        {
            await _context.Bookings.AddAsync(entity);
            await SaveAsync();
        }

        public async Task<Booking> UpdateAsync(Booking entity)
        {
            _context.Bookings.Update(entity);
            await SaveAsync();
            return entity;
        }
    }
}
