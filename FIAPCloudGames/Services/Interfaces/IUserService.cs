using FIAPCloudGames.Models.DTOs.Users;
using FIAPCloudGames.Models.Entities;
using System.Security.Claims;

namespace FIAPCloudGames.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto> GetUserByIdAsync(int id);
        Task<UserResponseDto> CreateUserAsync(UserCreateDto userDto);
        Task UpdateUserAsync(UserUpdateDto userDto);
        Task DeleteUserAsync(int id);
        Task<User> GetCurrentUserAsync(ClaimsPrincipal user);
        Task<User> GetByEmailAsync(string email);
    }
}