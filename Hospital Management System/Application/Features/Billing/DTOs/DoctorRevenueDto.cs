namespace Hospital_Management_System.Application.Features.Billing.DTOs
{
    public class DoctorRevenueDto // إيرادات دكتور
    {
        public int DoctorId { get; set; } // رقم الدكتور
        public string DoctorName { get; set; } = string.Empty; // اسمه
        public int InvoiceCount { get; set; } // عدد الفواتير
        public decimal TotalBilled { get; set; } // المفوتر
        public decimal TotalCollected { get; set; } // المحصّل
    }
}
