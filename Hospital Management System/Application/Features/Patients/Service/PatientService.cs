using FluentValidation; 
using Hospital_Management_System.Application.Common.Interfaces; 
using Hospital_Management_System.Application.Common.Models; 
using Hospital_Management_System.Domain.Entities; 
using Hospital_Management_System.Domain.Enums; 
using Hospital_Management_System.Domain.Exceptions; 
using Mapster; 
using System.Linq.Expressions; 
using UnauthorizedException = Hospital_Management_System.Application.Common.Exceptions.UnauthorizedException;

using ValidationException = Hospital_Management_System.Application.Common.Exceptions.ValidationException;
using Hospital_Management_System.Application.Features.Patients.DTOs;


namespace Hospital_Management_System.Application.Features.Patients.Service; 

public class PatientService : IPatientService 
{
    private readonly IUnitOfWork _unitOfWork; 
    private readonly IValidator<CreatePatientDto> _createValidator; 
    private readonly IValidator<UpdatePatientDto> _updateValidator; 
    private readonly ICurrentUserService _currentUser;

    public PatientService( 
        IUnitOfWork unitOfWork, 
        IValidator<CreatePatientDto> createValidator, 
        IValidator<UpdatePatientDto> updateValidator, 
        ICurrentUserService currentUser) 
    {
        _unitOfWork = unitOfWork; 
        _createValidator = createValidator; 
        _updateValidator = updateValidator; 
        _currentUser = currentUser; 
    }

    public async Task<PagedResult<PatientDto>> GetAllAsync(PaginationParams pagination) 
    {
        var term = pagination.SearchTerm?.Trim(); 

        Expression<Func<Patient, bool>>? filter = null; 
        if (!string.IsNullOrWhiteSpace(term)) 
        {
            filter = p => p.FullName.Contains(term) 
                       || p.NationalId.Contains(term) 
                       || p.Phone.Contains(term); 
        }

        var paged = await _unitOfWork.Patients.GetPagedAsync( 
            pagination, 
            filter, 
            orderBy: p => p.FullName); 

        return paged.Map(p => p.Adapt<PatientDto>()); 
    }

    public async Task<PatientDto> GetByIdAsync(int id) 
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(id) 
            ?? throw new NotFoundException(nameof(Patient), id);

        return patient.Adapt<PatientDto>(); 
    }

    public async Task<PatientDto> CreateAsync(CreatePatientDto dto) 
    {
        return await CreateInternalAsync(dto, null); 
    }

    public async Task<PatientDto> CreateMyProfileAsync(CreatePatientDto dto) 
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException(); 

        var hasProfile = await _unitOfWork.Patients.AnyAsync(p => p.UserId == userId);
        if (hasProfile) // لو عنده
            throw new BusinessRuleException("You already have a patient profile."); 

        return await CreateInternalAsync(dto, userId); 
    }

    public async Task<PatientDto> GetMyProfileAsync() 
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException(); 

        var matches = await _unitOfWork.Patients.GetAllAsync(p => p.UserId == userId); 
        var patient = matches.FirstOrDefault() 
            ?? throw new NotFoundException("Patient profile", userId); 

        return patient.Adapt<PatientDto>(); 
    }

    public async Task<PatientDto> UpdateAsync(int id, UpdatePatientDto dto) 
    {
        var validation = await _updateValidator.ValidateAsync(dto); 
        if (!validation.IsValid) 
            throw new ValidationException(validation.Errors); 

        var patient = await _unitOfWork.Patients.GetByIdAsync(id) 
            ?? throw new NotFoundException(nameof(Patient), id); 

        dto.Adapt(patient); 

        _unitOfWork.Patients.Update(patient);
        await _unitOfWork.SaveChangesAsync(); 

        return patient.Adapt<PatientDto>(); 
    }

    public async Task DeleteAsync(int id) 
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(id) 
            ?? throw new NotFoundException(nameof(Patient), id); 

        var hasActiveAppointments = await _unitOfWork.Appointments.AnyAsync( 
            a => a.PatientId == id 
              && (a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Confirmed)); 
        if (hasActiveAppointments) 
            throw new BusinessRuleException("Cannot delete a patient who has active appointments."); 

        _unitOfWork.Patients.Delete(patient); 
        await _unitOfWork.SaveChangesAsync(); 
    }

    private async Task<PatientDto> CreateInternalAsync(CreatePatientDto dto, string? userId) 
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid) 
            throw new ValidationException(validation.Errors); 

        var exists = await _unitOfWork.Patients.AnyAsync(p => p.NationalId == dto.NationalId); 
        if (exists) 
            throw new BusinessRuleException($"A patient with National ID '{dto.NationalId}' already exists."); 

        var patient = dto.Adapt<Patient>(); 
        patient.UserId = userId; 

        await _unitOfWork.Patients.AddAsync(patient); 
        await _unitOfWork.SaveChangesAsync(); 

        return patient.Adapt<PatientDto>(); 
    }
}