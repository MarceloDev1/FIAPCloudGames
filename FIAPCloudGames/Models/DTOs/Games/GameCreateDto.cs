using System;
using System.ComponentModel.DataAnnotations;

namespace FIAPCloudGames.Models.DTOs.Games
{
    public class GameCreateDto
    {
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O título deve ter entre 3 e 100 caracteres")]
        public string Title { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "A descrição deve ter entre 10 e 500 caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório")]
        [Range(0.01, 1000, ErrorMessage = "O preço deve estar entre 0.01 e 1000")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "A data de lançamento é obrigatória")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [Required(ErrorMessage = "O gênero é obrigatório")]
        [StringLength(50, ErrorMessage = "O gênero deve ter no máximo 50 caracteres")]
        public string Genre { get; set; }

        [Required(ErrorMessage = "O ID do criador é obrigatório")]
        public int CreatedById { get; set; }
    }
}