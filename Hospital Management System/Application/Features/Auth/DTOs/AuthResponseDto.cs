namespace Hospital_Management_System.Application.Features.Auth.DTOs
{
    public class AuthResponseDto 
    {
        public string UserId { get; set; } = string.Empty; 

        public string Email { get; set; } = string.Empty; 

        public string FullName { get; set; } = string.Empty; 

        public IList<string> Roles { get; set; } = new List<string>(); 

        public string AccessToken { get; set; } = string.Empty; 

        public DateTime AccessTokenExpiresAt { get; set; } 

        public string RefreshToken { get; set; } = string.Empty; 
    }
}
