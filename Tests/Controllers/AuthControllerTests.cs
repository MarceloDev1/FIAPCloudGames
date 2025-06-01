using FIAPCloudGames.Controllers;
using FIAPCloudGames.Models.Auth;
using FIAPCloudGames.Models.DTOs.Users;
using FIAPCloudGames.Models.Entities;
using FIAPCloudGames.Models.Enums;
using FIAPCloudGames.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace FIAPCloudGames.UnitTests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockUserService = new Mock<IUserService>();
            _controller = new AuthController(_mockAuthService.Object, _mockUserService.Object);
        }

        [Fact]
        public void Status_ShouldReturnOkWithStatusMessage()
        {
            // Act
            var result = _controller.Status();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value;
            var type = value.GetType();

            var message = type.GetProperty("message")?.GetValue(value) as string;
            var status = type.GetProperty("status")?.GetValue(value) as string;

            Assert.Equal("API de autenticação operacional", message);
            Assert.Equal("Online", status);
        }

        [Fact]
        public async Task Register_ShouldReturnCreatedWithAuthResponse_WhenSuccessful()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "password",
                ConfirmPassword = "password"
            };

            // Criando um usuário de teste para o construtor do AuthResponse
            var testUser = new User
            {
                Id = 1,
                Name = "Test User",
                Email = "test@example.com"
            };

            // Criando o UserResponseDto que será retornado pelo UserService
            var userResponseDto = new UserResponseDto
            {
                Id = 1,
                Name = "Test User",
                Email = "test@example.com"
            };

            // Configurando os mocks
            _mockUserService.Setup(x => x.CreateUserAsync(It.IsAny<UserCreateDto>()))
                .ReturnsAsync(userResponseDto);

            // Configurando o mock para retornar um AuthResponse válido
            _mockAuthService.Setup(x => x.AuthenticateAsync(It.IsAny<LoginRequest>()))
                .ReturnsAsync(new AuthResponse(testUser, "test-token"));

            // Act
            var result = await _controller.Register(registerRequest);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var authResponse = Assert.IsType<AuthResponse>(createdAtActionResult.Value);

            Assert.Equal(testUser.Id, authResponse.UserId);
            Assert.Equal(testUser.Name, authResponse.Name);
            Assert.Equal(testUser.Email, authResponse.Email);
            Assert.Equal("test-token", authResponse.Token);
            Assert.True(authResponse.ExpiresIn > DateTime.UtcNow); // Verifica se a expiração está no futuro
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "password",
                ConfirmPassword = "password"
            };

            _mockUserService.Setup(x => x.CreateUserAsync(It.IsAny<UserCreateDto>()))
                .ThrowsAsync(new Exception("Error message"));

            // Act
            var result = await _controller.Register(registerRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            var valueType = badRequestResult.Value.GetType();
            var messageProperty = valueType.GetProperty("message");
            Assert.NotNull(messageProperty);

            var messageValue = messageProperty.GetValue(badRequestResult.Value) as string;
            Assert.Equal("Error message", messageValue);
        }

        [Fact]
        public async Task Login_ShouldReturnOkWithAuthResponse_WhenCredentialsAreValid()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "test@example.com",
                Password = "password"
            };

            var testUser = new User
            {
                Id = 1,
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashed_password",
                Role = UserRole.User
            };

            var authResponse = new AuthResponse(testUser, "test-token");

            _mockAuthService.Setup(x => x.AuthenticateAsync(loginRequest))
                .ReturnsAsync(authResponse);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAuthResponse = Assert.IsType<AuthResponse>(okResult.Value);

            Assert.Equal(authResponse.UserId, returnedAuthResponse.UserId);
            Assert.Equal(authResponse.Name, returnedAuthResponse.Name);
            Assert.Equal(authResponse.Email, returnedAuthResponse.Email);
            Assert.Equal(authResponse.Token, returnedAuthResponse.Token);
            Assert.True(returnedAuthResponse.ExpiresIn > DateTime.UtcNow);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "test@example.com",
                Password = "wrong-password"
            };

            _mockAuthService.Setup(x => x.AuthenticateAsync(loginRequest))
                .ReturnsAsync((AuthResponse)null);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);

            var valueType = unauthorizedResult.Value.GetType();
            var messageProperty = valueType.GetProperty("message");
            Assert.NotNull(messageProperty);

            var messageValue = messageProperty.GetValue(unauthorizedResult.Value) as string;
            Assert.Equal("Credenciais inválidas", messageValue);
        }

        [Fact]
        public async Task GetCurrentUser_ShouldReturnOkWithUserData_WhenUserIsAuthenticated()
        {
            // Arrange
            var userId = 1;
            var user = new UserResponseDto
            {
                Id = userId,
                Name = "Test User",
                Email = "test@example.com",
                Role = UserRole.User
            };

            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            _mockUserService.Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            var value = okResult.Value;
            var type = value.GetType();

            var id = (int)type.GetProperty("Id").GetValue(value);
            var name = (string)type.GetProperty("Name").GetValue(value);
            var email = (string)type.GetProperty("Email").GetValue(value);
            var role = (UserRole)type.GetProperty("Role").GetValue(value);

            Assert.Equal(user.Id, id);
            Assert.Equal(user.Name, name);
            Assert.Equal(user.Email, email);
            Assert.Equal(user.Role, role);
        }
    }
}