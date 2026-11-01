namespace Hospital_Management_System.Application.Features.Doctors.DTOs
{
    public class DoctorScheduleDto 
    {
        public int Id { get; set; }

        public int DoctorId { get; set; } 

        public DayOfWeek DayOfWeek { get; set; } 

        public TimeOnly StartTime { get; set; } 

        public TimeOnly EndTime { get; set; } 

        public int SlotDurationMinutes { get; set; }
    }
}
