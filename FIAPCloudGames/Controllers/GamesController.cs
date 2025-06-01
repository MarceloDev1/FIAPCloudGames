using FIAPCloudGames.Data;
using FIAPCloudGames.Models.DTOs.Games;
using FIAPCloudGames.Models.Entities;
using FIAPCloudGames.Models.Enums;
using FIAPCloudGames.Services;
using FIAPCloudGames.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FIAPCloudGames.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class GamesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IGameService _gameService;
        private readonly IUserService _userService;


        public GamesController(AppDbContext context, IGameService gameService, IUserService userService)
        {
            _context = context;
            _gameService = gameService;
            _userService = userService;
        }

        /// <summary>
        /// Lista todos os jogos
        /// </summary>
        [HttpGet]
        [Route("GetAllGames")]
        public async Task<ActionResult<IEnumerable<GameResponseDto>>> GetAllGames()
        {
            var games = await _gameService.GetAllGamesAsync();
            return Ok(games);
        }

        /// <summary>
        /// Obtém um jogo pelo ID
        /// </summary>
        /// <param name="id">ID do jogo</param>
        [HttpGet]
        [Route("GetGameById/{id}")]
        public async Task<ActionResult<GameResponseDto>> GetGameById(int id)
        {
            var game = await _gameService.GetGameByIdAsync(id);

            if (game == null)
            {
                return NotFound(new { message = "Jogo não encontrado" });
            }

            return Ok(game);
        }

        /// <summary>
        /// Cria um novo jogo
        /// </summary>
        /// <param name="gameDto">Dados do novo jogo</param>
        [HttpPost]
        [Route("CreateGame")]
        [Authorize] // Requer autenticação
        public async Task<ActionResult<GameResponseDto>> CreateGame([FromBody] GameCreateDto gameDto)
        {            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var currentUser = await _userService.GetCurrentUserAsync(User);

                if (currentUser.Role != UserRole.Administrator)
                {
                    return Forbid(); // Retorna 403 - Proibido
                }
                var createdGame = await _gameService.CreateGameAsync(gameDto);
                return CreatedAtAction(nameof(GetGameById), new { id = createdGame.Id }, createdGame);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza um jogo existente
        /// </summary>
        /// <param name="gameDto">Dados atualizados do jogo</param>
        [HttpPut]
        [Route("UpdateGame")]
        public async Task<IActionResult> UpdateGame([FromBody] GameUpdateDto gameDto)
        {
            try
            {
                await _gameService.UpdateGameAsync(gameDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Jogo não encontrado" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza apenas o preço de um jogo
        /// </summary>
        /// <param name="updateDto">ID e novo preço</param>
        [HttpPatch]
        [Route("UpdateGamePrice")]
        public async Task<IActionResult> UpdateGamePrice([FromBody] UpdateGamePriceDto updateDto)
        {
            try
            {
                var game = await _gameService.GetGameByIdAsync(updateDto.Id);
                if (game == null)
                {
                    return NotFound(new { message = "Jogo não encontrado" });
                }

                game.Price = updateDto.NewPrice;
                await _gameService.UpdateGamePriceAsync(updateDto.Id, updateDto.NewPrice);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        /// <summary>
        /// Remove um jogo
        /// </summary>
        /// <param name="id">ID do jogo</param>
        [HttpDelete]
        [Route("DeleteGame")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            try
            {
                await _gameService.DeleteGameAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Jogo não encontrado" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}