using Hospital_Management_System.Domain.Enums;

namespace Hospital_Management_System.Application.Features.Billing.DTOs
{
    public class InvoiceFilterDto // فلاتر القايمة
    {
        public int? PatientId { get; set; } // فواتير مريض
        public int? AppointmentId { get; set; } // فاتورة حجز
        public PaymentStatus? Status { get; set; } // حالة الدفع
    }

}
