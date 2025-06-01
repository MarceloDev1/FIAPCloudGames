using Microsoft.AspNetCore.Mvc;
using FIAPCloudGames.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using FIAPCloudGames.Models.DTOs.Users;
using FIAPCloudGames.Models.Enums;
using FIAPCloudGames.Services.Interfaces;
using System.Security.Claims;

namespace FIAPCloudGames.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Status()
        {
            return Ok(new { message = "API de autenticação operacional", status = "Online" });
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                // Por padrão, novos usuários são registrados como User (não Admin)
                request.Role = UserRole.User;

                var user = await _userService.CreateUserAsync(new UserCreateDto
                {
                    Name = request.Name,
                    Email = request.Email,
                    Password = request.Password,
                    ConfirmPassword = request.ConfirmPassword,
                    Role = request.Role
                });

                var authResponse = await _authService.AuthenticateAsync(new LoginRequest
                {
                    Email = request.Email,
                    Password = request.Password
                });

                return CreatedAtAction(nameof(Register), authResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var authResponse = await _authService.AuthenticateAsync(request);

                if (authResponse == null)
                    return Unauthorized(new { message = "Credenciais inválidas" });

                return Ok(authResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var user = await _userService.GetUserByIdAsync(userId);

                return Ok(new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.Role
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}