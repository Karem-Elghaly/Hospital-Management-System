using Hospital_Management_System.Domain.Enums;

namespace Hospital_Management_System.Application.Features.Appointments.DTOs
{

    public class AppointmentFilterDto 
    {
        public int? DoctorId { get; set; } 

        public int? PatientId { get; set; } 

        public AppointmentStatus? Status { get; set; } 

        public DateOnly? Date { get; set; } 
    }
}
