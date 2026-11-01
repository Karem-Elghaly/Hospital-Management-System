using FluentValidation; 
using Hospital_Management_System.Application.Features.Auth.DTOs;

namespace Hospital_Management_System.Application.Features.Auth.Validators
{

    public class RefreshTokenDtoValidator : AbstractValidator<RefreshTokenDto>
    {
        public RefreshTokenDtoValidator() 
        {
            RuleFor(x => x.RefreshToken) 
                .NotEmpty().WithMessage("Refresh token is required."); 
        }
    }
}
