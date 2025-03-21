using AutoMapper;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Application.Interfaces.IService;
using MiniHotel.Domain.Entities;
using MiniHotel.Domain.Enums;
using System.Linq.Expressions;

namespace MiniHotel.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IInvoiceService _invoiceService;
        private readonly IMapper _mapper;

        public BookingService(IBookingRepository bookingRepository, IServiceRepository serviceRepository,
                              IRoomRepository roomRepository, IInvoiceService invoiceService, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _serviceRepository = serviceRepository;
            _roomRepository = roomRepository;
            _invoiceService = invoiceService;
            _mapper = mapper;
        }

        public async Task<BookingDto> CreateBookingAsync(BookingCreateDto bookingcreateDto, string userId)
        {
            var room = await _roomRepository.GetAsync(r => r.RoomNumber == bookingcreateDto.RoomNumber);
            if (room == null)
            {
                throw new ArgumentException("Room number not found");
            }

            var availableRooms = await _roomRepository.GetAvailableRoomsAsync(bookingcreateDto.StartDate, bookingcreateDto.EndDate);
            if (!availableRooms.Any(r => r.RoomId == room.RoomId))
            {
                throw new ArgumentException("Room is not available for the selected dates");
            }

            var booking = new Booking
            {
                UserId = userId,
                RoomId = room.RoomId,
                StartDate = bookingcreateDto.StartDate,
                EndDate = bookingcreateDto.EndDate,
                BookingStatus = BookingStatus.Pending,
                IsFullPaid = false
            };

            await _bookingRepository.CreateAsync(booking);
            var bookingDto = _mapper.Map<BookingDto>(booking);
            bookingDto.FinalInvoiceAmount = await GetFinalInvoiceAsync(booking.BookingId);
            return bookingDto;
        }

        public async Task<BookingDto> UpdateBookingStatusAsync(int bookingId, BookingStatus newStatus)
        {
            var booking = await _bookingRepository.GetAsync(b => b.BookingId == bookingId)
                ?? throw new KeyNotFoundException("Booking not found");

            if (booking.BookingStatus == BookingStatus.Completed || booking.BookingStatus == BookingStatus.Cancelled)
            {
                throw new InvalidOperationException("Cannot update status of completed or cancelled booking.");
            }

            booking.BookingStatus = newStatus;
            await _bookingRepository.UpdateAsync(booking);

            return _mapper.Map<BookingDto>(booking);
        }

        // TODO: Implement this method
        public async Task<BookingDto> AddServicesToBookingAsync(int bookingId, IEnumerable<ServiceToBookingDto> services)
        {
            throw new NotImplementedException();
        }

        public async Task<BookingDto> GetBookingAsync(Expression<Func<Booking, bool>> filter)
        {
            var includeProps = "Room,User";
            var booking = await _bookingRepository.GetAsync(filter, includeProps);
            return _mapper.Map<BookingDto>(booking);
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsAsync(Expression<Func<Booking, bool>>? filter = null)
        {
            var includeProps = "Room,User";
            var bookings = await _bookingRepository.GetAllAsync(filter, includeProps);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }

        private async Task<decimal> GetFinalInvoiceAsync(int bookingId)
        {
            return await _invoiceService.CalculateFinalInvoiceAsync(bookingId);
        }
    }
}
