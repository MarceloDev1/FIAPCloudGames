namespace FIAPCloudGames.Models.DTOs.Users
{
    public class UserListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int TotalGamesCreated { get; set; }
    }
}