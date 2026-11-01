using Hospital_Management_System.Domain.Enums;

namespace Hospital_Management_System.Application.Features.Billing.DTOs; // namespace فولدر الـ DTOs

public class InvoiceDto // الفاتورة الراجعة للـ Client
{
    public int Id { get; set; } // رقم الفاتورة
    public int AppointmentId { get; set; } // رقم الحجز
    public DateOnly AppointmentDate { get; set; } // تاريخ الكشف
    public int PatientId { get; set; } // رقم المريض
    public string PatientFullName { get; set; } = string.Empty; // اسم المريض
    public decimal TotalAmount { get; set; } // الإجمالي
    public decimal PaidAmount { get; set; } // المدفوع
    public decimal RemainingAmount { get; set; } // المتبقي
    public PaymentStatus Status { get; set; } // حالة الدفع
    public DateTime? PaidAt { get; set; } // وقت آخر دفعة
    public string? Description { get; set; } // الوصف
    public DateTime CreatedAt { get; set; } // وقت الإنشاء
}