using FIAPCloudGames.Models.Auth;
using FIAPCloudGames.Models.Entities;
using FIAPCloudGames.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FIAPCloudGames.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly JwtSettings _jwtSettings;

        public AuthService(IUserService userService, JwtSettings jwtSettings)
        {
            _userService = userService;
            _jwtSettings = jwtSettings;
        }

        public async Task<AuthResponse> AuthenticateAsync(LoginRequest request)
        {
            var user = await _userService.GetByEmailAsync(request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }

            var token = GenerateJwtToken(user);
            return new AuthResponse(user, token);
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public Task<AuthResponse> Authenticate(LoginRequest model)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResponse> RefreshToken(string token)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResponse> Register(RegisterRequest model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RevokeToken(string token)
        {
            throw new NotImplementedException();
        }
    }
}