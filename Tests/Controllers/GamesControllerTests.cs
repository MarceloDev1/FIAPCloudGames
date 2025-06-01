using FIAPCloudGames.Controllers;
using FIAPCloudGames.Models.DTOs.Games;
using FIAPCloudGames.Models.DTOs.Users;
using FIAPCloudGames.Models.Enums;
using FIAPCloudGames.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace FIAPCloudGames.UnitTests.Controllers
{
    public class GamesControllerTests
    {
        private readonly Mock<IGameService> _mockGameService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly GamesController _controller;

        public GamesControllerTests()
        {
            _mockGameService = new Mock<IGameService>();
            _mockUserService = new Mock<IUserService>();
            _controller = new GamesController(null, _mockGameService.Object, _mockUserService.Object);
        }

        #region GetAllGames Tests

        [Fact]
        public async Task GetAllGames_ShouldReturnOkResult_WithListOfGames()
        {
            // Arrange
            var games = new List<GameResponseDto>
            {
                new GameResponseDto { Id = 1, CreatedByName = "Game 1" },
                new GameResponseDto { Id = 2, CreatedByName = "Game 2" }
            };
            _mockGameService.Setup(x => x.GetAllGamesAsync()).ReturnsAsync(games);

            // Act
            var result = await _controller.GetAllGames();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<GameResponseDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        #endregion

        #region GetGameById Tests

        [Fact]
        public async Task GetGameById_ShouldReturnOkResult_WhenGameExists()
        {
            // Arrange
            var game = new GameResponseDto { Id = 1, CreatedByName = "Test Game" };
            _mockGameService.Setup(x => x.GetGameByIdAsync(1)).ReturnsAsync(game);

            // Act
            var result = await _controller.GetGameById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(game, okResult.Value);
        }

        [Fact]
        public async Task GetGameById_ShouldReturnNotFound_WhenGameDoesNotExist()
        {
            // Arrange
            _mockGameService.Setup(x => x.GetGameByIdAsync(1)).ReturnsAsync((GameResponseDto)null);

            // Act
            var result = await _controller.GetGameById(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        #endregion

        #region CreateGame Tests

        [Fact]
        public async Task CreateGame_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");
            var gameDto = new GameCreateDto();

            // Act
            var result = await _controller.CreateGame(gameDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateGame_ShouldReturnForbid_WhenUserIsNotAdmin()
        {
            // Arrange
            var gameDto = new GameCreateDto();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "test"),
                new Claim(ClaimTypes.Role, UserRole.User.ToString())
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            _mockUserService.Setup(x => x.GetCurrentUserAsync(user))
                .ReturnsAsync(new FIAPCloudGames.Models.Entities.User { Role = UserRole.User });

            // Act
            var result = await _controller.CreateGame(gameDto);

            // Assert
            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task CreateGame_ShouldReturnCreated_WhenSuccessful()
        {
            // Arrange
            var gameDto = new GameCreateDto { Title = "New Game" };
            var createdGame = new GameResponseDto { Id = 1, CreatedByName = "New Game" };

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Role, UserRole.Administrator.ToString())
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            _mockUserService.Setup(x => x.GetCurrentUserAsync(user))
                .ReturnsAsync(new FIAPCloudGames.Models.Entities.User { Role = UserRole.Administrator });

            _mockGameService.Setup(x => x.CreateGameAsync(gameDto)).ReturnsAsync(createdGame);

            // Act
            var result = await _controller.CreateGame(gameDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(_controller.GetGameById), createdAtActionResult.ActionName);
            Assert.Equal(createdGame, createdAtActionResult.Value);
        }

        #endregion

        #region UpdateGame Tests

        [Fact]
        public async Task UpdateGame_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var gameDto = new GameUpdateDto { Id = 1, Title = "Updated Game" };
            _mockGameService.Setup(x => x.UpdateGameAsync(gameDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateGame(gameDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateGame_ShouldReturnNotFound_WhenGameDoesNotExist()
        {
            // Arrange
            var gameDto = new GameUpdateDto { Id = 1, Title = "Updated Game" };
            _mockGameService.Setup(x => x.UpdateGameAsync(gameDto)).ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.UpdateGame(gameDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var value = notFoundResult.Value;
            var message = value.GetType().GetProperty("message").GetValue(value, null);
            Assert.Equal("Jogo não encontrado", message);
        }

        [Fact]
        public async Task UpdateGame_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var gameDto = new GameUpdateDto { Id = 1, Title = "Updated Game" };
            _mockGameService.Setup(x => x.UpdateGameAsync(gameDto)).ThrowsAsync(new Exception("Error message"));

            // Act
            var result = await _controller.UpdateGame(gameDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = badRequestResult.Value;
            var message = value.GetType().GetProperty("message").GetValue(value, null);
            Assert.Equal("Error message", message);
        }

        #endregion

        #region UpdateGamePrice Tests

        [Fact]
        public async Task UpdateGamePrice_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var updateDto = new UpdateGamePriceDto { Id = 1, NewPrice = 59.99m };
            _mockGameService.Setup(x => x.GetGameByIdAsync(1)).ReturnsAsync(new GameResponseDto());
            _mockGameService.Setup(x => x.UpdateGamePriceAsync(1, 59.99m)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateGamePrice(updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateGamePrice_ShouldReturnNotFound_WhenGameDoesNotExist()
        {
            // Arrange
            var updateDto = new UpdateGamePriceDto { Id = 1, NewPrice = 59.99m };
            _mockGameService.Setup(x => x.GetGameByIdAsync(1)).ReturnsAsync((GameResponseDto)null);

            // Act
            var result = await _controller.UpdateGamePrice(updateDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var value = notFoundResult.Value as dynamic;
            string message = value.GetType().GetProperty("message").GetValue(value, null);
            Assert.Equal("Jogo não encontrado", message);
        }

        #endregion

        #region DeleteGame Tests

        [Fact]
        public async Task DeleteGame_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            _mockGameService.Setup(x => x.DeleteGameAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteGame(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteGame_ShouldReturnNotFound_WhenGameDoesNotExist()
        {
            // Arrange
            _mockGameService.Setup(x => x.DeleteGameAsync(1)).ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.DeleteGame(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var value = notFoundResult.Value;
            var message = value.GetType().GetProperty("message").GetValue(value, null);
            Assert.Equal("Jogo não encontrado", message);
        }

        [Fact]
        public async Task DeleteGame_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            _mockGameService.Setup(x => x.DeleteGameAsync(1)).ThrowsAsync(new Exception("Error message"));

            // Act
            var result = await _controller.DeleteGame(1);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = badRequestResult.Value as dynamic;
            string message = value.GetType().GetProperty("message").GetValue(value, null);
            Assert.Equal("Error message", message);
        }

        #endregion
    }
}