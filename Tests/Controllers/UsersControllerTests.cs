using FIAPCloudGames.Controllers;
using FIAPCloudGames.Models.DTOs.Users;
using FIAPCloudGames.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;

namespace FIAPCloudGames.UnitTests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new UsersController(_mockUserService.Object);
        }

        #region GetAll Tests

        [Fact]
        public async Task GetAll_ShouldReturnOkResultWithUsers_WhenUsersExist()
        {
            // Arrange
            var mockUsers = new List<UserResponseDto>
            {
                new UserResponseDto { Id = 1, Name = "User 1", Email = "user1@test.com" },
                new UserResponseDto { Id = 2, Name = "User 2", Email = "user2@test.com" }
            };

            _mockUserService.Setup(x => x.GetAllUsersAsync()).ReturnsAsync(mockUsers);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedUsers.Count());
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkResultWithEmptyList_WhenNoUsersExist()
        {
            // Arrange
            _mockUserService.Setup(x => x.GetAllUsersAsync()).ReturnsAsync(new List<UserResponseDto>());

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserResponseDto>>(okResult.Value);
            Assert.Empty(returnedUsers);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_ShouldReturnOkResultWithUser_WhenUserExists()
        {
            // Arrange
            var mockUser = new UserResponseDto { Id = 1, Name = "Test User", Email = "test@test.com" };
            _mockUserService.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync(mockUser);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<UserResponseDto>(okResult.Value);
            Assert.Equal(mockUser.Id, returnedUser.Id);
            Assert.Equal(mockUser.Name, returnedUser.Name);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFoundResult_WhenUserDoesNotExist()
        {
            // Arrange
            int testId = 1;
            _mockUserService.Setup(x => x.GetUserByIdAsync(testId))
                           .ReturnsAsync((UserResponseDto)null);

            // Act
            var result = await _controller.GetById(testId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            Assert.NotNull(notFoundResult.Value);

            var message = (notFoundResult.Value.GetType().GetProperty("message")?.GetValue(notFoundResult.Value)) as string;
            Assert.Equal("Usuário não encontrado", message);

            var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                JsonConvert.SerializeObject(notFoundResult.Value));
            Assert.Equal("Usuário não encontrado", jsonResult["message"]);
        }

        [Fact]
        public async Task GetById_ShouldReturnBadRequestResult_WhenIdIsInvalid()
        {
            // Arrange
            var invalidId = 0;

            // Act
            var result = await _controller.GetById(invalidId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task Create_ShouldReturnCreatedAtActionResult_WhenUserIsValid()
        {
            // Arrange
            var userDto = new UserCreateDto { Name = "New User", Email = "new@test.com" };
            var createdUser = new UserResponseDto { Id = 1, Name = "New User", Email = "new@test.com" };

            _mockUserService.Setup(x => x.CreateUserAsync(userDto)).ReturnsAsync(createdUser);

            // Act
            var result = await _controller.Create(userDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(UsersController.GetById), createdAtActionResult.ActionName);
            Assert.Equal(createdUser.Id, createdAtActionResult.RouteValues["id"]);
            var returnedUser = Assert.IsType<UserResponseDto>(createdAtActionResult.Value);
            Assert.Equal(createdUser.Name, returnedUser.Name);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequestResult_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");
            var userDto = new UserCreateDto();

            // Act
            var result = await _controller.Create(userDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequestResult_WhenServiceThrowsException()
        {
            // Arrange
            var userDto = new UserCreateDto { Name = "New User", Email = "new@test.com" };
            _mockUserService.Setup(x => x.CreateUserAsync(userDto))
                           .ThrowsAsync(new Exception("Error message"));

            // Act
            var result = await _controller.Create(userDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(
            JsonConvert.SerializeObject(badRequestResult.Value));
            Assert.Equal("Error message", response["Message"]);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_ShouldReturnNoContentResult_WhenUpdateIsSuccessful()
        {
            // Arrange
            var userDto = new UserUpdateDto { Id = 1, Name = "Updated User", Email = "updated@test.com" };
            _mockUserService.Setup(x => x.UpdateUserAsync(userDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(userDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_ShouldReturnNotFoundResult_WhenUserDoesNotExist()
        {
            // Arrange
            var userDto = new UserUpdateDto { Id = 1, Name = "Updated User", Email = "updated@test.com" };
            _mockUserService.Setup(x => x.UpdateUserAsync(userDto)).ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.Update(userDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var json = System.Text.Json.JsonSerializer.Serialize(notFoundResult.Value);
            var response = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            Assert.Equal("Usuário não encontrado", response["Message"]);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequestResult_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");
            var userDto = new UserUpdateDto();

            // Act
            var result = await _controller.Update(userDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequestResult_WhenServiceThrowsException()
        {
            // Arrange
            var userDto = new UserUpdateDto
            {
                Id = 1,
                Name = "Updated User",
                Email = "updated@test.com"
            };

            _mockUserService.Setup(x => x.UpdateUserAsync(userDto))
                           .ThrowsAsync(new Exception("Error message"));

            // Act
            var result = await _controller.Update(userDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            var json = System.Text.Json.JsonSerializer.Serialize(badRequestResult.Value);
            var response = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            Assert.Equal("Error message", response["Message"]);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_ShouldReturnNoContentResult_WhenDeleteIsSuccessful()
        {
            // Arrange
            var userId = 1;
            _mockUserService.Setup(x => x.DeleteUserAsync(userId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(userId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFoundResult_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 1;
            _mockUserService.Setup(x => x.DeleteUserAsync(userId))
                           .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.Delete(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var responseJson = System.Text.Json.JsonSerializer.Serialize(notFoundResult.Value);
            var response = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(responseJson);

            Assert.Equal("Usuário não encontrado", response["message"] ?? response["Message"]);
        }

        [Fact]
        public async Task Delete_ShouldReturnBadRequestResult_WhenIdIsInvalid()
        {
            // Arrange
            var invalidId = 0;

            // Act
            var result = await _controller.Delete(invalidId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnBadRequestResult_WhenServiceThrowsException()
        {
            // Arrange
            var userId = 1;
            _mockUserService.Setup(x => x.DeleteUserAsync(userId)).ThrowsAsync(new Exception("Error message"));

            // Act
            var result = await _controller.Delete(userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var json = System.Text.Json.JsonSerializer.Serialize(badRequestResult.Value);
            var response = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            Assert.Equal("Error message", response["message"]); // lowercase 'message'
        }

        #endregion
    }
}