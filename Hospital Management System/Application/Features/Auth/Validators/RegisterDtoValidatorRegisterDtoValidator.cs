using FluentValidation; 
using Hospital_Management_System.Application.Features.Auth.DTOs;

namespace Hospital_Management_System.Application.Features.Auth.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto> 
    {
        public RegisterDtoValidator() 
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.") 
                .MaximumLength(150).WithMessage("Full name must not exceed 150 characters."); 

            RuleFor(x => x.Email) 
                .NotEmpty().WithMessage("Email is required.") 
                .EmailAddress().WithMessage("Email format is not valid.") 
                .MaximumLength(256).WithMessage("Email must not exceed 256 characters."); 

            RuleFor(x => x.Password) 
                .NotEmpty().WithMessage("Password is required.") 
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.") 
                .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.") 
                .Matches("[a-z]").WithMessage("Password must contain a lowercase letter.") 
                .Matches("[0-9]").WithMessage("Password must contain a digit."); 

            RuleFor(x => x.ConfirmPassword) 
                .Equal(x => x.Password).WithMessage("Passwords do not match."); 
        }
    }
}
