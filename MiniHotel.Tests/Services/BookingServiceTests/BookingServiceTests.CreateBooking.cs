using FluentAssertions;
using MiniHotel.Application.DTOs;
using MiniHotel.Application.Exceptions;
using MiniHotel.Domain.Entities;
using MiniHotel.Domain.Enums;
using Moq;
using System.Linq.Expressions;
using static Moq.It;

namespace MiniHotel.Tests.Services.BookingServiceTests
{
    public partial class BookingServiceTests
    {
        #region CreateBookingAsync

        [Fact]
        public async Task CreateBookingAsync_ShouldReturnDto_WhenRoomExistsAndAvailable()
        {
            // Arrange
            var room = BuildRoom();
            SetupGetRoom(room);
            SetupIsRoomAvailable(true);

            var createDto = BuildBookingCreateDto();
            const string userId = "user123";

            SetupBeginTransaction();
            SetupCreateBooking();
            SetupCreateInvoice();

            var expectedDto = new BookingDto { BookingId = 1, RoomNumber = "101", InvoiceId = 1 };
            SetupMapper(expectedDto);

            // Act
            var result = await _bookingService.CreateBookingAsync(createDto, userId);

            // Assert
            result.Should().BeEquivalentTo(expectedDto);
            _bookingRepo.Verify(repo => repo.CreateAsync(IsAny<Booking>()), Times.Once);
            _invoiceService.Verify(service => service.CreateInvoiceForBookingAsync(IsAny<int>()), Times.Once);
            _mockTransaction?.Verify(t => t.CommitAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldRollbackAndThrow_WhenInvoiceFails()
        {
            // Arrange
            var createDto = BuildBookingCreateDto();
            const string userId = "user123";

            var room = BuildRoom();
            SetupGetRoom(room);
            SetupIsRoomAvailable(true);

            SetupBeginTransaction();

            _invoiceService
                .Setup(service => service.CreateInvoiceForBookingAsync(IsAny<int>()))
                .ThrowsAsync(new Exception());

            // Act
            var result = async () =>
                await _bookingService.CreateBookingAsync(createDto, userId);

            // Assert
            await result
                .Should()
                .ThrowAsync<BookingCreationException>();
            _mockTransaction?.Verify(t => t.RollbackAsync(CancellationToken.None), Times.Once);
        }

        #endregion

        #region CreateOfflineBookingAsync

        [Fact]
        public async Task CreateOfflineBookingAsync_ShouldReturnDto_WhenRoomExistsAndAvailable()
        {
            // Arrange
            var room = BuildRoom();
            SetupGetRoom(room);
            SetupIsRoomAvailable(true);

            var dto = BuildBookingCreateByReceptionDto();

            var offlineUser = new HotelUser { UserId = "offline", Email = "offline_client@hotel.local", PhoneNumber = "+38011111", FirstName = "Offline", LastName = "Offline" };
            _userRepo
                .Setup(repo => repo.GetAsync(IsAny<Expression<Func<HotelUser, bool>>>(), ""))
                .ReturnsAsync(offlineUser);

            SetupBeginTransaction();
            SetupCreateBooking();
            SetupCreateInvoice();

            var expectedDto = new BookingDto { BookingId = 1, RoomNumber = "101", InvoiceId = 1, BookingStatus = BookingStatus.Confirmed };
            SetupMapper(expectedDto);

            // Act
            var result = await _bookingService.CreateOfflineBookingAsync(dto);

            // Assert
            result.BookingStatus.Should().Be(BookingStatus.Confirmed);
            _bookingRepo.Verify(repo => repo.CreateAsync(Is<Booking>(b =>
                    b.UserId == offlineUser.UserId &&
                    b.BookingStatus == BookingStatus.Confirmed)), Times.Once);
            _mockTransaction?.Verify(t => t.CommitAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task CreateOfflineBookingAsync_ShouldThrowInvalidOperation_WhenOfflineClientMissing()
        {
            // Arrange
            var room = BuildRoom();
            SetupGetRoom(room);
            SetupIsRoomAvailable(true);

            var dto = BuildBookingCreateByReceptionDto();

            var expectedDto = new BookingDto { BookingId = 1, RoomNumber = "101", InvoiceId = 1, BookingStatus = BookingStatus.Confirmed };
            SetupMapper(expectedDto);

            // Act
            var result = async () =>
                await _bookingService.CreateOfflineBookingAsync(dto);

            // Assert
            await result
                .Should()
                .ThrowAsync<InvalidOperationException>();
        }

        #endregion

        #region Helpers

        private static BookingCreateDto BuildBookingCreateDto() =>
            new BookingCreateDto
            {
                RoomNumber = "101",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(2),
                PaymentMethod = PaymentMethod.OnSite
            };

        private static BookingCreateByReceptionDto BuildBookingCreateByReceptionDto() =>
            new BookingCreateByReceptionDto()
            {
                RoomNumber = "101",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(2),
                FullName = "John Doe",
                PhoneNumber = "+38011111111",
                PaymentMethod = PaymentMethod.OnSite
            };

        private void SetupBeginTransaction()
        {
            _mockTransaction
                .Setup(t => t.CommitAsync(IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _mockTransaction
                .Setup(t => t.RollbackAsync(IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _bookingRepo
                .Setup(repo => repo.BeginTransactionAsync())
                .ReturnsAsync(_mockTransaction.Object);
        }

        private void SetupCreateBooking()
        {
            _bookingRepo
                .Setup(repo => repo.CreateAsync(IsAny<Booking>()))
                .Returns(Task.CompletedTask);
        }

        private void SetupCreateInvoice()
        {
            _invoiceService
                .Setup(service => service.CreateInvoiceForBookingAsync(IsAny<int>()))
                .ReturnsAsync(new InvoiceDto { InvoiceId = 1, BookingId = 1 });
        }

        private void SetupMapper(BookingDto expectedDto)
        {
            _mapper
                .Setup(m => m.Map<BookingDto>(IsAny<Booking>()))
                .Returns(expectedDto);
        }

        #endregion
    }
}
