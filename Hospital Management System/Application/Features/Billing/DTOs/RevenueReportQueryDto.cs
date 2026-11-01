namespace Hospital_Management_System.Application.Features.Billing.DTOs
{
    public class RevenueReportQueryDto // مدخلات التقرير
    {
        public DateOnly From { get; set; } // من تاريخ
        public DateOnly To { get; set; } // إلى تاريخ
    }

}
