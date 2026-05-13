using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagerAPI.DTOs.Auth;
using TaskManagerAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagerAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var response = await _authService.Register(dto);

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var response = await _authService.Login(dto);
            if (response == null)
            {
                return Unauthorized("Invalid credentials");
            }

            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
        {
            var response = await _authService.RefreshToken(dto.RefreshToken);
            if (response == null)
            {
                return Unauthorized(new { error = "Invalid refresh token" });
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenDto dto)
        {
            var success = await _authService.Logout(dto.RefreshToken);
            if (!success)
            {
                return Unauthorized(new { error = "Invalid refresh token" });
            }

            return Ok(new { message = "Logged out successfully" });
        }
    }
}
