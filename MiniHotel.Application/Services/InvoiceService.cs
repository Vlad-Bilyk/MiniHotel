using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Application.Interfaces.IService;

namespace MiniHotel.Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IBookingRepository _bookingRepository;

        public InvoiceService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        // TODO: Refactor this method with the correct implementation
        public async Task<decimal> CalculateFinalInvoiceAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetAsync(
                b => b.BookingId == bookingId,
                includeProperties: "Room,BookingServices.Service");

            if (booking == null)
            {
                throw new KeyNotFoundException("Booking is not found.");
            }

            var totalDays = (booking.EndDate.Date - booking.StartDate.Date).Days;

            decimal roomCost = booking.Room.RoomType.PricePerNight * totalDays;

            decimal totalCost = roomCost;
            return totalCost;
        }
    }
}
