using FIAPCloudGames.Data;
using FIAPCloudGames.Models.DTOs.Games;
using FIAPCloudGames.Models.Entities;
using FIAPCloudGames.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FIAPCloudGames.Services
{
    public class GameService : IGameService
    {
        private readonly AppDbContext _context;

        public GameService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GameResponseDto>> GetAllGamesAsync()
        {
            return await _context.Games
                .Include(g => g.CreatedBy)
                .Select(g => new GameResponseDto
                {
                    Id = g.Id,
                    Title = g.Name,
                    Description = g.Description,
                    Price = g.Price,
                    ReleaseDate = g.ReleaseDate,
                    Genre = g.Genre,
                    CreatedAt = g.CreatedAt,
                    CreatedById = g.CreatedById,
                    CreatedByName = g.CreatedBy.Name
                })
                .ToListAsync();
        }

        public async Task<GameResponseDto> GetGameByIdAsync(int id)
        {
            var game = await _context.Games
                .Include(g => g.CreatedBy)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null) return null;

            return new GameResponseDto
            {
                Id = game.Id,
                Title = game.Name,
                Description = game.Description,
                Price = game.Price,
                ReleaseDate = game.ReleaseDate,
                Genre = game.Genre,
                CreatedAt = game.CreatedAt,
                CreatedById = game.CreatedById,
                CreatedByName = game.CreatedBy?.Name
            };
        }

        public async Task<GameResponseDto> CreateGameAsync(RegisterRequest gameDto)
        {
            var game = new Game
            {
                Name = gameDto.Title,
                Description = gameDto.Description,
                Price = gameDto.Price,
                ReleaseDate = gameDto.ReleaseDate,
                Genre = gameDto.Genre,
                CreatedById = gameDto.CreatedById,
                CreatedAt = DateTime.UtcNow
            };

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return await GetGameByIdAsync(game.Id);
        }

        public async Task UpdateGameAsync(int id, GameUpdateDto gameDto)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null) throw new KeyNotFoundException();

            game.Name = gameDto.Title ?? game.Name;
            game.Description = gameDto.Description ?? game.Description;
            game.Price = gameDto.Price ?? game.Price;
            game.ReleaseDate = gameDto.ReleaseDate ?? game.ReleaseDate;
            game.Genre = gameDto.Genre ?? game.Genre;
            game.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteGameAsync(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null) throw new KeyNotFoundException();

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
        }
    }
}