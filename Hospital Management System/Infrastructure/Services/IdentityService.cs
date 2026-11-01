using Hospital_Management_System.Application.Common.Exceptions;
using Hospital_Management_System.Application.Common.Interfaces;
using Hospital_Management_System.Application.Common.Models;
using Hospital_Management_System.Domain.Exceptions;
using Hospital_Management_System.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity; 

namespace HospitalManagementSystem.Infrastructure.Services; 

public class IdentityService : IIdentityService 
{
    private readonly UserManager<ApplicationUser> _userManager; 

    public IdentityService(UserManager<ApplicationUser> userManager) 
    {
        _userManager = userManager; 
    }

    public async Task<UserInfo> CreateUserAsync(string fullName, string email, string password, string role) 
    {
        var existing = await _userManager.FindByEmailAsync(email); 
        if (existing is not null)
            throw new BusinessRuleException($"Email '{email}' is already registered."); 

        var user = new ApplicationUser 
        {
            UserName = email, 
            Email = email, 
            FullName = fullName 
        };

        var createResult = await _userManager.CreateAsync(user, password); 
        if (!createResult.Succeeded) 
            throw new BusinessRuleException(string.Join(" ", createResult.Errors.Select(e => e.Description))); 

        var roleResult = await _userManager.AddToRoleAsync(user, role); 
        if (!roleResult.Succeeded) 
        {
            await _userManager.DeleteAsync(user); 
            throw new BusinessRuleException($"Could not assign role '{role}'."); 
        }

        return new UserInfo(user.Id, email, fullName, new List<string> { role }); 
    }

    public async Task<UserInfo?> ValidateCredentialsAsync(string email, string password) 
    {
        var user = await _userManager.FindByEmailAsync(email); 
        if (user is null) return null;

        if (await _userManager.IsLockedOutAsync(user))
            throw new UnauthorizedException("Account is locked. Try again later or contact an administrator."); 

        if (!await _userManager.CheckPasswordAsync(user, password)) 
        {
            await _userManager.AccessFailedAsync(user);
            return null; 
        }

        await _userManager.ResetAccessFailedCountAsync(user); 

        var roles = await _userManager.GetRolesAsync(user); 
        return new UserInfo(user.Id, user.Email!, user.FullName, roles); 
    }

    public async Task<UserInfo?> GetUserByIdAsync(string userId) 
    {
        var user = await _userManager.FindByIdAsync(userId); 
        if (user is null) return null; 

        if (await _userManager.IsLockedOutAsync(user)) 
            return null; 

        var roles = await _userManager.GetRolesAsync(user);
        return new UserInfo(user.Id, user.Email!, user.FullName, roles); 
    }

    public async Task DeleteUserAsync(string userId) 
    {
        var user = await _userManager.FindByIdAsync(userId); 
        if (user is null) return; 

        await _userManager.DeleteAsync(user); 
    }

    public async Task DeactivateUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId); 
        if (user is null) return;

        await _userManager.SetLockoutEnabledAsync(user, true); 
        await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue); 
    }
}