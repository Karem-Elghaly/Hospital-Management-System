using FluentValidation; 
using Hospital_Management_System.Application.Features.Patients.DTOs;

namespace Hospital_Management_System.Application.Features.Patients.Validators
{
    public class UpdatePatientDtoValidator : AbstractValidator<UpdatePatientDto> 
    {
        public UpdatePatientDtoValidator() 
        {
            RuleFor(x => x.FullName) 
                .NotEmpty().WithMessage("Full name is required.") 
                .MaximumLength(150).WithMessage("Full name must not exceed 150 characters."); 

            RuleFor(x => x.DateOfBirth) 
                .Must(d => d <= DateOnly.FromDateTime(DateTime.UtcNow)) 
                .WithMessage("Date of birth cannot be in the future.") 
                .Must(d => d >= DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-120)) 
                .WithMessage("Date of birth is not valid.");

            RuleFor(x => x.Gender) 
                .IsInEnum().WithMessage("Gender value is not valid."); 

            RuleFor(x => x.BloodType) 
                .IsInEnum().When(x => x.BloodType.HasValue) 
                .WithMessage("Blood type value is not valid."); 

            RuleFor(x => x.Phone) 
                .NotEmpty().WithMessage("Phone is required.") 
                .Matches(@"^01[0125]\d{8}$").WithMessage("Phone must be a valid Egyptian mobile number (11 digits)."); 

            RuleFor(x => x.Address)
                .MaximumLength(300).WithMessage("Address must not exceed 300 characters."); 
        }
    }
}
