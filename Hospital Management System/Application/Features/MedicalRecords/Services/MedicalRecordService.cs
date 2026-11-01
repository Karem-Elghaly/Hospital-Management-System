using FluentValidation; 
using Hospital_Management_System.Application.Common.Constants; 
using Hospital_Management_System.Application.Common.Interfaces; 
using Hospital_Management_System.Application.Common.Models; 
using Hospital_Management_System.Application.Common.Utilities; 
using Hospital_Management_System.Application.Features.MedicalRecords.DTOs;
using Hospital_Management_System.Domain.Entities; 
using Hospital_Management_System.Domain.Enums; 
using Hospital_Management_System.Domain.Exceptions; 
using Mapster; 
using System.Linq.Expressions; 
using ForbiddenException = Hospital_Management_System.Application.Common.Exceptions.ForbiddenException; 
using ValidationException = Hospital_Management_System.Application.Common.Exceptions.ValidationException;

namespace Hospital_Management_System.Application.Features.MedicalRecords.Services; 

public class MedicalRecordService : IMedicalRecordService 
{
    private static readonly Expression<Func<MedicalRecord, object>>[] DetailIncludes = 
    {
        m => m.Patient, 
        m => m.Doctor,
        m => m.Appointment, 
        m => m.Prescription!.Items 
    };

    private readonly IUnitOfWork _unitOfWork; 
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<CreateMedicalRecordDto> _recordValidator; 
    private readonly IValidator<CreatePrescriptionDto> _prescriptionValidator; 

    public MedicalRecordService( 
        IUnitOfWork unitOfWork, 
        ICurrentUserService currentUser, 
        IValidator<CreateMedicalRecordDto> recordValidator, 
        IValidator<CreatePrescriptionDto> prescriptionValidator) 
    {
        _unitOfWork = unitOfWork; 
        _currentUser = currentUser; 
        _recordValidator = recordValidator;
        _prescriptionValidator = prescriptionValidator; 
    }

    

    public async Task<PagedResult<MedicalRecordDto>> GetAllAsync(PaginationParams pagination, MedicalRecordFilterDto filter) 
    {
        var patientId = filter.PatientId; 
        int? doctorId = null; 

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

        Expression<Func<MedicalRecord, bool>> predicate = m => 
            (patientId == null || m.PatientId == patientId) 
            && (doctorId == null || m.DoctorId == doctorId);

        var paged = await _unitOfWork.MedicalRecords.GetPagedAsync( 
            pagination, 
            predicate,
            m => m.CreatedAt,
            true, 
            DetailIncludes); 

        return paged.Map(m => m.Adapt<MedicalRecordDto>()); 
    }

    public async Task<MedicalRecordDto> GetByIdAsync(int id) 
    {
        var record = await LoadWithDetailsAsync(id); 
        await EnsureCanAccessAsync(record); 
        return record.Adapt<MedicalRecordDto>();
    }

    

    public async Task<MedicalRecordDto> CreateAsync(CreateMedicalRecordDto dto) 
    {
        await ValidateAsync(_recordValidator, dto); 

        var doctor = await GetCurrentDoctorAsync() 
            ?? throw new ForbiddenException("Only doctors can write medical records."); 

        var appointment = await _unitOfWork.Appointments.GetByIdAsync(dto.AppointmentId) 
            ?? throw new NotFoundException(nameof(Appointment), dto.AppointmentId); 

        if (appointment.DoctorId != doctor.Id) 
            throw new ForbiddenException("You can only write records for your own appointments."); 

        if (appointment.Status != AppointmentStatus.Confirmed && appointment.Status != AppointmentStatus.Completed) 
            throw new BusinessRuleException("A medical record can only be written for a confirmed or completed appointment.");

        if (appointment.AppointmentDate > ClinicClock.Today)
            throw new BusinessRuleException("Cannot write a medical record for an appointment scheduled in the future."); 

        var exists = await _unitOfWork.MedicalRecords.AnyAsync(m => m.AppointmentId == dto.AppointmentId); 
        if (exists) 
            throw new BusinessRuleException("This appointment already has a medical record."); 

        var record = new MedicalRecord 
        {
            PatientId = appointment.PatientId,
            DoctorId = doctor.Id,
            AppointmentId = appointment.Id, 
            Symptoms = dto.Symptoms, 
            Diagnosis = dto.Diagnosis, 
            Notes = dto.Notes 
        };

        await _unitOfWork.MedicalRecords.AddAsync(record); 

        if (appointment.Status == AppointmentStatus.Confirmed) 
        {
            appointment.Status = AppointmentStatus.Completed; 
            _unitOfWork.Appointments.Update(appointment); 
        }

        await _unitOfWork.SaveChangesAsync(); 

        var created = await LoadWithDetailsAsync(record.Id); 
        return created.Adapt<MedicalRecordDto>(); 
    }

    public async Task<MedicalRecordDto> AddPrescriptionAsync(int medicalRecordId, CreatePrescriptionDto dto) 
    {
        await ValidateAsync(_prescriptionValidator, dto); 

        var doctor = await GetCurrentDoctorAsync() 
            ?? throw new ForbiddenException("Only doctors can write prescriptions.");

        var record = await _unitOfWork.MedicalRecords.GetByIdAsync(medicalRecordId, m => m.Prescription!)
            ?? throw new NotFoundException(nameof(MedicalRecord), medicalRecordId); 

        if (record.DoctorId != doctor.Id)
            throw new ForbiddenException("You can only prescribe for your own medical records.");

        if (record.Prescription is not null) 
            throw new BusinessRuleException("This medical record already has a prescription.");

        var prescription = dto.Adapt<Prescription>();
        prescription.MedicalRecordId = medicalRecordId; 

        await _unitOfWork.Prescriptions.AddAsync(prescription); 
        await _unitOfWork.SaveChangesAsync(); 

        var updated = await LoadWithDetailsAsync(medicalRecordId);
        return updated.Adapt<MedicalRecordDto>(); 
    }

    

    private async Task EnsureCanAccessAsync(MedicalRecord record) 
    {
        switch (_currentUser.Role) 
        {
            case Roles.Admin: 
                break; 

            case Roles.Patient: 
                var patient = await GetCurrentPatientAsync(); 
                if (patient is null || patient.Id != record.PatientId) 
                    throw new ForbiddenException();
                break;

            case Roles.Doctor: 
                var doctor = await GetCurrentDoctorAsync(); 
                if (doctor is null || doctor.Id != record.DoctorId) 
                    throw new ForbiddenException(); 
                break; 

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

    private async Task<MedicalRecord> LoadWithDetailsAsync(int id) 
    {
        return await _unitOfWork.MedicalRecords.GetByIdAsync(id, DetailIncludes) 
            ?? throw new NotFoundException(nameof(MedicalRecord), id); 
    }

    private static PagedResult<MedicalRecordDto> EmptyPage(PaginationParams pagination) 
    {
        return new PagedResult<MedicalRecordDto>(new List<MedicalRecordDto>(), 0, pagination.PageNumber, pagination.PageSize); 
    }

    private static async Task ValidateAsync<T>(IValidator<T> validator, T dto) 
    {
        var result = await validator.ValidateAsync(dto); 
        if (!result.IsValid) 
            throw new ValidationException(result.Errors); 
    }
}