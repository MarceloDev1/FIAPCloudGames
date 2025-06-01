namespace FIAPCloudGames.Models.DTOs.Games
{
    public class GameResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Genre { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedById { get; set; }
        public string CreatedByName { get; set; }
    }
}