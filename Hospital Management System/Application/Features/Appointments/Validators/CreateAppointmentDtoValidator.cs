using FluentValidation; // عشان AbstractValidator
using Hospital_Management_System.Application.Common.Utilities; 
using Hospital_Management_System.Application.Features.Appointments.DTOs;

namespace Hospital_Management_System.Application.Features.Appointments.Validators; // namespace فولدر الـ Validators

public class CreateAppointmentDtoValidator : AbstractValidator<CreateAppointmentDto> // Validator الحجز
{
    public CreateAppointmentDtoValidator() // القواعد
    {
        RuleFor(x => x.PatientId) // المريض
            .GreaterThan(0).When(x => x.PatientId.HasValue) // لو اتبعت لازم رقم صحيح
            .WithMessage("Patient id is not valid."); // رسالة الخطأ

        RuleFor(x => x.DoctorId) // الدكتور
            .GreaterThan(0).WithMessage("Doctor is required."); // مطلوب

        RuleFor(x => x.AppointmentDate) // التاريخ
            .Must(d => d >= ClinicClock.Today) // مش في الماضي (لو متبعتش خالص هيبقى 0001-01-01 فيقع هنا)
            .WithMessage("Appointment date cannot be in the past.") // رسالة الخطأ
            .Must(d => d <= ClinicClock.Today.AddDays(90)) // مش أبعد من 90 يوم
            .WithMessage("Appointments can only be booked up to 90 days ahead."); // رسالة الخطأ

        RuleFor(x => x.Notes) // الملاحظات
            .MaximumLength(500).WithMessage("Notes must not exceed 500 characters."); // أقصى 500 (نفس الـ Configuration)
    }
}