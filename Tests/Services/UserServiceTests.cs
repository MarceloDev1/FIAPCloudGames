using FIAPCloudGames.Data;
using FIAPCloudGames.Models.Entities;
using FIAPCloudGames.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;

namespace Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<AppDbContext> _mockContext;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _userService = new UserService(_mockContext.Object, _mockHttpContextAccessor.Object);
        }

        [Fact]
        public async Task GetCurrentUserAsync_WithValidClaims_ReturnsUser()
        {
            // Arrange
            var userId = 1;
            var mockUser = new User { Id = userId, Name = "Test User" };

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }));

            // Create a mock DbSet<User>
            var mockDbSet = new Mock<DbSet<User>>();

            // Setup FindAsync for the mock DbSet
            mockDbSet.Setup(x => x.FindAsync(userId))
                .ReturnsAsync(mockUser);

            // Setup the Users property to return the mock DbSet
            _mockContext.Setup(x => x.Users)
                .Returns(mockDbSet.Object);

            // Act
            var result = await _userService.GetCurrentUserAsync(claimsPrincipal);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
        }

        [Fact]
        public async Task GetCurrentUserAsync_WithInvalidClaims_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _userService.GetCurrentUserAsync(claimsPrincipal));
        }

        [Fact]
        public async Task GetCurrentUserAsync_WithNonExistentUser_ThrowsKeyNotFoundException()
        {
            // Arrange
            var userId = 999;
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }));

            _mockContext.Setup(x => x.Users.FindAsync(userId))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _userService.GetCurrentUserAsync(claimsPrincipal));
        }
    }
}