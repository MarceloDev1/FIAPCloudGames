using FIAPCloudGames.Models.Entities;

namespace FIAPCloudGames.Models.Auth
{
    public class AuthResponse
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresIn { get; set; }

        public AuthResponse(User user, string token)
        {
            UserId = user.Id;
            Name = user.Name;
            Email = user.Email;
            Token = token;
            ExpiresIn = DateTime.UtcNow.AddHours(1); // Exemplo: token expira em 1 hora
        }
    }
}