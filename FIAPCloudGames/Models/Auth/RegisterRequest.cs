using FIAPCloudGames.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace FIAPCloudGames.Models.Auth
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email em formato inválido")]
        [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "As senhas não coincidem")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        public UserRole Role { get; set; }
    }
}