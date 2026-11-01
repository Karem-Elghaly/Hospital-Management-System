using Hospital_Management_System.Application.Features.Billing.DTOs; // عشان InvoiceDto
using Hospital_Management_System.Domain.Entities; // عشان Invoice
using Mapster;

namespace Hospital_Management_System.Application.Features.Billing.Service; // namespace الـ Feature

public class InvoiceMappingConfig : IRegister // Mapster بيلاقيه لوحده
{
    public void Register(TypeAdapterConfig config) // التحويلات المخصصة
    {
        config.NewConfig<Invoice, InvoiceDto>() // من الـ Entity للـ DTO
            .Map(dest => dest.AppointmentDate, src => src.Appointment.AppointmentDate); // تاريخ الكشف من الحجز
    }
}