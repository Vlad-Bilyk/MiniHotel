using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiniHotel.Application.DTOs;
using MiniHotel.Domain.Enums;
using MiniHotel.Infrastructure.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MiniHotel.Tests.Integration.ControllersTests
{
    [Collection("IntegrationTests")]
    public class BookingControllerTests : IDisposable
    {
        private readonly HttpClient _client;
        private readonly IServiceScope _scope;
        private readonly MiniHotelDbContext _db;

        public BookingControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _scope = factory.Services.CreateScope();
            _db = _scope.ServiceProvider.GetRequiredService<MiniHotelDbContext>();
        }

        public void Dispose() => _scope.Dispose();

        [Fact]
        public async Task CreateBooking_ShouldReturn201AndPersist_WhenRoomIsAvailable()
        {
            // Arrange
            await AuthenticateAsync();

            var createBookingDto = new BookingCreateDto
            {
                RoomNumber = "101",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(2),
                PaymentMethod = PaymentMethod.OnSite
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/bookings", createBookingDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var body = await response.Content.ReadFromJsonAsync<BookingDto>(JsonHelpers.Default);
            body!.RoomNumber.Should().Be("101");

            var exists = await _db.Bookings.AnyAsync(b => b.BookingId == body.BookingId);
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task CreateBooking_ShouldReturn400_WhenEndDateIsEarlierThanStartDate()
        {
            // Arrange
            await AuthenticateAsync();

            var createBookingDto = new BookingCreateDto
            {
                RoomNumber = "102",
                StartDate = DateTime.Today.AddDays(2),
                EndDate = DateTime.Today,
                PaymentMethod = PaymentMethod.OnSite
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/bookings", createBookingDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateBooking_ShouldReturn400_WhenRoomIsAlreadyBooked()
        {
            // Arrange
            await AuthenticateAsync();

            var createBookingDto = new BookingCreateDto
            {
                RoomNumber = "103",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(2),
                PaymentMethod = PaymentMethod.OnSite
            };

            // Act
            // First booking to create the conflict
            await _client.PostAsJsonAsync("/api/bookings", createBookingDto);
            // Second booking with the same room and overlapping dates
            var response = await _client.PostAsJsonAsync("/api/bookings", createBookingDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateBooking_ShouldReturn401_WhenNoJwtTokenProvided()
        {
            // Arrange
            var createBookingDto = new BookingCreateDto
            {
                RoomNumber = "102",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(2),
                PaymentMethod = PaymentMethod.OnSite
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/bookings", createBookingDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateBooking_ShouldReturn404_WhenRoomDoesNotExist()
        {
            // Arrange
            await AuthenticateAsync();

            var createBookingDto = new BookingCreateDto
            {
                RoomNumber = "999",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(2),
                PaymentMethod = PaymentMethod.OnSite
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/bookings", createBookingDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        private async Task AuthenticateAsync()
        {
            var token = await GetJwtAsync();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task<string> GetJwtAsync()
        {
            var login = new LoginRequestDto { Email = "maria.ivanova@example.com", Password = "Password1!" };
            var response = await _client.PostAsJsonAsync("/api/auth/login", login);
            return await response.Content
                .ReadFromJsonAsync<AuthenticationResultDto>()
                .ContinueWith(task => task!.Result!.Token) ?? string.Empty;
        }
    }
}
