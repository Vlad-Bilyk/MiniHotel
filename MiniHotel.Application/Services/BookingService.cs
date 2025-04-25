using AutoMapper;
using MiniHotel.Application.Common;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Exceptions;
using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Application.Interfaces.IService;
using MiniHotel.Domain.Entities;
using MiniHotel.Domain.Enums;
using System.Globalization;
using System.Linq.Expressions;

namespace MiniHotel.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IInvoiceService _invoiceService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        private const string IncludeProperties = "Room,Room.RoomType,User,Invoice,Invoice.InvoiceItems";

        public BookingService(IBookingRepository bookingRepository, IRoomRepository roomRepository,
                              IInvoiceService invoiceService, IMapper mapper, IUserRepository userRepository)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
            _invoiceService = invoiceService;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<BookingDto> CreateBookingAsync(BookingCreateDto bookingcreateDto, string userId)
        {
            return await CreateBookingInternal(async () =>
            {
                var room = await ValidateAndGetRoomAsync(bookingcreateDto.RoomNumber, bookingcreateDto.StartDate, bookingcreateDto.EndDate);

                var booking = new Booking
                {
                    UserId = userId,
                    RoomId = room.RoomId,
                    StartDate = bookingcreateDto.StartDate,
                    EndDate = bookingcreateDto.EndDate,
                    PaymentMethod = bookingcreateDto.PaymentMethod,
                    BookingStatus = BookingStatus.Pending,
                };

                return booking;
            });
        }

        public async Task<BookingDto> CreateOfflineBookingAsync(BookingCreateByReceptionDto createDto)
        {
            return await CreateBookingInternal(async () =>
            {
                var room = await ValidateAndGetRoomAsync(createDto.RoomNumber, createDto.StartDate, createDto.EndDate);

                var offlineUser = await _userRepository.GetAsync(u => u.Email == "offline_client@hotel.local")
                    ?? throw new ArgumentException("Offline user not found");

                var booking = new Booking
                {
                    UserId = offlineUser.UserId,
                    RoomId = room.RoomId,
                    StartDate = createDto.StartDate,
                    EndDate = createDto.EndDate,
                    FullName = createDto.FullName,
                    PhoneNumber = createDto.PhoneNumber,
                    PaymentMethod = createDto.PaymentMethod,
                    BookingStatus = BookingStatus.Confirmed,
                };

                return booking;
            });
        }

        public async Task<BookingDto> UpdateBookingStatusAsync(int bookingId, BookingStatus newStatus)
        {
            var booking = await _bookingRepository.GetAsync(b => b.BookingId == bookingId, includeProperties: "Invoice")
                ?? throw new KeyNotFoundException("Booking not found");

            if (booking.BookingStatus == BookingStatus.Cancelled || booking.BookingStatus == BookingStatus.CheckedOut)
            {
                throw new InvalidOperationException("Cannot update status of cancelled or checkedOut booking.");
            }

            if (newStatus == BookingStatus.Cancelled)
            {
                await _invoiceService.UpdateStatusAsync(booking.Invoice.InvoiceId, InvoiceStatus.Cancelled);
            }

            booking.BookingStatus = newStatus;
            await _bookingRepository.UpdateAsync(booking);

            return _mapper.Map<BookingDto>(booking);
        }

        public async Task<BookingDto> GetBookingAsync(Expression<Func<Booking, bool>> filter)
        {
            var booking = await _bookingRepository.GetAsync(filter, IncludeProperties);
            return _mapper.Map<BookingDto>(booking);
        }

        public async Task<PagedResult<BookingDto>> GetBookingsAsync(int pageNumber, int pageSize, string? search = null)
        {
            Expression<Func<Booking, bool>>? filter = null;

            if (!string.IsNullOrWhiteSpace(search))
            {
                var dateFilter = BuildDateFilter(search);
                var textFilter = BuildTextFilter(search);
                filter = CombineFilters(textFilter, dateFilter);
            }

            var result = await _bookingRepository.GetPagedAsync(pageNumber, pageSize, filter, IncludeProperties);
            return _mapper.Map<PagedResult<BookingDto>>(result);
        }

        public async Task<PagedResult<UserBookingsDto>> GetUserBookingsAsync(int pageNumber, int pageSize, string userId)
        {
            var bookings = await _bookingRepository.GetPagedAsync(pageNumber, pageSize, b => b.UserId == userId, IncludeProperties);
            return _mapper.Map<PagedResult<UserBookingsDto>>(bookings);
        }

        public async Task<BookingDto> UpdateBookingAsync(int bookingId, BookingUpdateDto updateDto)
        {
            var room = await ValidateAndGetRoomAsync(updateDto.RoomNumber, updateDto.StartDate, updateDto.EndDate);

            var booking = await _bookingRepository.GetAsync(b => b.BookingId == bookingId, "Room")
                ?? throw new KeyNotFoundException("Booking not found");

            if (booking.PaymentMethod == PaymentMethod.Online)
            {
                throw new InvalidOperationException("Online-paid bookings cannot be edited; please cancel and rebook.");
            }

            if (booking.BookingStatus is BookingStatus.CheckedIn or BookingStatus.CheckedOut or BookingStatus.Cancelled)
            {
                throw new InvalidOperationException("This booking cannot be edited in its current status.");
            }

            booking.RoomId = room.RoomId;
            booking.StartDate = updateDto.StartDate;
            booking.EndDate = updateDto.EndDate;

            await _bookingRepository.UpdateAsync(booking);
            await _invoiceService.UpdateBookingTypeItemAsync(booking.BookingId);

            return _mapper.Map<BookingDto>(booking);
        }

        private async Task<BookingDto> CreateBookingInternal(Func<Task<Booking>> createBookingFunc)
        {
            await using var transaction = await _bookingRepository.BeginTransactionAsync();

            try
            {
                var booking = await createBookingFunc();
                await _bookingRepository.CreateAsync(booking);
                await _invoiceService.CreateInvoiceForBookingAsync(booking.BookingId);
                await transaction.CommitAsync();
                return _mapper.Map<BookingDto>(booking);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new BookingCreationException("Failed to create booking and invoice." + ex.Message, ex);
            }
        }

        private async Task<Room> ValidateAndGetRoomAsync(string roomNumber, DateTime startDate, DateTime endDate)
        {
            var room = await _roomRepository
                .GetAsync(r => r.RoomNumber == roomNumber)
                ?? throw new KeyNotFoundException("Room number not found");
            var available = await _roomRepository.IsRoomAvailableAsync(room.RoomId, startDate, endDate);
            if (!available)
            {
                throw new ArgumentException("Room is not available for the selected dates");
            }
            return room;
        }

        private static Expression<Func<Booking, bool>>? BuildDateFilter(string search)
        {
            var formats = new[] { "dd.MM.yyyy", "MM.yyyy", "yyyy", "dd.MM", "MM" };
            if (!DateTime.TryParseExact(search.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return null;

            return search.Trim().Length switch
            {
                10 => b => b.StartDate.Date == date.Date || b.EndDate.Date == date.Date,
                7 => b => (b.StartDate.Month == date.Month && b.StartDate.Year == date.Year)
                         || (b.EndDate.Month == date.Month && b.EndDate.Year == date.Year),
                5 => b => b.StartDate.Day == date.Day && b.StartDate.Month == date.Month
                         || b.EndDate.Day == date.Day && b.EndDate.Month == date.Month,
                4 => b => b.StartDate.Year == date.Year || b.EndDate.Year == date.Year,
                2 => b => b.StartDate.Month == date.Month || b.EndDate.Month == date.Month,
                _ => null
            };
        }

        private static Expression<Func<Booking, bool>> BuildTextFilter(string search)
        {
            var normalized = search.ToLower();

            return b =>
                (
                    b.FullName != null
                        ? b.FullName.ToLower().Contains(normalized)
                        : (b.User.FirstName.ToLower().Contains(normalized) ||
                           b.User.LastName.ToLower().Contains(normalized))
                )
                || b.Room.RoomNumber.ToLower().Contains(normalized)
                || b.Room.RoomType.RoomCategory.ToLower().Contains(normalized);
        }

        private static Expression<Func<Booking, bool>> CombineFilters(
            Expression<Func<Booking, bool>> filter1,
            Expression<Func<Booking, bool>>? filter2)
        {
            if (filter2 is null) return filter1;

            var param = Expression.Parameter(typeof(Booking), "b");

            var body1 = Expression.Invoke(filter1, param);
            var body2 = Expression.Invoke(filter2, param);

            var bodyCombined = Expression.OrElse(body1, body2);
            return Expression.Lambda<Func<Booking, bool>>(bodyCombined, param);
        }
    }
}
