using MiniHotel.Application.DTOs;
using MiniHotel.Domain.Entities;
using MiniHotel.Domain.Enums;
using System.Linq.Expressions;

namespace MiniHotel.Application.Interfaces.IService
{
    public interface IBookingService
    {
        Task<BookingDto> CreateBookingAsync(BookingCreateDto bookingcreateDto, string userId);
        Task<BookingDto> UpdateBookingStatusAsync(int bookingId, BookingStatus newStatus);
        Task<BookingDto> GetBookingAsync(Expression<Func<Booking, bool>> filter);
        Task<IEnumerable<BookingDto>> GetBookingsAsync(Expression<Func<Booking, bool>>? filter = null);
        Task<IEnumerable<UserBookingsDto>> GetUserBookingsAsync(string userId);
        Task<BookingDto> CreateOfflineBookingAsync(BookingCreateByAdminDto createDto);
    }
}
