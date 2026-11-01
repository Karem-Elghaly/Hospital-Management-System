using FluentValidation; 
using Hospital_Management_System.Application.Common.Constants; 
using Hospital_Management_System.Application.Common.Interfaces;
using Hospital_Management_System.Application.Common.Models; 
using Hospital_Management_System.Application.Common.Utilities; 
using Hospital_Management_System.Domain.Entities; 
using Hospital_Management_System.Domain.Enums; 
using Hospital_Management_System.Domain.Exceptions; 
using Mapster; 
using System.Linq.Expressions; 
using ValidationException = Hospital_Management_System.Application.Common.Exceptions.ValidationException;
using Hospital_Management_System.Application.Features.Doctors.DTOs;

namespace Hospital_Management_System.Application.Features.Doctors.Service; 

public class DoctorService : IDoctorService 
{
    private readonly IUnitOfWork _unitOfWork; 
    private readonly IIdentityService _identityService; 
    private readonly IValidator<CreateDoctorDto> _createValidator; 
    private readonly IValidator<UpdateDoctorDto> _updateValidator; 
    private readonly IValidator<CreateDoctorScheduleDto> _scheduleValidator; 

    public DoctorService( 
        IUnitOfWork unitOfWork, 
        IIdentityService identityService, 
        IValidator<CreateDoctorDto> createValidator, 
        IValidator<UpdateDoctorDto> updateValidator, 
        IValidator<CreateDoctorScheduleDto> scheduleValidator) 
    {
        _unitOfWork = unitOfWork; 
        _identityService = identityService;
        _createValidator = createValidator;
        _updateValidator = updateValidator; 
        _scheduleValidator = scheduleValidator; 
    }

    public async Task<PagedResult<DoctorDto>> GetAllAsync(PaginationParams pagination, int? departmentId = null) 
    {
        var term = pagination.SearchTerm?.Trim() ?? string.Empty; 
        var hasTerm = term.Length > 0; 

        Expression<Func<Doctor, bool>> filter = d => 
            (departmentId == null || d.DepartmentId == departmentId) 
            && (!hasTerm || d.FullName.Contains(term) || d.Specialization.Contains(term));

        var paged = await _unitOfWork.Doctors.GetPagedAsync( 
            pagination, 
            filter, 
            d => d.FullName, 
            false, 
            d => d.Department); 

        return paged.Map(d => d.Adapt<DoctorDto>());
    }

    public async Task<DoctorDto> GetByIdAsync(int id) 
    {
        return await GetDtoAsync(id); 
    }

    public async Task<DoctorDto> CreateAsync(CreateDoctorDto dto) 
    {
        var validation = await _createValidator.ValidateAsync(dto); 
        if (!validation.IsValid) 
            throw new ValidationException(validation.Errors); 

        var departmentExists = await _unitOfWork.Departments.AnyAsync(d => d.Id == dto.DepartmentId); 
        if (!departmentExists) 
            throw new NotFoundException(nameof(Department), dto.DepartmentId); 

        var user = await _identityService.CreateUserAsync(dto.FullName, dto.Email, dto.Password, Roles.Doctor); 

        var doctor = dto.Adapt<Doctor>(); 
        doctor.UserId = user.Id; 

        try 
        {
            await _unitOfWork.Doctors.AddAsync(doctor);
            await _unitOfWork.SaveChangesAsync(); 
        }
        catch 
        {
            await _identityService.DeleteUserAsync(user.Id); 
            throw; 
        }

        return await GetDtoAsync(doctor.Id); 
    }

    public async Task<DoctorDto> UpdateAsync(int id, UpdateDoctorDto dto) 
    {
        var validation = await _updateValidator.ValidateAsync(dto); 
        if (!validation.IsValid) 
            throw new ValidationException(validation.Errors); 

        var doctor = await _unitOfWork.Doctors.GetByIdAsync(id) 
            ?? throw new NotFoundException(nameof(Doctor), id); 

        var departmentExists = await _unitOfWork.Departments.AnyAsync(d => d.Id == dto.DepartmentId); 
        if (!departmentExists) 
            throw new NotFoundException(nameof(Department), dto.DepartmentId); 

        dto.Adapt(doctor); 
        _unitOfWork.Doctors.Update(doctor); 
        await _unitOfWork.SaveChangesAsync(); 

        return await GetDtoAsync(id);
    }

