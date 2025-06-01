using FIAPCloudGames.Data;
using FIAPCloudGames.Models.DTOs.Games;
using FIAPCloudGames.Models.Entities;
using FIAPCloudGames.Services;
using FIAPCloudGames.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests.Services
{
    public class GameServiceTests
    {
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;

        public GameServiceTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "GameServiceTestDB")
                .Options;
        }

        [Fact]
        public async Task CreateGameAsync_ShouldCreateNewGame()
        {
            // Arrange
            using (var context = new AppDbContext(_dbContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Users.Add(new User 
                { 
                    Id = 1, 
                    Name = "Marcelo", 
                    Email = "marcelo@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                    RefreshToken = string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    Role = UserRole.User
                });
                await context.SaveChangesAsync();

                var service = new GameService(context);
                var gameDto = new GameCreateDto
                {
                    Title = "Novo Jogo",
                    Description = "Descrição do jogo",
                    Price = 199.99m,
                    ReleaseDate = DateTime.Now.AddDays(-30),
                    Genre = "Ação",
                    CreatedById = 1 // Garante que corresponde ao usuário criado
                };

                // Act
                var result = await service.CreateGameAsync(gameDto);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(gameDto.Title, result.Title);
                Assert.Equal(gameDto.Description, result.Description);
                Assert.Equal(gameDto.Price, result.Price);
                Assert.True(result.Id > 0);

                var gameInDb = await context.Games
                    .Include(g => g.CreatedBy)
                    .FirstOrDefaultAsync(g => g.Id == result.Id);

                Assert.NotNull(gameInDb);
                Assert.Equal(gameDto.Title, gameInDb.Name);
                Assert.Equal(gameDto.CreatedById, gameInDb.CreatedById);
                Assert.NotNull(gameInDb.CreatedAt);
            }
        }

        [Fact]
        public async Task UpdateGameAsync_ShouldUpdateExistingGame()
        {
            // Arrange
            using (var context = new AppDbContext(_dbContextOptions))
            {
                var existingGame = new Game
                {
                    Name = "Jogo Antigo",
                    Description = "Descrição antiga",
                    Price = 99.99m,
                    ReleaseDate = DateTime.Now.AddDays(-60),
                    Genre = "Aventura",
                    CreatedById = 1,
                    CreatedAt = DateTime.UtcNow
                };
                context.Games.Add(existingGame);
                await context.SaveChangesAsync();

                var service = new GameService(context);
                var gameDto = new GameUpdateDto
                {
                    Id = existingGame.Id,
                    Title = "Jogo Atualizado",
                    Description = "Nova descrição",
                    Price = 149.99m,
                    ReleaseDate = DateTime.Now.AddDays(-30),
                    Genre = "RPG"
                };

                // Act
                await service.UpdateGameAsync(gameDto);

                // Assert
                var updatedGame = await context.Games.FindAsync(existingGame.Id);
                Assert.NotNull(updatedGame);
                Assert.Equal(gameDto.Title, updatedGame.Name);
                Assert.Equal(gameDto.Description, updatedGame.Description);
                Assert.Equal(gameDto.Price, updatedGame.Price);
                Assert.Equal(gameDto.Genre, updatedGame.Genre);
                Assert.NotNull(updatedGame.UpdatedAt);
            }
        }

        [Fact]
        public async Task UpdateGameAsync_WithNonExistentGame_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            using (var context = new AppDbContext(_dbContextOptions))
            {
                var service = new GameService(context);
                var gameDto = new GameUpdateDto { Id = 999 };

                // Act & Assert
                await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateGameAsync(gameDto));
            }
        }

        [Fact]
        public async Task UpdateGamePriceAsync_ShouldUpdatePrice()
        {
            // Arrange
            using (var context = new AppDbContext(_dbContextOptions))
            {
                var existingGame = new Game
                {
                    Name = "Jogo para Atualizar",
                    Price = 99.99m,
                    CreatedById = 1,
                    CreatedAt = DateTime.UtcNow,
                    Description = "Descrição do jogo",
                    ReleaseDate = DateTime.Now.AddDays(-30),
                    Genre = "Ação",
                    UpdatedAt = DateTime.UtcNow
                };
                context.Games.Add(existingGame);
                await context.SaveChangesAsync();

                var service = new GameService(context);
                var novoPreco = 149.99m;

                // Act
                await service.UpdateGamePriceAsync(existingGame.Id, novoPreco);

                // Assert
                var updatedGame = await context.Games.FindAsync(existingGame.Id);
                Assert.NotNull(updatedGame);
                Assert.Equal(novoPreco, updatedGame.Price);
                Assert.NotNull(updatedGame.UpdatedAt);
            }
        }

        [Fact]
        public async Task UpdateGamePriceAsync_WithNonExistentGame_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            using (var context = new AppDbContext(_dbContextOptions))
            {
                var service = new GameService(context);

                // Act & Assert
                await Assert.ThrowsAsync<KeyNotFoundException>(
                    () => service.UpdateGamePriceAsync(999, 199.99m));
            }
        }
    }
}