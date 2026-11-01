using FluentValidation; 
using Hospital_Management_System.Application.Features.Doctors.DTOs;

namespace Hospital_Management_System.Application.Features.Doctors.Validators; 

public class UpdateDoctorDtoValidator : AbstractValidator<UpdateDoctorDto>
{
    public UpdateDoctorDtoValidator() 
    {
        RuleFor(x => x.FullName) 
            .NotEmpty().WithMessage("Full name is required.") 
            .MaximumLength(150).WithMessage("Full name must not exceed 150 characters."); 

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