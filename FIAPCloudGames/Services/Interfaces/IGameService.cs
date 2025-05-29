using FIAPCloudGames.Models.DTOs.Games;

namespace FIAPCloudGames.Services.Interfaces
{
    public interface IGameService
    {

        Task<IEnumerable<GameResponseDto>> GetAllGamesAsync();
        Task<GameResponseDto> GetGameByIdAsync(int id);
        Task<GameResponseDto> CreateGameAsync(RegisterRequest gameDto);
        Task UpdateGameAsync(int id, GameUpdateDto gameDto);
        Task DeleteGameAsync(int id);

    }
}