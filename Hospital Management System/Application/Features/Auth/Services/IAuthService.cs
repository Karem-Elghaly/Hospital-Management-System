using Hospital_Management_System.Application.Features.Auth.DTOs;

namespace Hospital_Management_System.Application.Features.Auth.Services
{
    public interface IAuthService 
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto); 

        Task<AuthResponseDto> LoginAsync(LoginDto dto); 

        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto); 

        Task LogoutAsync(RefreshTokenDto dto);
    }
}
