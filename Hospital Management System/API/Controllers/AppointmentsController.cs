using Hospital_Management_System.Application.Common.Constants; 
using Hospital_Management_System.Application.Common.Models; 
using Hospital_Management_System.Application.Features.Appointments.DTOs;
using Hospital_Management_System.Application.Features.Appointments.Services;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc; 

namespace Hospital_Management_Systeml.API.Controllers; 

[ApiController] 
[Route("api/[controller]")] 
[Authorize] 
public class AppointmentsController : ControllerBase 
{
    private readonly IAppointmentService _appointmentService; 

    public AppointmentsController(IAppointmentService appointmentService) 
    {
        _appointmentService = appointmentService; 
    }

    [HttpGet] 
    public async Task<ActionResult<Result<PagedResult<AppointmentDto>>>> GetAll( 
        [FromQuery] PaginationParams pagination, 
        [FromQuery] AppointmentFilterDto filter) 
    {
        var result = await _appointmentService.GetAllAsync(pagination, filter); 
        return Ok(Result<PagedResult<AppointmentDto>>.Ok(result)); 
    }

    [HttpGet("{id:int}")] 
    public async Task<ActionResult<Result<AppointmentDto>>> GetById(int id) 
    {
        var result = await _appointmentService.GetByIdAsync(id); 
        return Ok(Result<AppointmentDto>.Ok(result)); 
    }

    [HttpGet("available-slots")] 
    public async Task<ActionResult<Result<IReadOnlyList<AvailableSlotDto>>>> GetAvailableSlots( 
        [FromQuery] int doctorId,
        [FromQuery] DateOnly date) 
    {
        var result = await _appointmentService.GetAvailableSlotsAsync(doctorId, date); 
        return Ok(Result<IReadOnlyList<AvailableSlotDto>>.Ok(result)); 
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Receptionist},{Roles.Patient}")] 
    [HttpPost] 
    public async Task<ActionResult<Result<AppointmentDto>>> Create([FromBody] CreateAppointmentDto dto) 
    {
        var result = await _appointmentService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, Result<AppointmentDto>.Ok(result, "Appointment booked successfully.")); 
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Receptionist},{Roles.Patient}")] 
    [HttpPut("{id:int}")] 
    public async Task<ActionResult<Result<AppointmentDto>>> Update(int id, [FromBody] UpdateAppointmentDto dto) 
    {
        var result = await _appointmentService.UpdateAsync(id, dto); 
        return Ok(Result<AppointmentDto>.Ok(result, "Appointment rescheduled successfully.")); 
    }

    [HttpPatch("{id:int}/status")] 
    public async Task<ActionResult<Result<AppointmentDto>>> ChangeStatus(int id, [FromBody] ChangeAppointmentStatusDto dto) 
    {
        var result = await _appointmentService.ChangeStatusAsync(id, dto); 
        return Ok(Result<AppointmentDto>.Ok(result, "Appointment status updated successfully.")); 
    }
}