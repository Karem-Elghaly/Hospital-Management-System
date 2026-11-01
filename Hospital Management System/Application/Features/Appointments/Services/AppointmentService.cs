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
using ForbiddenException = Hospital_Management_System.Application.Common.Exceptions.ForbiddenException;

using ValidationException = Hospital_Management_System.Application.Common.Exceptions.ValidationException;
using Hospital_Management_System.Application.Features.Appointments.DTOs;


namespace Hospital_Management_System.Application.Features.Appointments.Services;

public class AppointmentService : IAppointmentService 
{
    private readonly record struct Slot(TimeOnly Start, TimeOnly End); 

    private static readonly Dictionary<AppointmentStatus, AppointmentStatus[]> AllowedTransitions = new() 
    {
        [AppointmentStatus.Pending] = new[] { AppointmentStatus.Confirmed, AppointmentStatus.Cancelled }, 
        [AppointmentStatus.Confirmed] = new[] { AppointmentStatus.Completed, AppointmentStatus.Cancelled }, 
        [AppointmentStatus.Cancelled] = Array.Empty<AppointmentStatus>(), 
        [AppointmentStatus.Completed] = Array.Empty<AppointmentStatus>() 
    };

    private readonly IUnitOfWork _unitOfWork; 
    private readonly ICurrentUserService _currentUser; 
    private readonly IValidator<CreateAppointmentDto> _createValidator; 
    private readonly IValidator<UpdateAppointmentDto> _updateValidator; 
    private readonly IValidator<ChangeAppointmentStatusDto> _statusValidator; 

    public AppointmentService(
        IUnitOfWork unitOfWork, 
        ICurrentUserService currentUser, 
        IValidator<CreateAppointmentDto> createValidator, 
        IValidator<UpdateAppointmentDto> updateValidator,
        IValidator<ChangeAppointmentStatusDto> statusValidator) 
    {
        _unitOfWork = unitOfWork; 
        _currentUser = currentUser; 
        _createValidator = createValidator; 
        _updateValidator = updateValidator; 
        _statusValidator = statusValidator;
    }

    

    public async Task<PagedResult<AppointmentDto>> GetAllAsync(PaginationParams pagination, AppointmentFilterDto filter) 
    {
        var doctorId = filter.DoctorId; 
        var patientId = filter.PatientId; 
        var status = filter.Status; 
        var date = filter.Date;

        if (_currentUser.Role == Roles.Patient) 
        {
            var me = await GetCurrentPatientAsync(); 
            if (me is null) return EmptyPage(pagination); 
            patientId = me.Id; 
        }
        else if (_currentUser.Role == Roles.Doctor) 
        {
            var me = await GetCurrentDoctorAsync(); 
            if (me is null) return EmptyPage(pagination); 
            doctorId = me.Id; 
        }

        Expression<Func<Appointment, bool>> predicate = a => 
            (doctorId == null || a.DoctorId == doctorId) 
            && (patientId == null || a.PatientId == patientId) 
            && (status == null || a.Status == status) 
            && (date == null || a.AppointmentDate == date); 

        var paged = await _unitOfWork.Appointments.GetPagedAsync( 
            pagination, 
            predicate, 
            a => a.AppointmentDate, 
            true, 
            a => a.Patient, 
            a => a.Doctor);

        return paged.Map(a => a.Adapt<AppointmentDto>()); 
    }

    public async Task<AppointmentDto> GetByIdAsync(int id) 
    {
        var appointment = await LoadWithDetailsAsync(id); 
        await EnsureCanAccessAsync(appointment); 
        return appointment.Adapt<AppointmentDto>(); 
    }

    public async Task<IReadOnlyList<AvailableSlotDto>> GetAvailableSlotsAsync(int doctorId, DateOnly date)
    {
        var doctorExists = await _unitOfWork.Doctors.AnyAsync(d => d.Id == doctorId); 
        if (!doctorExists)
            throw new NotFoundException(nameof(Doctor), doctorId); 

        var now = ClinicClock.Now;
        var today = DateOnly.FromDateTime(now); 
        if (date < today) return new List<AvailableSlotDto>(); 

        var dayOfWeek = date.DayOfWeek; 
        var schedules = await _unitOfWork.DoctorSchedules.GetAllAsync( 
            s => s.DoctorId == doctorId && s.DayOfWeek == dayOfWeek); 

        var booked = await _unitOfWork.Appointments.GetAllAsync( 
            a => a.DoctorId == doctorId && a.AppointmentDate == date && a.Status != AppointmentStatus.Cancelled); 
        var bookedStarts = booked.Select(a => a.StartTime).ToHashSet(); 

        var nowTime = TimeOnly.FromDateTime(now); 

        return BuildSlots(schedules) 
            .Where(s => !bookedStarts.Contains(s.Start)) 
            .Where(s => date > today || s.Start > nowTime)
            .Select(s => new AvailableSlotDto { StartTime = s.Start, EndTime = s.End }) 
            .ToList(); 
    }

   

