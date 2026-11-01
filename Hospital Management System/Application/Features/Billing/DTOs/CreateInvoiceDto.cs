namespace Hospital_Management_System.Application.Features.Billing.DTOs; // نفس الـ namespace

public class CreateInvoiceDto // بيانات إنشاء فاتورة
{
    public int AppointmentId { get; set; } // الحجز
    public decimal AdditionalAmount { get; set; } // مبلغ إضافي فوق سعر الكشف
    public string? Description { get; set; } // وصف اختياري
}