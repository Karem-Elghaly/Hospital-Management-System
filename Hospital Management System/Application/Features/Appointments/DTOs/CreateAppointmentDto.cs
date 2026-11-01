namespace Hospital_Management_System.Application.Features.Appointments.DTOs
{
    public class CreateAppointmentDto 
    {
        public int? PatientId { get; set; } 

        public int DoctorId { get; set; } 

        public DateOnly AppointmentDate { get; set; } 

        public TimeOnly StartTime { get; set; } 

        public string? Notes { get; set; } 
    }
}
