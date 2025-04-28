using Microsoft.AspNetCore.Mvc;
using MiniHotel.API.Controllers;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Interfaces.IService;
using MiniHotel.Domain.Enums;
using Moq;

namespace MiniHotel.Tests.ControllersTest
{
    public class BookingControllerTests
    {
        private readonly Mock<IBookingService> _bookingServiceMock;
        private readonly BookingsController _controller;

        public BookingControllerTests()
        {
            _bookingServiceMock = new Mock<IBookingService>();
            _controller = new BookingsController(_bookingServiceMock.Object);
        }

        [Fact]
        public async Task CancelBooking_ReturnOkResult_WhenBookingStatusUpdatedSuccessfully()
        {
            // Arrange
            int bookingId = 1;
            var expectedBooking = new BookingDto { BookingId = bookingId, BookingStatus = BookingStatus.Cancelled };
            _bookingServiceMock.Setup(s => s.UpdateBookingStatusAsync(bookingId, BookingStatus.Cancelled)).ReturnsAsync(expectedBooking);

            // Act
            var result = await _controller.CancelBooking(bookingId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnBooking = Assert.IsType<BookingDto>(okResult.Value);
            Assert.Equal(BookingStatus.Cancelled, returnBooking.BookingStatus);
        }

        [Fact]
        public async Task CheckInBooking_ReturnsOkResult_WhenBookingStatusUpdatedSuccessfully()
        {
            // Arrange
            int bookingId = 2;
            var expectedBooking = new BookingDto { BookingId = bookingId, BookingStatus = BookingStatus.CheckedIn };
            _bookingServiceMock.Setup(s => s.UpdateBookingStatusAsync(bookingId, BookingStatus.CheckedIn))
                               .ReturnsAsync(expectedBooking);

            // Act
            var result = await _controller.CheckInBooking(bookingId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBooking = Assert.IsType<BookingDto>(okResult.Value);
            Assert.Equal(BookingStatus.CheckedIn, returnedBooking.BookingStatus);
        }

        [Fact]
        public async Task CheckOutBooking_ReturnsOkResult_WhenBookingStatusUpdatedSuccessfully()
        {
            // Arrange
            int bookingId = 3;
            var expectedBooking = new BookingDto { BookingId = bookingId, BookingStatus = BookingStatus.CheckedOut };
            _bookingServiceMock.Setup(s => s.UpdateBookingStatusAsync(bookingId, BookingStatus.CheckedOut))
                               .ReturnsAsync(expectedBooking);

            // Act
            var result = await _controller.CheckOutBooking(bookingId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBooking = Assert.IsType<BookingDto>(okResult.Value);
            Assert.Equal(BookingStatus.CheckedOut, returnedBooking.BookingStatus);
        }

        [Fact]
        public async Task ConfirmedBooking_ReturnsOkResult_WhenBookingStatusUpdatedSuccessfully()
        {
            // Arrange
            int bookingId = 4;
            var expectedBooking = new BookingDto { BookingId = bookingId, BookingStatus = BookingStatus.Confirmed };
            _bookingServiceMock.Setup(s => s.UpdateBookingStatusAsync(bookingId, BookingStatus.Confirmed))
                               .ReturnsAsync(expectedBooking);

            // Act
            var result = await _controller.ConfirmedBooking(bookingId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBooking = Assert.IsType<BookingDto>(okResult.Value);
            Assert.Equal(BookingStatus.Confirmed, returnedBooking.BookingStatus);
        }

        [Fact]
        public async Task CheckInBooking_ReturnsBadRequest_WhenInvalidOperationExceptionThrown()
        {
            // Arrange
            int bookingId = 6;
            _bookingServiceMock.Setup(s => s.UpdateBookingStatusAsync(bookingId, BookingStatus.CheckedIn))
                               .ThrowsAsync(new InvalidOperationException("Invalid operation"));

            // Act
            var result = await _controller.CheckInBooking(bookingId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid operation: Invalid operation", badRequestResult.Value);
        }
    }
}
