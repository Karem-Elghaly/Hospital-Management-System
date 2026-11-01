using Hospital_Management_System.Application.Common.Constants; 
using Hospital_Management_System.Application.Common.Models; 
using Hospital_Management_System.Application.Features.Patients.DTOs;
using Hospital_Management_System.Application.Features.Patients.Service;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc; 
namespace Hospital_Management_System.API.Controllers; 

[ApiController] 
[Route("api/[controller]")] 
[Authorize] 
public class PatientsController : ControllerBase 
{
    private readonly IPatientService _patientService; 

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService; 
    }

    [Authorize(Roles = Roles.Patient)] 
    [HttpGet("me")] 
    public async Task<ActionResult<Result<PatientDto>>> GetMyProfile() 
    {
        var result = await _patientService.GetMyProfileAsync(); 
        return Ok(Result<PatientDto>.Ok(result)); 
    }

    [Authorize(Roles = Roles.Patient)] 
    [HttpPost("me")] 
    public async Task<ActionResult<Result<PatientDto>>> CreateMyProfile([FromBody] CreatePatientDto dto) 
    {
        var result = await _patientService.CreateMyProfileAsync(dto); 
        return CreatedAtAction(nameof(GetMyProfile), Result<PatientDto>.Ok(result, "Profile created successfully.")); 
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Doctor},{Roles.Receptionist}")] 
    [HttpGet] 
    public async Task<ActionResult<Result<PagedResult<PatientDto>>>> GetAll([FromQuery] PaginationParams pagination)
    {
        var result = await _patientService.GetAllAsync(pagination);
        return Ok(Result<PagedResult<PatientDto>>.Ok(result)); 
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Doctor},{Roles.Receptionist}")] 
    [HttpGet("{id:int}")] 
    public async Task<ActionResult<Result<PatientDto>>> GetById(int id) 
    {
        var result = await _patientService.GetByIdAsync(id); 
        return Ok(Result<PatientDto>.Ok(result)); 
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Receptionist}")] 
    [HttpPost] 
    public async Task<ActionResult<Result<PatientDto>>> Create([FromBody] CreatePatientDto dto)
    {
        var result = await _patientService.CreateAsync(dto); 
        return CreatedAtAction( 
            nameof(GetById), 
            new { id = result.Id }, 
            Result<PatientDto>.Ok(result, "Patient created successfully.")); 
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Receptionist}")] 
    [HttpPut("{id:int}")] 
    public async Task<ActionResult<Result<PatientDto>>> Update(int id, [FromBody] UpdatePatientDto dto) 
    {
        var result = await _patientService.UpdateAsync(id, dto); 
        return Ok(Result<PatientDto>.Ok(result, "Patient updated successfully.")); 
    }

    [Authorize(Roles = Roles.Admin)] 
    [HttpDelete("{id:int}")] 
    public async Task<IActionResult> Delete(int id) 
    {
        await _patientService.DeleteAsync(id); 
        return NoContent(); 
    }
}