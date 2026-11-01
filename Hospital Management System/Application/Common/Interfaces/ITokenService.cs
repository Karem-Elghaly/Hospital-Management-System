using Hospital_Management_System.Domain.Entities;
namespace Hospital_Management_System.Application.Common.Interfaces
{
    public interface ITokenService 
    {
        (string Token, DateTime ExpiresAt) GenerateAccessToken(string userId, string email, IEnumerable<string> roles);

        RefreshToken GenerateRefreshToken(string userId); 
    }
}
