using FIAPCloudGames.Models.DTOs.Games;

namespace FIAPCloudGames.Services.Interfaces
{
    public interface IGameService
    {

        Task<IEnumerable<GameResponseDto>> GetAllGamesAsync();
        Task<GameResponseDto> GetGameByIdAsync(int id);
        Task<GameResponseDto> CreateGameAsync(GameCreateDto gameDto);
        Task UpdateGameAsync(GameUpdateDto gameDto);
        Task DeleteGameAsync(int id);
        Task UpdateGamePriceAsync(int id, decimal novoPreco);
    }
}