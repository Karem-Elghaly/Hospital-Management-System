using Hospital_Management_System.Domain.Enums;

namespace Hospital_Management_System.Application.Features.Appointments.DTOs
{
    public class AppointmentDto 
    {
        public int Id { get; set; } 

        public int PatientId { get; set; } 

        public string PatientFullName { get; set; } = string.Empty; 

        public int DoctorId { get; set; } 

        public string DoctorFullName { get; set; } = string.Empty; 

        public string DoctorSpecialization { get; set; } = string.Empty; 

        public DateOnly AppointmentDate { get; set; } 

        public TimeOnly StartTime { get; set; } 

        public TimeOnly EndTime { get; set; } 

        public AppointmentStatus Status { get; set; } 

        public string? Notes { get; set; } 

        public DateTime CreatedAt { get; set; } 
    }
}
