using Hospital_Management_System.Application.Common.Models; // عشان PagedResult و PaginationParams
using Hospital_Management_System.Application.Features.Billing.DTOs;
namespace Hospital_Management_System.Application.Features.Billing.Service; // namespace الـ Feature

public interface IBillingService // عقد الـ Service
{
    Task<PagedResult<InvoiceDto>> GetAllAsync(PaginationParams pagination, InvoiceFilterDto filter); // قايمة الفواتير
    Task<InvoiceDto> GetByIdAsync(int id); // فاتورة بالـ Id
    Task<InvoiceDto> CreateAsync(CreateInvoiceDto dto); // إنشاء فاتورة
    Task<InvoiceDto> PayAsync(int id, PayInvoiceDto dto); // تسجيل دفعة
    Task<RevenueReportDto> GetRevenueReportAsync(RevenueReportQueryDto query); // تقرير الإيرادات
}