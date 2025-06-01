using FIAPCloudGames.Models.Auth;
using FIAPCloudGames.Models.Entities;
using FIAPCloudGames.Services;
using FIAPCloudGames.Services.Interfaces;
using Moq;
using FIAPCloudGames.Models.Enums;

namespace Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly JwtSettings _jwtSettings;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUserService = new Mock<IUserService>();

            // Configuração básica do JWT para testes
            _jwtSettings = new JwtSettings
            {
                Secret = "SuperSecretKeyForTesting123!",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpiryMinutes = 60
            };

            _authService = new AuthService(_mockUserService.Object, _jwtSettings);
        }        

        [Fact]
        public async Task AuthenticateAsync_WithInvalidEmail_ReturnsNull()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "nonexistent@example.com",
                Password = "anypassword"
            };

            _mockUserService.Setup(x => x.GetByEmailAsync(loginRequest.Email))
                .ReturnsAsync((User)null);

            // Act
            var result = await _authService.AuthenticateAsync(loginRequest);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_WithInvalidPassword_ReturnsNull()
        {
            // Arrange
            var testUser = new User
            {
                Id = 1,
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
                Role = UserRole.User
            };

            var loginRequest = new LoginRequest
            {
                Email = "test@example.com",
                Password = "wrongpassword"
            };

            _mockUserService.Setup(x => x.GetByEmailAsync(loginRequest.Email))
                .ReturnsAsync(testUser);

            // Act
            var result = await _authService.AuthenticateAsync(loginRequest);

            // Assert
            Assert.Null(result);
        }        
    }
}