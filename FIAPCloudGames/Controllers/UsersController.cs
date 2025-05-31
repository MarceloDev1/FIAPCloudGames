using Microsoft.AspNetCore.Mvc;
using FIAPCloudGames.Models.DTOs.Users;
using FIAPCloudGames.Services.Interfaces;

namespace FIAPCloudGames.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Lista todos os usuários
        /// </summary>
        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Obtém um usuário pelo ID
        /// </summary>
        /// <param name="id">ID do usuário</param>
        [HttpGet]
        [Route("GetUserById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            return Ok(user);
        }

        /// <summary>
        /// Cria um novo usuário
        /// </summary>
        /// <param name="userDto">Dados do novo usuário</param>
        [HttpPost]
        [Route("CreateUser")]
        public async Task<IActionResult> Create([FromBody] UserCreateDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdUser = await _userService.CreateUserAsync(userDto);
                return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza um usuário existente
        /// </summary>
        /// <param name="userDto">Dados atualizados do usuário</param>
        [HttpPut]
        [Route("UpdateUser")]
        public async Task<IActionResult> Update([FromBody] UserUpdateDto userDto)
        {
            try
            {
                await _userService.UpdateUserAsync(userDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Remove um usuário
        /// </summary>
        /// <param name="id">ID do usuário</param>
        [HttpDelete]
        [Route("DeleteUser")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}