using FluentValidation; 
using Hospital_Management_System.Application.Features.Doctors.DTOs;

namespace Hospital_Management_System.Application.Features.Doctors.Validators; 

public class CreateDoctorScheduleDtoValidator : AbstractValidator<CreateDoctorScheduleDto> 
{
    public CreateDoctorScheduleDtoValidator() 
    {
        RuleFor(x => x.DayOfWeek) 
            .IsInEnum().WithMessage("Day of week is not valid."); 

        RuleFor(x => x.EndTime) 
            .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time."); 

        RuleFor(x => x.SlotDurationMinutes) 
            .InclusiveBetween(10, 120).WithMessage("Slot duration must be between 10 and 120 minutes.");

        RuleFor(x => x) 
            .Must(x => (x.EndTime - x.StartTime).TotalMinutes >= x.SlotDurationMinutes) 
            .When(x => x.EndTime > x.StartTime) 
            .WithMessage("The working window must fit at least one slot."); 
    }
}