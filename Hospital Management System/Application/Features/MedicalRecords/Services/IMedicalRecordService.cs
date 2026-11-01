using Hospital_Management_System.Application.Common.Models;
using Hospital_Management_System.Application.Features.MedicalRecords.DTOs;



namespace Hospital_Management_System.Application.Features.MedicalRecords.Services;

public interface IMedicalRecordService 
{
    Task<PagedResult<MedicalRecordDto>> GetAllAsync(PaginationParams pagination, MedicalRecordFilterDto filter);

    Task<MedicalRecordDto> GetByIdAsync(int id); 

    Task<MedicalRecordDto> CreateAsync(CreateMedicalRecordDto dto); 

    Task<MedicalRecordDto> AddPrescriptionAsync(int medicalRecordId, CreatePrescriptionDto dto); 
}