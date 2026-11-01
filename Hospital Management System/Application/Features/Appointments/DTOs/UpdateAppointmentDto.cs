namespace Hospital_Management_System.Application.Features.Appointments.DTOs
{
    public class UpdateAppointmentDto 
    {
        public DateOnly AppointmentDate { get; set; } 

        public TimeOnly StartTime { get; set; } 

        public string? Notes { get; set; } 
    }
}
