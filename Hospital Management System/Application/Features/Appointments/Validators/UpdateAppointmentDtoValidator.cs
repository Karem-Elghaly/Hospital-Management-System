using FluentValidation; 
using Hospital_Management_System.Application.Common.Utilities; 
using Hospital_Management_System.Application.Features.Appointments.DTOs;

namespace Hospital_Management_System.Application.Features.Appointments.Validators
{
    public class UpdateAppointmentDtoValidator : AbstractValidator<UpdateAppointmentDto> 
    {
        public UpdateAppointmentDtoValidator() 
        {
            RuleFor(x => x.AppointmentDate) 
                .Must(d => d >= ClinicClock.Today) 
                .WithMessage("Appointment date cannot be in the past.") 
                .Must(d => d <= ClinicClock.Today.AddDays(90)) 
                .WithMessage("Appointments can only be booked up to 90 days ahead."); 

            RuleFor(x => x.Notes) 
                .MaximumLength(500).WithMessage("Notes must not exceed 500 characters."); 
        }
    }
}
