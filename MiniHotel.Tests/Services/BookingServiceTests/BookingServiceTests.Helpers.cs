using MiniHotel.Domain.Entities;
using MiniHotel.Domain.Enums;
using Moq;
using System.Linq.Expressions;
using static Moq.It;

namespace MiniHotel.Tests.Services.BookingServiceTests
{
    public partial class BookingServiceTests
    {
        #region Builders

        private static Room BuildRoom() =>
            new Room
            {
                RoomId = 1,
                RoomNumber = "101",
                RoomTypeId = 1,
            };

        private static Booking BuildBooking(
            int id = 1,
            string userId = "u123",
            BookingStatus s = BookingStatus.Pending) => new Booking
            {
                BookingId = id,
                UserId = userId,
                BookingStatus = s,
                Invoice = new Invoice { InvoiceId = 1 }
            };

        #endregion

        #region Setups

        private void SetupGetBooking(Booking booking)
        {
            _bookingRepo
                .Setup(repo => repo.GetAsync(
                    IsAny<Expression<Func<Booking, bool>>>(),
                    IsAny<string>()))
                .ReturnsAsync(booking);
        }

        private void SetupGetRoom(Room room)
        {
            _roomRepo
                .Setup(repo => repo.GetAsync(
                    IsAny<Expression<Func<Room, bool>>>(),
                    ""))
                .ReturnsAsync(room);
        }

        private void SetupIsRoomAvailable(bool isAvailable)
        {
            _roomRepo
                .Setup(repo => repo.IsRoomAvailableAsync(
                    IsAny<int>(),
                    IsAny<DateTime>(),
                    IsAny<DateTime>(),
                    IsAny<int?>()))
                .ReturnsAsync(isAvailable);
        }

        #endregion
    }
}
