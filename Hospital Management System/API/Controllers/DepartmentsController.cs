using Hospital_Management_System.Application.Common.Constants; 
using Microsoft.AspNetCore.Authorization; 
using Hospital_Management_System.Application.Common.Models;
using Hospital_Management_System.Application.Features.Departments; 
using Microsoft.AspNetCore.Mvc;
using Hospital_Management_System.Application.Features.Departments.DTOs;
using Hospital_Management_System.Application.Features.Departments.Services; 

namespace Hospital_Management_System.API.Controllers; 

[ApiController] 
[Route("api/[controller]")] 
public class DepartmentsController : ControllerBase 
{
    private readonly IDepartmentService _departmentService; 

    public DepartmentsController(IDepartmentService departmentService) 
    {
        _departmentService = departmentService; 
    }

    
    [HttpGet] 
    public async Task<ActionResult<Result<PagedResult<DepartmentDto>>>> GetAll([FromQuery] PaginationParams pagination) 
    {
        var result = await _departmentService.GetAllAsync(pagination);
        return Ok(Result<PagedResult<DepartmentDto>>.Ok(result)); 
    }

    [HttpGet("{id:int}")] 
    public async Task<ActionResult<Result<DepartmentDto>>> GetById(int id) 
    {
        var result = await _departmentService.GetByIdAsync(id); 
        return Ok(Result<DepartmentDto>.Ok(result)); 
    }

    [Authorize(Roles = Roles.Admin)] 
    [HttpPost] 
    public async Task<ActionResult<Result<DepartmentDto>>> Create([FromBody] CreateDepartmentDto dto) 
    {
        var result = await _departmentService.CreateAsync(dto); 
        return CreatedAtAction( 
            nameof(GetById),
            new { id = result.Id }, 
            Result<DepartmentDto>.Ok(result, "Department created successfully.")); 
    }

    [Authorize(Roles = Roles.Admin)] 
    [HttpPut("{id:int}")] 
    public async Task<ActionResult<Result<DepartmentDto>>> Update(int id, [FromBody] UpdateDepartmentDto dto) 
    {
        var result = await _departmentService.UpdateAsync(id, dto); 
        return Ok(Result<DepartmentDto>.Ok(result, "Department updated successfully.")); 
    }

    [Authorize(Roles = Roles.Admin)] 
    [HttpDelete("{id:int}")] 
    public async Task<IActionResult> Delete(int id) 
    {
        await _departmentService.DeleteAsync(id); 
        return NoContent(); 
    }
}