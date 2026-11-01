using FluentValidation; 
using Hospital_Management_System.Application.Features.MedicalRecords.DTOs;

namespace Hospital_Management_System.Application.Features.MedicalRecords.Validators; 

public class CreateMedicalRecordDtoValidator : AbstractValidator<CreateMedicalRecordDto> 
{
    public CreateMedicalRecordDtoValidator()
    {
        RuleFor(x => x.AppointmentId) 
            .GreaterThan(0).WithMessage("Appointment is required.");

        RuleFor(x => x.Symptoms)
            .NotEmpty().WithMessage("Symptoms are required.") 
            .MaximumLength(1000).WithMessage("Symptoms must not exceed 1000 characters."); 

        RuleFor(x => x.Diagnosis) 
            .NotEmpty().WithMessage("Diagnosis is required.") 
            .MaximumLength(1000).WithMessage("Diagnosis must not exceed 1000 characters."); 

        RuleFor(x => x.Notes) 
            .MaximumLength(2000).WithMessage("Notes must not exceed 2000 characters."); 
    }
}