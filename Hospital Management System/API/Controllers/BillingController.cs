using Hospital_Management_System.Application.Common.Constants; 
using Hospital_Management_System.Application.Common.Models; 
using Hospital_Management_System.Application.Features.Billing; 
using Hospital_Management_System.Application.Features.Billing.DTOs; 
using Hospital_Management_System.Application.Features.Billing.Service;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc; 

namespace Hospital_Management_System.API.Controllers;

[ApiController] 
[Route("api/[controller]")] 
[Authorize] 
public class BillingController : ControllerBase 
{
    private readonly IBillingService _billingService; 

    public BillingController(IBillingService billingService) 
    {
        _billingService = billingService;
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Receptionist},{Roles.Patient}")] 
    [HttpGet("invoices")] 
    public async Task<ActionResult<Result<PagedResult<InvoiceDto>>>> GetInvoices([FromQuery] PaginationParams pagination, [FromQuery] InvoiceFilterDto filter) // قايمة
    {
        var result = await _billingService.GetAllAsync(pagination, filter); 
        return Ok(Result<PagedResult<InvoiceDto>>.Ok(result)); 
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Receptionist},{Roles.Patient}")] 
    [HttpGet("invoices/{id:int}")] 
    public async Task<ActionResult<Result<InvoiceDto>>> GetInvoiceById(int id) 
    {
        var result = await _billingService.GetByIdAsync(id); 
        return Ok(Result<InvoiceDto>.Ok(result)); 
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Receptionist}")] 
    [HttpPost("invoices")] 
    public async Task<ActionResult<Result<InvoiceDto>>> CreateInvoice([FromBody] CreateInvoiceDto dto) 
    {
        var result = await _billingService.CreateAsync(dto); 
        return CreatedAtAction(nameof(GetInvoiceById), new { id = result.Id }, Result<InvoiceDto>.Ok(result, "Invoice created successfully.")); 
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Receptionist}")] 
    [HttpPost("invoices/{id:int}/payments")] 
    public async Task<ActionResult<Result<InvoiceDto>>> Pay(int id, [FromBody] PayInvoiceDto dto) 
    {
        var result = await _billingService.PayAsync(id, dto); 
        return Ok(Result<InvoiceDto>.Ok(result, "Payment recorded successfully.")); 
    }

    [Authorize(Roles = Roles.Admin)] 
    [HttpGet("reports/revenue")] 
    public async Task<ActionResult<Result<RevenueReportDto>>> GetRevenueReport([FromQuery] RevenueReportQueryDto query) 
    {
        var result = await _billingService.GetRevenueReportAsync(query); 
        return Ok(Result<RevenueReportDto>.Ok(result)); 
    }
}