    public async Task DeleteAsync(int id) 
    {
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(id) 
            ?? throw new NotFoundException(nameof(Doctor), id); 

        var hasActiveAppointments = await _unitOfWork.Appointments.AnyAsync( 
            a => a.DoctorId == id 
              && (a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Confirmed));
        if (hasActiveAppointments) 
            throw new BusinessRuleException("Cannot delete a doctor who has active appointments."); 

        _unitOfWork.Doctors.Delete(doctor); 
        await _unitOfWork.SaveChangesAsync(); 

        await _identityService.DeactivateUserAsync(doctor.UserId); 
    }

    public async Task<IReadOnlyList<DoctorScheduleDto>> GetSchedulesAsync(int doctorId) 
    {
        await EnsureDoctorExistsAsync(doctorId); 

        var schedules = await _unitOfWork.DoctorSchedules.GetAllAsync(s => s.DoctorId == doctorId); 

        return schedules 
            .OrderBy(s => s.DayOfWeek)
            .ThenBy(s => s.StartTime) 
            .Select(s => s.Adapt<DoctorScheduleDto>()) 
            .ToList(); 
    }

    public async Task<DoctorScheduleDto> AddScheduleAsync(int doctorId, CreateDoctorScheduleDto dto) 
    {
        var validation = await _scheduleValidator.ValidateAsync(dto);
        if (!validation.IsValid) 
            throw new ValidationException(validation.Errors); 

        await EnsureDoctorExistsAsync(doctorId); 

        var overlaps = await _unitOfWork.DoctorSchedules.AnyAsync( 
            s => s.DoctorId == doctorId 
              && s.DayOfWeek == dto.DayOfWeek 
              && dto.StartTime < s.EndTime
              && dto.EndTime > s.StartTime); 
        if (overlaps) 
            throw new BusinessRuleException("This schedule overlaps with an existing one on the same day."); 

        var schedule = dto.Adapt<DoctorSchedule>(); 
        schedule.DoctorId = doctorId; 

        await _unitOfWork.DoctorSchedules.AddAsync(schedule);
        await _unitOfWork.SaveChangesAsync(); 

        return schedule.Adapt<DoctorScheduleDto>(); 
    }

    public async Task DeleteScheduleAsync(int doctorId, int scheduleId) 
    {
        var schedule = await _unitOfWork.DoctorSchedules.GetByIdAsync(scheduleId); 
        if (schedule is null || schedule.DoctorId != doctorId) 
            throw new NotFoundException(nameof(DoctorSchedule), scheduleId); 

        var today = ClinicClock.Today; 
        var upcoming = await _unitOfWork.Appointments.GetAllAsync( 
            a => a.DoctorId == doctorId 
              && a.AppointmentDate >= today 
              && (a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Confirmed)); 

        var affected = upcoming.Any(a => 
            a.AppointmentDate.DayOfWeek == schedule.DayOfWeek 
            && a.StartTime >= schedule.StartTime 
            && a.StartTime < schedule.EndTime); 
        if (affected) 
            throw new BusinessRuleException("Cannot delete a schedule that has upcoming appointments.");

        _unitOfWork.DoctorSchedules.Delete(schedule); 
        await _unitOfWork.SaveChangesAsync(); 
    }

    private async Task<DoctorDto> GetDtoAsync(int id)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(id, d => d.Department) 
            ?? throw new NotFoundException(nameof(Doctor), id); 
        return doctor.Adapt<DoctorDto>();
    }

    private async Task EnsureDoctorExistsAsync(int doctorId) 
    {
        var exists = await _unitOfWork.Doctors.AnyAsync(d => d.Id == doctorId); 
        if (!exists) 
            throw new NotFoundException(nameof(Doctor), doctorId); 
    }
}