using Hospital_Management_System.Application.Common.Constants;
using Hospital_Management_System.Application.Common.Models;
using Hospital_Management_System.Application.Features.MedicalRecords.DTOs;
using Hospital_Management_System.Application.Features.MedicalRecords.Services;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc; 
namespace HospitalManagementSystem.API.Controllers; 

[ApiController] 
[Route("api/[controller]")] 
[Authorize] 
public class MedicalRecordsController : ControllerBase 
{
    private readonly IMedicalRecordService _medicalRecordService; 

    public MedicalRecordsController(IMedicalRecordService medicalRecordService) 
    {
        _medicalRecordService = medicalRecordService;
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Doctor},{Roles.Patient}")] 
    [HttpGet] 
    public async Task<ActionResult<Result<PagedResult<MedicalRecordDto>>>> GetAll( 
        [FromQuery] PaginationParams pagination, 
        [FromQuery] MedicalRecordFilterDto filter)
    {
        var result = await _medicalRecordService.GetAllAsync(pagination, filter);
        return Ok(Result<PagedResult<MedicalRecordDto>>.Ok(result)); 
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Doctor},{Roles.Patient}")] 
    [HttpGet("{id:int}")] 
    public async Task<ActionResult<Result<MedicalRecordDto>>> GetById(int id) 
    {
        var result = await _medicalRecordService.GetByIdAsync(id); 
        return Ok(Result<MedicalRecordDto>.Ok(result)); 
    }

    [Authorize(Roles = Roles.Doctor)] 
    [HttpPost] 
    public async Task<ActionResult<Result<MedicalRecordDto>>> Create([FromBody] CreateMedicalRecordDto dto) 
    {
        var result = await _medicalRecordService.CreateAsync(dto); 
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, Result<MedicalRecordDto>.Ok(result, "Medical record created successfully.")); 
    }

    [Authorize(Roles = Roles.Doctor)] 
    [HttpPost("{id:int}/prescription")] 
    public async Task<ActionResult<Result<MedicalRecordDto>>> AddPrescription(int id, [FromBody] CreatePrescriptionDto dto) 
    {
        var result = await _medicalRecordService.AddPrescriptionAsync(id, dto);
        return CreatedAtAction(nameof(GetById), new { id }, Result<MedicalRecordDto>.Ok(result, "Prescription added successfully.")); 
    }
}