using FluentValidation; 
using Hospital_Management_System.Application.Features.MedicalRecords.DTOs;
using Hospital_Management_System.Application.Features.MedicalRecords.Validators;

namespace Hospital_Management_System.Application.Features.MedicalRecords.Validators;

public class CreatePrescriptionDtoValidator : AbstractValidator<CreatePrescriptionDto> 
{
    public CreatePrescriptionDtoValidator() 
    {
        RuleFor(x => x.GeneralInstructions) 
            .MaximumLength(1000).WithMessage("General instructions must not exceed 1000 characters."); 

        RuleFor(x => x.Items) 
            .NotEmpty().WithMessage("A prescription must contain at least one medicine.") 
            .Must(items => items.Count <= 20).WithMessage("A prescription cannot contain more than 20 medicines."); 

        RuleForEach(x => x.Items) 
            .SetValidator(new CreatePrescriptionItemDtoValidator());
    }
}