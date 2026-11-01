using FluentValidation; 
using Hospital_Management_System.Domain.Enums;
using Hospital_Management_System.Application.Features.Appointments.DTOs;

namespace Hospital_Management_System.Application.Features.Appointments.Validators; 

public class ChangeAppointmentStatusDtoValidator : AbstractValidator<ChangeAppointmentStatusDto> 
{
    public ChangeAppointmentStatusDtoValidator() 
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status value is not valid.") 
            .NotEqual(AppointmentStatus.Pending).WithMessage("An appointment cannot be moved back to Pending."); 
    }
}