    public async Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto)
    {
        await ValidateAsync(_createValidator, dto); 

        var patientId = await ResolvePatientIdForBookingAsync(dto.PatientId); 

        var doctorExists = await _unitOfWork.Doctors.AnyAsync(d => d.Id == dto.DoctorId); 
        if (!doctorExists) // لو لأ
            throw new NotFoundException(nameof(Doctor), dto.DoctorId);

        var endTime = await ResolveSlotAsync(dto.DoctorId, dto.AppointmentDate, dto.StartTime, null);
        await EnsurePatientIsFreeAsync(patientId, dto.AppointmentDate, dto.StartTime, endTime, null);

        var appointment = new Appointment 
        {
            PatientId = patientId, 
            DoctorId = dto.DoctorId, 
            AppointmentDate = dto.AppointmentDate, 
            StartTime = dto.StartTime, 
            EndTime = endTime, 
            Status = AppointmentStatus.Pending, 
            Notes = dto.Notes 
        };

        await _unitOfWork.Appointments.AddAsync(appointment); 
        await _unitOfWork.SaveChangesAsync();

        var created = await LoadWithDetailsAsync(appointment.Id); 
        return created.Adapt<AppointmentDto>(); 
    }

    public async Task<AppointmentDto> UpdateAsync(int id, UpdateAppointmentDto dto) 
    {
        await ValidateAsync(_updateValidator, dto);

        var appointment = await _unitOfWork.Appointments.GetByIdAsync(id) 
            ?? throw new NotFoundException(nameof(Appointment), id);

        await EnsureCanAccessAsync(appointment); 

        if (appointment.Status != AppointmentStatus.Pending && appointment.Status != AppointmentStatus.Confirmed) 
            throw new BusinessRuleException("Only pending or confirmed appointments can be rescheduled."); 

        var endTime = await ResolveSlotAsync(appointment.DoctorId, dto.AppointmentDate, dto.StartTime, appointment.Id); 
        await EnsurePatientIsFreeAsync(appointment.PatientId, dto.AppointmentDate, dto.StartTime, endTime, appointment.Id); 

        appointment.AppointmentDate = dto.AppointmentDate; 
        appointment.StartTime = dto.StartTime; 
        appointment.EndTime = endTime; 
        appointment.Notes = dto.Notes; 

        _unitOfWork.Appointments.Update(appointment); 
        await _unitOfWork.SaveChangesAsync(); 

        var updated = await LoadWithDetailsAsync(id); 
        return updated.Adapt<AppointmentDto>(); 
    }

    public async Task<AppointmentDto> ChangeStatusAsync(int id, ChangeAppointmentStatusDto dto) 
    {
        await ValidateAsync(_statusValidator, dto); 

        var appointment = await _unitOfWork.Appointments.GetByIdAsync(id) 
            ?? throw new NotFoundException(nameof(Appointment), id); 

        await EnsureCanChangeStatusAsync(appointment, dto.Status); 

        if (!AllowedTransitions[appointment.Status].Contains(dto.Status)) 
            throw new BusinessRuleException($"Cannot change status from {appointment.Status} to {dto.Status}.");

        if (dto.Status == AppointmentStatus.Completed && appointment.AppointmentDate > ClinicClock.Today) 
            throw new BusinessRuleException("Cannot complete an appointment that is scheduled in the future."); 

        appointment.Status = dto.Status; 
        _unitOfWork.Appointments.Update(appointment); 
        await _unitOfWork.SaveChangesAsync(); 

        var updated = await LoadWithDetailsAsync(id); 
        return updated.Adapt<AppointmentDto>(); 
    }

    

    private static List<Slot> BuildSlots(IEnumerable<DoctorSchedule> schedules) 
    {
        var slots = new List<Slot>(); 

        foreach (var schedule in schedules) 
        {
            var start = schedule.StartTime; 
            var duration = schedule.SlotDurationMinutes; 

            while (true)
            {
                var end = start.AddMinutes(duration); 
                if (end > schedule.EndTime || end <= start) break; 
                slots.Add(new Slot(start, end)); 
                start = end; 
            }
        }

        return slots.OrderBy(s => s.Start).ToList(); 
    }

    private async Task<TimeOnly> ResolveSlotAsync(int doctorId, DateOnly date, TimeOnly startTime, int? excludeAppointmentId) 
    {
        var now = ClinicClock.Now; 
        var today = DateOnly.FromDateTime(now);
        if (date < today || date == today && startTime <= TimeOnly.FromDateTime(now)) 
            throw new BusinessRuleException("Cannot book an appointment in the past."); 

        var dayOfWeek = date.DayOfWeek; 
        var schedules = await _unitOfWork.DoctorSchedules.GetAllAsync( 
            s => s.DoctorId == doctorId && s.DayOfWeek == dayOfWeek); 

        var slot = BuildSlots(schedules) 
            .Where(s => s.Start == startTime) 
            .Cast<Slot?>() 
            .FirstOrDefault(); 

        if (slot is null) 
            throw new BusinessRuleException("The doctor does not work at the selected time. Use the available-slots endpoint to pick a valid time."); 

        var taken = await _unitOfWork.Appointments.AnyAsync( 
            a => a.DoctorId == doctorId 
              && a.AppointmentDate == date 
              && a.StartTime == startTime 
              && a.Status != AppointmentStatus.Cancelled 
              && (excludeAppointmentId == null || a.Id != excludeAppointmentId)); 
        if (taken) 
            throw new DoubleBookingException("This time slot is already booked."); 

        return slot.Value.End; 
    }

    private async Task EnsurePatientIsFreeAsync(int patientId, DateOnly date, TimeOnly start, TimeOnly end, int? excludeAppointmentId)
    {
        var busy = await _unitOfWork.Appointments.AnyAsync( 
            a => a.PatientId == patientId 
              && a.AppointmentDate == date 
              && a.Status != AppointmentStatus.Cancelled 
              && start < a.EndTime 
              && end > a.StartTime 
              && (excludeAppointmentId == null || a.Id != excludeAppointmentId)); 
        if (busy) 
            throw new BusinessRuleException("The patient already has another appointment at this time."); 
    }

    private async Task<int> ResolvePatientIdForBookingAsync(int? requestedPatientId) 
    {
        if (_currentUser.Role == Roles.Patient) 
        {
            var me = await GetCurrentPatientAsync() 
                ?? throw new BusinessRuleException("Please complete your patient profile before booking an appointment."); 
            return me.Id; 
        }

        if (requestedPatientId is null) 
            throw new BusinessRuleException("PatientId is required when booking on behalf of a patient."); 

        var patientId = requestedPatientId.Value; 
        var exists = await _unitOfWork.Patients.AnyAsync(p => p.Id == patientId); 
        if (!exists)
            throw new NotFoundException(nameof(Patient), patientId); 

        return patientId; 
    }

   

    private async Task EnsureCanAccessAsync(Appointment appointment) 
    {
        switch (_currentUser.Role) 
        {
            case Roles.Admin: 
            case Roles.Receptionist:
                break; 

            case Roles.Patient: 
                var patient = await GetCurrentPatientAsync(); 
                if (patient is null || patient.Id != appointment.PatientId) 
                    throw new ForbiddenException();
                break; 

            case Roles.Doctor: 
                var doctor = await GetCurrentDoctorAsync(); 
                if (doctor is null || doctor.Id != appointment.DoctorId) 
                    throw new ForbiddenException();
                break; 

            default: 
                throw new ForbiddenException(); 
        }
    }

    private async Task EnsureCanChangeStatusAsync(Appointment appointment, AppointmentStatus target) 
    {
        switch (_currentUser.Role) 
        {
            case Roles.Admin: 
                return; 

            case Roles.Receptionist: 
                if (target == AppointmentStatus.Completed) 
                    throw new ForbiddenException("Receptionists cannot complete appointments."); 
                return; 

            case Roles.Doctor: 
                await EnsureCanAccessAsync(appointment); 
                return; 

            case Roles.Patient: 
                await EnsureCanAccessAsync(appointment); 
                if (target != AppointmentStatus.Cancelled) 
                    throw new ForbiddenException("Patients can only cancel their appointments."); 
                return; 

            default: 
                throw new ForbiddenException(); 
        }
    }


    private async Task<Patient?> GetCurrentPatientAsync() 
    {
        var userId = _currentUser.UserId; 
        if (userId is null) return null; 

        var matches = await _unitOfWork.Patients.GetAllAsync(p => p.UserId == userId); 
        return matches.FirstOrDefault(); 
    }

    private async Task<Doctor?> GetCurrentDoctorAsync() 
    {
        var userId = _currentUser.UserId; 
        if (userId is null) return null;

        var matches = await _unitOfWork.Doctors.GetAllAsync(d => d.UserId == userId); 
        return matches.FirstOrDefault(); 
    }

    private async Task<Appointment> LoadWithDetailsAsync(int id) 
    {
        return await _unitOfWork.Appointments.GetByIdAsync(id, a => a.Patient, a => a.Doctor) 
            ?? throw new NotFoundException(nameof(Appointment), id); 
    }

    private static PagedResult<AppointmentDto> EmptyPage(PaginationParams pagination) 
    {
        return new PagedResult<AppointmentDto>(new List<AppointmentDto>(), 0, pagination.PageNumber, pagination.PageSize); 
    }

    private static async Task ValidateAsync<T>(IValidator<T> validator, T dto) 
    {
        var result = await validator.ValidateAsync(dto); 
        if (!result.IsValid) 
            throw new ValidationException(result.Errors); 
    }
}