using FluentValidation;
using Hospital_Management_System.Application.Features.Departments.DTOs;

namespace Hospital_Management_System.Application.Features.Departments.Validators
{
    public class UpdateDepartmentDtoValidator : AbstractValidator<UpdateDepartmentDto> // Validator للتعديل
    {
        public UpdateDepartmentDtoValidator() // الـ Constructor
        {
            RuleFor(x => x.Name) // قواعد الاسم
                .NotEmpty().WithMessage("Department name is required.") // مطلوب
                .MaximumLength(100).WithMessage("Department name must not exceed 100 characters."); // أقصى 100 حرف

            RuleFor(x => x.Description) // قواعد الوصف
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters."); // أقصى 500 حرف
        }
    }
}
