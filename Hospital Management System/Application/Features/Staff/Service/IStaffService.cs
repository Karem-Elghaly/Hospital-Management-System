using Hospital_Management_System.Application.Features.Staff.DTOs;

namespace Hospital_Management_System.Application.Features.Staff.Service
{
    public interface IStaffService 
    {
        Task<StaffDto> CreateReceptionistAsync(CreateStaffDto dto); 

        Task DeactivateReceptionistAsync(string userId); 
    }
}
