using FluentValidation; // عشان AbstractValidator
using Hospital_Management_System.Application.Features.Billing.DTOs;

namespace Hospital_Management_System.Application.Features.Billing.Validators; // namespace الـ Validators

public class CreateInvoiceDtoValidator : AbstractValidator<CreateInvoiceDto> // Validator إنشاء فاتورة
{
    public CreateInvoiceDtoValidator() // القواعد
    {
        RuleFor(x => x.AppointmentId).GreaterThan(0).WithMessage("Appointment is required."); // الحجز مطلوب

        RuleFor(x => x.AdditionalAmount) // المبلغ الإضافي
            .InclusiveBetween(0, 100000).WithMessage("Additional amount must be between 0 and 100000.") // من صفر لحد أقصى
            .Must(a => decimal.Round(a, 2) == a).WithMessage("Additional amount can have at most 2 decimal places."); // رقمين عشريين بس

        RuleFor(x => x.Description).MaximumLength(500).WithMessage("Description must not exceed 500 characters."); // أقصى 500
    }
}

public class PayInvoiceDtoValidator : AbstractValidator<PayInvoiceDto> // Validator الدفع
{
    public PayInvoiceDtoValidator() // القواعد
    {
        RuleFor(x => x.Amount) // المبلغ
            .GreaterThan(0).WithMessage("Payment amount must be greater than zero.") // أكبر من صفر
            .LessThanOrEqualTo(1000000).WithMessage("Payment amount is not valid.") // حد أقصى
            .Must(a => decimal.Round(a, 2) == a).WithMessage("Payment amount can have at most 2 decimal places."); // رقمين عشريين بس
    }
}

public class RevenueReportQueryDtoValidator : AbstractValidator<RevenueReportQueryDto> // Validator التقرير
{
    public RevenueReportQueryDtoValidator() // القواعد
    {
        RuleFor(x => x.From).Must(d => d != default).WithMessage("From date is required."); // مطلوب

        RuleFor(x => x.To) // إلى
            .Must(d => d != default).WithMessage("To date is required.") // مطلوب
            .GreaterThanOrEqualTo(x => x.From).WithMessage("To date must not be before From date."); // مش قبل البداية

        RuleFor(x => x) // الفترة كلها
            .Must(x => x.To.DayNumber - x.From.DayNumber <= 366) // مش أكتر من سنة
            .When(x => x.From != default && x.To != default) // لو التواريخ اتبعتت
            .WithMessage("The report period cannot exceed 366 days."); // رسالة الخطأ
    }
}