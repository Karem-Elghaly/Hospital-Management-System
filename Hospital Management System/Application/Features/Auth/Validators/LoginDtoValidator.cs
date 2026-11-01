using FluentValidation; 
using Hospital_Management_System.Application.Features.Auth.DTOs;

namespace Hospital_Management_System.Application.Features.Auth.Validators
{

    public class LoginDtoValidator : AbstractValidator<LoginDto> 
    {
        public LoginDtoValidator() 
        {
            RuleFor(x => x.Email) 
                .NotEmpty().WithMessage("Email is required.") 
                .EmailAddress().WithMessage("Email format is not valid."); 

            RuleFor(x => x.Password) 
                .NotEmpty().WithMessage("Password is required."); 
        }
    }
}
