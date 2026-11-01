using FluentValidation; 
using Hospital_Management_System.Application.Common.Constants;
using Hospital_Management_System.Application.Common.Interfaces;
using Hospital_Management_System.Application.Features.Staff.DTOs;
using Hospital_Management_System.Domain.Exceptions;
using ForbiddenException = Hospital_Management_System.Application.Common.Exceptions.ForbiddenException; 
using ValidationException = Hospital_Management_System.Application.Common.Exceptions.ValidationException; 

namespace Hospital_Management_System.Application.Features.Staff.Service; 

public class StaffService : IStaffService 
{
    private readonly IIdentityService _identityService; 
    private readonly ICurrentUserService _currentUser; 
    private readonly IValidator<CreateStaffDto> _createValidator; 

    public StaffService( 
        IIdentityService identityService, 
        ICurrentUserService currentUser, 
        IValidator<CreateStaffDto> createValidator) 
    {
        _identityService = identityService; 
        _currentUser = currentUser; 
        _createValidator = createValidator; 
    }

    public async Task<StaffDto> CreateReceptionistAsync(CreateStaffDto dto) 
    {
        EnsureAdmin(); 

        var validation = await _createValidator.ValidateAsync(dto); 
        if (!validation.IsValid) 
            throw new ValidationException(validation.Errors);

        var user = await _identityService.CreateUserAsync(dto.FullName, dto.Email, dto.Password, Roles.Receptionist); 

        return new StaffDto 
        {
            UserId = user.Id, 
            FullName = user.FullName, 
            Email = user.Email, 
            Role = Roles.Receptionist 
        };
    }

    public async Task DeactivateReceptionistAsync(string userId) 
    {
        EnsureAdmin(); 

        if (userId == _currentUser.UserId) 
            throw new BusinessRuleException("You cannot deactivate your own account."); 

        var user = await _identityService.GetUserByIdAsync(userId) 
            ?? throw new NotFoundException("Staff member", userId); 

        if (!user.Roles.Contains(Roles.Receptionist)) 
            throw new NotFoundException("Receptionist", userId); 

        await _identityService.DeactivateUserAsync(userId); 
    }

    private void EnsureAdmin() 
    {
        if (_currentUser.Role != Roles.Admin) 
            throw new ForbiddenException(); 
    }
}