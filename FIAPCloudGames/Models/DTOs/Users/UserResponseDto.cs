using FIAPCloudGames.Models.Enums;
namespace FIAPCloudGames.Models.DTOs.Users
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public UserRole Role { get; set; }
    }
}