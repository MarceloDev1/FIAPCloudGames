using FIAPCloudGames.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace FIAPCloudGames.Models.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        [Required]
        public UserRole Role { get; set; } = UserRole.User; // Valor padrão: User

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Relacionamentos (apenas Administradores podem criar games)
        public virtual ICollection<Game> GamesCreated { get; set; }
    }
}