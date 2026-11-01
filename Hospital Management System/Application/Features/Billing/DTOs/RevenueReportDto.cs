namespace Hospital_Management_System.Application.Features.Billing.DTOs; // نفس الـ namespace

public class RevenueReportDto // تقرير الإيرادات
{
    public DateOnly From { get; set; } // بداية الفترة
    public DateOnly To { get; set; } // نهايتها
    public int InvoiceCount { get; set; } // عدد الفواتير
    public decimal TotalBilled { get; set; } // المفوتر
    public decimal TotalCollected { get; set; } // المحصّل
    public decimal TotalOutstanding { get; set; } // المتبقي
    public int PaidCount { get; set; } // المدفوعة كاملة
    public int PartialCount { get; set; } // المدفوعة جزئياً
    public int UnpaidCount { get; set; } // غير المدفوعة
    public List<DoctorRevenueDto> ByDoctor { get; set; } = new(); // تفصيل لكل دكتور
}