using FluentAssertions;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Exceptions;
using MiniHotel.Domain.Entities;
using MiniHotel.Domain.Enums;
using Moq;

namespace MiniHotel.Tests.Unit.ApplicationLayer.BookingServiceTests
{
    public partial class BookingServiceTests
    {
        [Fact]
        public async Task UpdateBookingAsync_ShouldReturnDto_WhenBookingExistsAndEditable()
        {
            // Arrange
            var room = BuildRoom();
            SetupGetRoom(room);
            SetupIsRoomAvailable(true);

            var currentBooking = BuildBooking();
            SetupGetBooking(currentBooking);

            var updateDto = BuildUpdateBookingDto();

            _invoiceService
                .Setup(s => s.UpdateBookingTypeItemAsync(currentBooking.BookingId))
                .Returns(Task.CompletedTask);

            var expectedDto = new BookingDto { BookingId = currentBooking.BookingId };
            _mapper
                .Setup(m => m.Map<BookingDto>(It.IsAny<Booking>()))
                .Returns(expectedDto);

            // Act
            var result = await _bookingService.UpdateBookingAsync(currentBooking.BookingId, updateDto);

            // Assert
            result.Should().BeEquivalentTo(expectedDto);
            _bookingRepo.Verify(repo => repo.UpdateAsync(
                It.Is<Booking>(b =>
                    b.BookingId == currentBooking.BookingId &&
                    b.RoomId == room.RoomId &&
                    b.StartDate == updateDto.StartDate &&
                    b.EndDate == updateDto.EndDate)), Times.Once);
            _invoiceService.Verify(s => s.UpdateBookingTypeItemAsync(currentBooking.BookingId), Times.Once);
        }

        [Fact]
        public async Task UpdateBookingAsync_ShouldThrowNotFound_WhenBookingDoesNotExist()
        {
            // Arrange
            SetupGetRoom(BuildRoom());
            SetupIsRoomAvailable(true);

            var updateDto = BuildUpdateBookingDto();

            // Act
            var result = async () =>
                await _bookingService.UpdateBookingAsync(999, updateDto);

            // Assert
            await result
                .Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("Booking not found");
        }

        [Fact]
        public async Task UpdateBookingAsync_ShouldThrowBadRequest_WhenPaymentMethodOnline()
        {
            // Arrange
            SetupGetRoom(BuildRoom());
            SetupIsRoomAvailable(true);

            var currentBooking = BuildBooking();
            SetupGetBooking(currentBooking);

            currentBooking.PaymentMethod = PaymentMethod.Online;

            // Act
            var result = async () =>
                await _bookingService.UpdateBookingAsync(currentBooking.BookingId, new BookingUpdateDto());

            // Assert
            await result
                .Should()
                .ThrowAsync<BadRequestException>();
        }

        [Theory]
        [InlineData(BookingStatus.Cancelled)]
        [InlineData(BookingStatus.CheckedIn)]
        [InlineData(BookingStatus.CheckedOut)]
        public async Task UpdateBookingAsync_ShouldThrowBadRequest_WhenBookingInTerminalStatus(BookingStatus status)
        {
            // Arrange
            SetupGetRoom(BuildRoom());
            SetupIsRoomAvailable(true);

            var currentBooking = BuildBooking();
            SetupGetBooking(currentBooking);

            currentBooking.BookingStatus = status;

            // Act & Assert
            await FluentActions.Invoking(() =>
                    _bookingService.UpdateBookingAsync(currentBooking.BookingId, new BookingUpdateDto()))
                .Should()
                .ThrowAsync<BadRequestException>();
        }

        #region Helpers

        private static BookingUpdateDto BuildUpdateBookingDto() => new BookingUpdateDto
        {
            RoomNumber = "101",
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(3)
        };
    }

    #endregion
}
