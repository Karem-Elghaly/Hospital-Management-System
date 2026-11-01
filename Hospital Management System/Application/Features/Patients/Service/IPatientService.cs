using Hospital_Management_System.Application.Common.Models; 
using Hospital_Management_System.Application.Features.Patients.DTOs;

namespace Hospital_Management_System.Application.Features.Patients.Service; 

public interface IPatientService 
{
    Task<PagedResult<PatientDto>> GetAllAsync(PaginationParams pagination); 

    Task<PatientDto> GetByIdAsync(int id); 

    Task<PatientDto> CreateAsync(CreatePatientDto dto); 

    Task<PatientDto> UpdateAsync(int id, UpdatePatientDto dto);

    Task DeleteAsync(int id); 

    Task<PatientDto> GetMyProfileAsync(); 

    Task<PatientDto> CreateMyProfileAsync(CreatePatientDto dto); 
}