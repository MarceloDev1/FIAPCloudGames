using FIAPCloudGames.Models.Auth;

namespace FIAPCloudGames.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> Authenticate(LoginRequest model);
        Task<AuthResponse> Register(RegisterRequest model);
        Task<AuthResponse> RefreshToken(string token);
        Task<bool> RevokeToken(string token);
        Task<AuthResponse> AuthenticateAsync(LoginRequest request);

    }
}