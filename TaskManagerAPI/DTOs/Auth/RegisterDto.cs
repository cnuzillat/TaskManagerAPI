namespace TaskManagerAPI.DTOs.Auth
{
    public class RegisterDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Username { get; set; } 
    }
}
