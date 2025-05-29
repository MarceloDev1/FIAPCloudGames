using System.ComponentModel.DataAnnotations;

namespace FIAPCloudGames.Models.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email em formato inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}