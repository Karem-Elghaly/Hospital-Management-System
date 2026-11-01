using Hospital_Management_System.Application.Common.Interfaces; 
using Hospital_Management_System.Application.Common.Models; 
using Hospital_Management_System.Application.Features.Auth.DTOs;
using Hospital_Management_System.Application.Features.Auth.Services;
using Hospital_Management_System.Application.Features.Auth; 
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc; 

namespace Hospital_Management_System.API.Controllers; 

[ApiController] 
[Route("api/[controller]")] 
public class AuthController : ControllerBase 
{
    private readonly IAuthService _authService; 
    private readonly ICurrentUserService _currentUser; 

    public AuthController(IAuthService authService, ICurrentUserService currentUser) 
    {
        _authService = authService; 
        _currentUser = currentUser; 
    }

    [AllowAnonymous] 
    [HttpPost("register")] 
    public async Task<ActionResult<Result<AuthResponseDto>>> Register([FromBody] RegisterDto dto) 
    {
        var result = await _authService.RegisterAsync(dto); 
        return Ok(Result<AuthResponseDto>.Ok(result, "Registered successfully.")); 
    }

    [AllowAnonymous] 
    [HttpPost("login")] 
    public async Task<ActionResult<Result<AuthResponseDto>>> Login([FromBody] LoginDto dto) 
    {
        var result = await _authService.LoginAsync(dto); 
        return Ok(Result<AuthResponseDto>.Ok(result, "Logged in successfully.")); 
    }

    [AllowAnonymous] 
    [HttpPost("refresh")] 
    public async Task<ActionResult<Result<AuthResponseDto>>> Refresh([FromBody] RefreshTokenDto dto) 
    {
        var result = await _authService.RefreshTokenAsync(dto); 
        return Ok(Result<AuthResponseDto>.Ok(result)); 
    }

    [AllowAnonymous] 
    [HttpPost("logout")] 
    public async Task<IActionResult> Logout([FromBody] RefreshTokenDto dto) 
    {
        await _authService.LogoutAsync(dto); 
        return NoContent(); 
    }

    [Authorize] 
    [HttpGet("me")] 
    public ActionResult<Result<object>> Me() 
    {
        var info = new { _currentUser.UserId, _currentUser.Role, _currentUser.IsAuthenticated }; 
        return Ok(Result<object>.Ok(info)); 
    }
}