namespace Hospital_Management_System.Application.Features.Doctors.DTOs
{
    public class CreateDoctorScheduleDto 
    {
        public DayOfWeek DayOfWeek { get; set; } 

        public TimeOnly StartTime { get; set; } 

        public TimeOnly EndTime { get; set; } 

        public int SlotDurationMinutes { get; set; } = 30; 
    }

}
