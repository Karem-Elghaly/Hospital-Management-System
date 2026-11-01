using FluentValidation; 
using Hospital_Management_System.Application.Features.MedicalRecords.DTOs;

namespace Hospital_Management_System.Application.Features.MedicalRecords.Validators; 

public class CreatePrescriptionItemDtoValidator : AbstractValidator<CreatePrescriptionItemDto> 
{
    public CreatePrescriptionItemDtoValidator() 
    {
        RuleFor(x => x.MedicineName) 
            .NotEmpty().WithMessage("Medicine name is required.") 
            .MaximumLength(150).WithMessage("Medicine name must not exceed 150 characters."); 

        RuleFor(x => x.Dosage) 
            .NotEmpty().WithMessage("Dosage is required.") 
            .MaximumLength(100).WithMessage("Dosage must not exceed 100 characters."); 

        RuleFor(x => x.Frequency) 
            .NotEmpty().WithMessage("Frequency is required.") 
            .MaximumLength(100).WithMessage("Frequency must not exceed 100 characters."); 

        RuleFor(x => x.DurationInDays) 
            .InclusiveBetween(1, 365).WithMessage("Duration must be between 1 and 365 days."); 

        RuleFor(x => x.Instructions) 
            .MaximumLength(500).WithMessage("Instructions must not exceed 500 characters."); 
    }
}