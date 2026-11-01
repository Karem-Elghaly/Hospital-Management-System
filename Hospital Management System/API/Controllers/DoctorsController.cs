using Hospital_Management_System.Application.Common.Constants; 
using Hospital_Management_System.Application.Common.Models; 
using Hospital_Management_System.Application.Features.Doctors.DTOs;
using Hospital_Management_System.Application.Features.Doctors.Service;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc; 

namespace Hospital_Management_System.API.Controllers; 

[ApiController] 
[Route("api/[controller]")] 
[Authorize] 
public class DoctorsController : ControllerBase 
{
    private readonly IDoctorService _doctorService; 

    public DoctorsController(IDoctorService doctorService) 
    {
        _doctorService = doctorService; 
    }

    [HttpGet] 
    public async Task<ActionResult<Result<PagedResult<DoctorDto>>>> GetAll([FromQuery] PaginationParams pagination, [FromQuery] int? departmentId) 
    {
        var result = await _doctorService.GetAllAsync(pagination, departmentId); 
        return Ok(Result<PagedResult<DoctorDto>>.Ok(result)); 
    }

    [HttpGet("{id:int}")] 
    public async Task<ActionResult<Result<DoctorDto>>> GetById(int id) 
    {
        var result = await _doctorService.GetByIdAsync(id); 
        return Ok(Result<DoctorDto>.Ok(result)); 
    }

    [Authorize(Roles = Roles.Admin)] 
    [HttpPost] 
    public async Task<ActionResult<Result<DoctorDto>>> Create([FromBody] CreateDoctorDto dto) 
    {
        var result = await _doctorService.CreateAsync(dto); 
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, Result<DoctorDto>.Ok(result, "Doctor created successfully.")); 
    }

    [Authorize(Roles = Roles.Admin)] 
    [HttpPut("{id:int}")] 
    public async Task<ActionResult<Result<DoctorDto>>> Update(int id, [FromBody] UpdateDoctorDto dto) 
    {
        var result = await _doctorService.UpdateAsync(id, dto); 
        return Ok(Result<DoctorDto>.Ok(result, "Doctor updated successfully.")); 
    }

    [Authorize(Roles = Roles.Admin)] 
    [HttpDelete("{id:int}")] 
    public async Task<IActionResult> Delete(int id) 
    {
        await _doctorService.DeleteAsync(id); 
        return NoContent(); 
    }

    [HttpGet("{id:int}/schedules")] 
    public async Task<ActionResult<Result<IReadOnlyList<DoctorScheduleDto>>>> GetSchedules(int id) 
    {
        var result = await _doctorService.GetSchedulesAsync(id); 
        return Ok(Result<IReadOnlyList<DoctorScheduleDto>>.Ok(result)); 
    }

    [Authorize(Roles = Roles.Admin)] 
    [HttpPost("{id:int}/schedules")] 
    public async Task<ActionResult<Result<DoctorScheduleDto>>> AddSchedule(int id, [FromBody] CreateDoctorScheduleDto dto) 
    {
        var result = await _doctorService.AddScheduleAsync(id, dto);
        return CreatedAtAction(nameof(GetSchedules), new { id }, Result<DoctorScheduleDto>.Ok(result, "Schedule added successfully."));
    }

    [Authorize(Roles = Roles.Admin)] 
    [HttpDelete("{id:int}/schedules/{scheduleId:int}")] 
    public async Task<IActionResult> DeleteSchedule(int id, int scheduleId) 
    {
        await _doctorService.DeleteScheduleAsync(id, scheduleId); 
        return NoContent(); 
    }
}