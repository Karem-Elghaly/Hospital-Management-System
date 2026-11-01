using FluentValidation;
using Hospital_Management_System.Application.Features.Departments.DTOs;

namespace Hospital_Management_System.Application.Features.Departments.Validators
{
    public class CreateDepartmentDtoValidator : AbstractValidator<CreateDepartmentDto> // كل Validator بيرث من AbstractValidator<نوع الـ DTO>
    {
        public CreateDepartmentDtoValidator() // القواعد بتتكتب جوه الـ Constructor
        {
            RuleFor(x => x.Name) // بنبدأ قواعد حقل الاسم
                .NotEmpty().WithMessage("Department name is required.") // مينفعش يبقى فاضي أو null أو مسافات، والـ WithMessage بتحدد رسالة الخطأ
                .MaximumLength(100).WithMessage("Department name must not exceed 100 characters."); // أقصى طول 100 حرف

            RuleFor(x => x.Description) // قواعد حقل الوصف
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters."); // أقصى طول 500، ومفيش NotEmpty لأنه اختياري (الـ null بيعدي)
        }
    }
}
