using TaskManagerAPI.DTOs.Auth;

namespace TaskManagerAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> Register(RegisterDto dto);
        Task<AuthResponseDto> Login(LoginDto dto);
        Task<AuthResponseDto?>  RefreshToken(string refreshToken);
    }
}
