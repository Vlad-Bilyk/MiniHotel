using MiniHotel.Application.Interfaces.IRepository;
using MiniHotel.Application.Interfaces.IService;
using MiniHotel.Domain.Enums;

namespace MiniHotel.Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IBookingRepository _bookingRepository;

        public InvoiceService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

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

            decimal roomCost = booking.Room.PricePerDay * totalDays;

            decimal serviceCost = booking.BookingServices
                .Where(bs => bs.Status != BookingServiceStatus.Cancelled)
                .Sum(bs => bs.Service.Price * bs.Quantity);

            decimal totalCost = roomCost + serviceCost;
            return totalCost;
        }
    }
}
