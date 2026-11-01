using Hospital_Management_System.Application.Common.Models; 
using Hospital_Management_System.Application.Features.Appointments.DTOs;

namespace Hospital_Management_System.Application.Features.Appointments.Services; 

public interface IAppointmentService 
{
    Task<PagedResult<AppointmentDto>> GetAllAsync(PaginationParams pagination, AppointmentFilterDto filter); 

    Task<AppointmentDto> GetByIdAsync(int id); 

    Task<IReadOnlyList<AvailableSlotDto>> GetAvailableSlotsAsync(int doctorId, DateOnly date); 

    Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto); 

    Task<AppointmentDto> UpdateAsync(int id, UpdateAppointmentDto dto); 

    Task<AppointmentDto> ChangeStatusAsync(int id, ChangeAppointmentStatusDto dto); 
}