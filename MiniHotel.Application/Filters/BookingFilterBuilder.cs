using MiniHotel.Domain.Entities;
using MiniHotel.Domain.Enums;
using System.Globalization;
using System.Linq.Expressions;

namespace MiniHotel.Application.Filters
{
    public static class BookingFilterBuilder
    {
        public static Expression<Func<Booking, bool>> BuildStatusFilter(BookingStatus status)
        {
            return b => b.BookingStatus == status;
        }

        public static Expression<Func<Booking, bool>> BuildDateFilter(string search)
        {
            var formats = new[] { "dd.MM.yyyy", "dd.MM", "MM" };
            if (!DateTime.TryParseExact(search.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return b => false;

            return search.Trim().Length switch
            {
                10 => b => b.StartDate.Date == date.Date || b.EndDate.Date == date.Date,
                5 => b => b.StartDate.Day == date.Day && b.StartDate.Month == date.Month
                         || b.EndDate.Day == date.Day && b.EndDate.Month == date.Month,
                2 => b => b.StartDate.Month == date.Month || b.EndDate.Month == date.Month,
                _ => b => false
            };
        }

        public static Expression<Func<Booking, bool>> BuildTextFilter(string search)
        {
            var normalized = search.ToLower();

            return b =>
                (
                    b.FullName != null
                        ? b.FullName.ToLower().Contains(normalized)
                        : b.User.FirstName.ToLower().Contains(normalized) ||
                           b.User.LastName.ToLower().Contains(normalized)
                )
                || b.Room.RoomNumber.ToLower().Contains(normalized)
                || b.Room.RoomType.RoomCategory.ToLower().Contains(normalized);
        }

        public static Expression<Func<Booking, bool>> CombineFilters(
            Expression<Func<Booking, bool>> filter1,
            Expression<Func<Booking, bool>> filter2,
            bool useAnd)
        {
            var param = Expression.Parameter(typeof(Booking), "b");

            var invoked1 = Expression.Invoke(filter1, param);
            var invoked2 = Expression.Invoke(filter2, param);

            var body = useAnd
                ? Expression.AndAlso(invoked1, invoked2)
                : Expression.OrElse(invoked1, invoked2);

            return Expression.Lambda<Func<Booking, bool>>(body, param);
        }
    }
}
