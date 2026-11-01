using Hospital_Management_System.Domain.Common;
using Hospital_Management_System.Domain.Enums;

namespace Hospital_Management_System.Domain.Entities
{
    public class Appointment : BaseEntity 
    {
        public int PatientId { get; set; } 

        public Patient Patient { get; set; } = null!; 

        public int DoctorId { get; set; } 

        public Doctor Doctor { get; set; } = null!;

        public DateOnly AppointmentDate { get; set; } 

        public TimeOnly StartTime { get; set; } 

        public TimeOnly EndTime { get; set; } 

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending; 

        public string? Notes { get; set; } 

        public MedicalRecord? MedicalRecord { get; set; } 

        public Invoice? Invoice { get; set; } 
    }
}
