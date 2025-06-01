using FIAPCloudGames.Models.Auth;

namespace FIAPCloudGames.Services.Interfaces
{
    public interface IAuthService
    {
        
        Task<AuthResponse> AuthenticateAsync(LoginRequest request);
    }
}