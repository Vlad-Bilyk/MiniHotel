using FluentAssertions;
using MiniHotel.Application.DTOs;
using MiniHotel.Domain.Enums;
using System.Net;
using System.Net.Http.Json;

namespace MiniHotel.Tests.Integration.ControllersTests
{
    [Collection("IntegrationTests")]
    public class AuthControllerTests
    {
        private readonly HttpClient _client;

        public AuthControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_ShouldReturnsToken_WhenDataValid()
        {
            // Arrange
            var registerDto = new RegisterRequestDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@email.com",
                PhoneNumber = "+380501234567",
                Password = "Password1!",
                ConfirmPassword = "Password1!",
                Role = UserRole.Client
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<AuthenticationResultDto>();
            body!.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Register_ShouldReturns400_WhenDataInValid()
        {
            // Arrange
            var registerDto = new RegisterRequestDto
            {
                FirstName = "Test",
                LastName = "",
                Email = "test@email.com",
                PhoneNumber = "",
                Password = "Password1!",
                ConfirmPassword = "Password1!",
                Role = UserRole.Client
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Login_ShouldReturnsToken_WhenCredentialsValid()
        {
            // Arrange
            var loginDto = new LoginRequestDto { Email = "maria.ivanova@example.com", Password = "Password1!" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<AuthenticationResultDto>();
            body!.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Login_ShouldReturns401_WhenPasswordWrong()
        {
            // Assert
            var loginDto = new LoginRequestDto { Email = "maria.ivanova@example.com", Password = "BadPas!" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

            // Arrange
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
