using MiniHotel.Application.Common;
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
        Task<PagedResult<BookingDto>> GetBookingsAsync(int pageNumber, int pageSize, string? search = null, BookingStatus? status = null);
        Task<PagedResult<UserBookingsDto>> GetUserBookingsAsync(int pageNumber, int pageSize, string userId);
        Task<BookingDto> CreateOfflineBookingAsync(BookingCreateByReceptionDto createDto);
        Task<BookingDto> UpdateBookingAsync(int bookingId, BookingUpdateDto updateDto);
    }
}
