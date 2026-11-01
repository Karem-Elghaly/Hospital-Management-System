using FluentValidation;
using Hospital_Management_System.Application.Common.Constants;
using Hospital_Management_System.Application.Common.Interfaces; 
using Hospital_Management_System.Application.Common.Models;
using Hospital_Management_System.Application.Features.Auth.DTOs;
using Hospital_Management_System.Domain.Entities; 
using System.Data;
using System.Linq;
using UnauthorizedException = Hospital_Management_System.Application.Common.Exceptions.UnauthorizedException; 
using ValidationException = Hospital_Management_System.Application.Common.Exceptions.ValidationException; 

namespace Hospital_Management_System.Application.Features.Auth.Services
{

    public class AuthService : IAuthService 
    {
        private readonly IUnitOfWork _unitOfWork; 
        private readonly IIdentityService _identityService; 
        private readonly ITokenService _tokenService; 
        private readonly IValidator<RegisterDto> _registerValidator; 
        private readonly IValidator<LoginDto> _loginValidator; 
        private readonly IValidator<RefreshTokenDto> _refreshValidator; 

        public AuthService( 
            IUnitOfWork unitOfWork, 
            IIdentityService identityService, 
            ITokenService tokenService, 
            IValidator<RegisterDto> registerValidator, 
            IValidator<LoginDto> loginValidator, 
            IValidator<RefreshTokenDto> refreshValidator) 
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService; 
            _tokenService = tokenService; 
            _registerValidator = registerValidator; 
            _loginValidator = loginValidator; 
            _refreshValidator = refreshValidator; 
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto) 
        {
            await ValidateAsync(_registerValidator, dto); 

            var user = await _identityService.CreateUserAsync(dto.FullName, dto.Email, dto.Password, Roles.Patient); 

            var response = await IssueTokensAsync(user); 
            await _unitOfWork.SaveChangesAsync(); 

            return response; 
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto) 
        {
            await ValidateAsync(_loginValidator, dto); 

            var user = await _identityService.ValidateCredentialsAsync(dto.Email, dto.Password) 
                ?? throw new UnauthorizedException("Invalid email or password."); 

            var response = await IssueTokensAsync(user); 
            await _unitOfWork.SaveChangesAsync();

            return response; 
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
        {
            await ValidateAsync(_refreshValidator, dto);

            var stored = await FindTokenAsync(dto.RefreshToken) 
                ?? throw new UnauthorizedException("Invalid refresh token."); 

            if (stored.IsRevoked) 
            {
                await RevokeAllUserTokensAsync(stored.UserId); 
                throw new UnauthorizedException("Refresh token was already used. Please log in again.");
            }

            if (stored.IsExpired) 
                throw new UnauthorizedException("Refresh token has expired. Please log in again."); 

            var user = await _identityService.GetUserByIdAsync(stored.UserId)
                ?? throw new UnauthorizedException("User no longer exists.");

            var response = await IssueTokensAsync(user);

            stored.RevokedAt = DateTime.UtcNow; 
            stored.ReplacedByToken = response.RefreshToken; 
            _unitOfWork.RefreshTokens.Update(stored); 

            await _unitOfWork.SaveChangesAsync(); 

            return response; 
        }

        public async Task LogoutAsync(RefreshTokenDto dto)
        {
            await ValidateAsync(_refreshValidator, dto); 

            var stored = await FindTokenAsync(dto.RefreshToken); 
            if (stored is null || !stored.IsActive) return; 

            stored.RevokedAt = DateTime.UtcNow;
            _unitOfWork.RefreshTokens.Update(stored);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<AuthResponseDto> IssueTokensAsync(UserInfo user) 
        {
            var access = _tokenService.GenerateAccessToken(user.Id, user.Email, user.Roles);
            var refresh = _tokenService.GenerateRefreshToken(user.Id); 

            await _unitOfWork.RefreshTokens.AddAsync(refresh); 

            return new AuthResponseDto 
            {
                UserId = user.Id,
                Email = user.Email, 
                FullName = user.FullName, 
                Roles = user.Roles,
                AccessToken = access.Token, 
                AccessTokenExpiresAt = access.ExpiresAt, 
                RefreshToken = refresh.Token 
            };
        }

        private async Task<RefreshToken?> FindTokenAsync(string token) 
        {
            var tokens = await _unitOfWork.RefreshTokens.GetAllAsync(t => t.Token == token); 
            return tokens.FirstOrDefault();
        }

        private async Task RevokeAllUserTokensAsync(string userId) 
        {
            var active = await _unitOfWork.RefreshTokens.GetAllAsync(
                t => t.UserId == userId && t.RevokedAt == null && t.ExpiresAt > DateTime.UtcNow);

            foreach (var token in active) 
            {
                token.RevokedAt = DateTime.UtcNow; 
                _unitOfWork.RefreshTokens.Update(token); 
            }

            await _unitOfWork.SaveChangesAsync(); 
        }

        private static async Task ValidateAsync<T>(IValidator<T> validator, T dto) 
        {
            var result = await validator.ValidateAsync(dto); 
            if (!result.IsValid) 
                throw new ValidationException(result.Errors); 
        }
    }

}