using Hospital_Management_System.Application.Common.Models; 
using Hospital_Management_System.Application.Features.Doctors.DTOs;

namespace Hospital_Management_System.Application.Features.Doctors.Service; 

public interface IDoctorService 
{
    Task<PagedResult<DoctorDto>> GetAllAsync(PaginationParams pagination, int? departmentId = null);

    Task<DoctorDto> GetByIdAsync(int id); 

    Task<DoctorDto> CreateAsync(CreateDoctorDto dto); 

    Task<DoctorDto> UpdateAsync(int id, UpdateDoctorDto dto); 

    Task DeleteAsync(int id); 

    Task<IReadOnlyList<DoctorScheduleDto>> GetSchedulesAsync(int doctorId); 

    Task<DoctorScheduleDto> AddScheduleAsync(int doctorId, CreateDoctorScheduleDto dto);

    Task DeleteScheduleAsync(int doctorId, int scheduleId); 
}