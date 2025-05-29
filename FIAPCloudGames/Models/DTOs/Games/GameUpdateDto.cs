using System;
using System.ComponentModel.DataAnnotations;

namespace FIAPCloudGames.Models.DTOs.Games
{
    public class GameUpdateDto
    {
        public int Id { get; internal set; }
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O título deve ter entre 3 e 100 caracteres")]
        public string? Title { get; set; }

        [StringLength(500, MinimumLength = 10, ErrorMessage = "A descrição deve ter entre 10 e 500 caracteres")]
        public string? Description { get; set; }

        [Range(0.01, 1000, ErrorMessage = "O preço deve estar entre 0.01 e 1000")]
        public decimal? Price { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ReleaseDate { get; set; }

        [StringLength(50, ErrorMessage = "O gênero deve ter no máximo 50 caracteres")]
        public string? Genre { get; set; }
    }
}