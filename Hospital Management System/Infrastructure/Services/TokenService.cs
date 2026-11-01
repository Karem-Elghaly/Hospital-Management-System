using System.IdentityModel.Tokens.Jwt; 
using System.Security.Claims; 
using System.Security.Cryptography; 
using System.Text; 
using Hospital_Management_System.Application.Common.Interfaces; 
using Hospital_Management_System.Domain.Entities; 
using Hospital_Management_System.Infrastructure.Identity; 
using Microsoft.Extensions.Options; 
using Microsoft.IdentityModel.Tokens; 


namespace Hospital_Management_System.Infrastructure.Services
{

    public class TokenService : ITokenService 
    {
        private readonly JwtSettings _settings; 

        public TokenService(IOptions<JwtSettings> options) 
        {
            _settings = options.Value; 
        }

        public (string Token, DateTime ExpiresAt) GenerateAccessToken(string userId, string email, IEnumerable<string> roles) 
        {
            var claims = new List<Claim> 
        {
            new(ClaimTypes.NameIdentifier, userId), 
            new(ClaimTypes.Email, email), 
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) 
        };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role))); 

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); 

            var expiresAt = DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes); 

            var token = new JwtSecurityToken( 
                issuer: _settings.Issuer, 
                audience: _settings.Audience,
                claims: claims, 
                expires: expiresAt, 
                signingCredentials: credentials); 

            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt); 
        }

        public RefreshToken GenerateRefreshToken(string userId) 
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64); 

            return new RefreshToken 
            {
                UserId = userId,
                Token = Convert.ToBase64String(randomBytes), 
                ExpiresAt = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpirationDays) 
            };
        }
    }
}
