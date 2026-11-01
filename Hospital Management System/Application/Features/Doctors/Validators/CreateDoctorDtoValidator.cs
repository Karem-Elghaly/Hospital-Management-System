using FluentValidation;
using Hospital_Management_System.Application.Features.Doctors.DTOs;

namespace Hospital_Management_System.Application.Features.Doctors.Validators; 

public class CreateDoctorDtoValidator : AbstractValidator<CreateDoctorDto> 
{
    public CreateDoctorDtoValidator() 
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

        RuleFor(x => x.Specialization) 
            .NotEmpty().WithMessage("Specialization is required.") 
            .MaximumLength(100).WithMessage("Specialization must not exceed 100 characters."); 

        RuleFor(x => x.Phone) 
            .NotEmpty().WithMessage("Phone is required.") 
            .Matches(@"^01[0125]\d{8}$").WithMessage("Phone must be a valid Egyptian mobile number (11 digits)."); 

        RuleFor(x => x.ConsultationFee) 
            .GreaterThan(0).WithMessage("Consultation fee must be greater than zero.")
            .LessThanOrEqualTo(100000).WithMessage("Consultation fee is not valid."); 

        RuleFor(x => x.DepartmentId) 
            .GreaterThan(0).WithMessage("Department is required."); 
    }
}