using FluentAssertions;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Exceptions;
using MiniHotel.Domain.Enums;
using Moq;

namespace MiniHotel.Tests.Unit.ApplicationLayer.BookingServiceTests
{
    public partial class BookingServiceTests
    {
        [Fact]
        public async Task UpdateBookingStatusAsync_ShouldThrowNotFoundException_WhenBookingDoesNotExist()
        {
            // Arrange

            // Act 
            var result = async () =>
                await _bookingService.UpdateBookingStatusAsync(1, BookingStatus.Confirmed);

            // Assert
            await result
                .Should()
                .ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task UpdateBookingStatusAsync_ShouldUpdateBookingStatus_WhenBookingExists()
        {
            // Arrange
            var booking = BuildBooking();
            SetupGetBooking(booking);

            // Act
            await _bookingService.UpdateBookingStatusAsync(booking.BookingId, BookingStatus.Confirmed);

            // Assert
            booking.BookingStatus.Should().Be(BookingStatus.Confirmed);
            _bookingRepo
                .Verify(repo => repo.UpdateAsync(booking), Times.Once);
        }

        [Fact]
        public async Task UpdateBookingStatusAsync_ShouldCancelInvoiceAndUpdateBooking_WhenNewStatusIsCancelled()
        {
            // Arrange
            var booking = BuildBooking();
            SetupGetBooking(booking);

            // TODO: Replace after add integration tests
            var returnedDto = new BookingDto
            {
                BookingId = booking.BookingId,
                BookingStatus = BookingStatus.Cancelled
            };
            _mapper
                .Setup(m => m.Map<BookingDto>(booking))
                .Returns(returnedDto);

            // Act
            var result = await _bookingService.UpdateBookingStatusAsync(booking.BookingId, BookingStatus.Cancelled);

            // Assert
            _invoiceService
                .Verify(service => service.UpdateStatusAsync(booking.Invoice.InvoiceId, InvoiceStatus.Cancelled), Times.Once);
            _bookingRepo
                .Verify(repo => repo.UpdateAsync(booking), Times.Once);

            result.BookingStatus.Should().Be(BookingStatus.Cancelled);
            result.Should().BeEquivalentTo(returnedDto);
        }

        [Theory]
        [InlineData(BookingStatus.Cancelled)]
        [InlineData(BookingStatus.CheckedOut)]
        public async Task UpdateBookingStatusAsync_ShouldThrowBadRequestException_WhenBookingIsCancelledOrCheckedOut(BookingStatus status)
        {
            // Arrange
            var booking = BuildBooking(1, "u123", status);
            SetupGetBooking(booking);

            // Act
            var result = async () =>
                await _bookingService.UpdateBookingStatusAsync(booking.BookingId, BookingStatus.Confirmed);

            // Assert
            await result
                .Should()
                .ThrowAsync<BadRequestException>();
        }
    }
}
