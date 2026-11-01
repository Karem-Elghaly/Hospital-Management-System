using Hospital_Management_System.Application.Common.Constants;
using Hospital_Management_System.Application.Common.Models;
using Hospital_Management_System.Application.Features.Staff.DTOs;
using Hospital_Management_System.Application.Features.Staff.Service;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc; 

namespace HospitalManagementSystem.API.Controllers; 

[ApiController] 
[Route("api/[controller]")] 
[Authorize(Roles = Roles.Admin)] 
public class StaffController : ControllerBase 
{
    private readonly IStaffService _staffService;

    public StaffController(IStaffService staffService) 
    {
        _staffService = staffService; 
    }

    [HttpPost("receptionists")] 
    public async Task<ActionResult<Result<StaffDto>>> CreateReceptionist([FromBody] CreateStaffDto dto)
    {
        var result = await _staffService.CreateReceptionistAsync(dto); 
        return StatusCode(StatusCodes.Status201Created, Result<StaffDto>.Ok(result, "Receptionist created successfully.")); 
    }

    [HttpDelete("receptionists/{userId}")] 
    public async Task<IActionResult> DeactivateReceptionist(string userId) 
    {
        await _staffService.DeactivateReceptionistAsync(userId); 
        return NoContent(); 
    }
}