using Hospital_Management_System.Domain.Common;

namespace Hospital_Management_System.Domain.Entities
{
    public class MedicalRecord : BaseEntity 
    {
        public int PatientId { get; set; } 

        public Patient Patient { get; set; } = null!;

        public int DoctorId { get; set; } 

        public Doctor Doctor { get; set; } = null!; 

        public int AppointmentId { get; set; } 

        public Appointment Appointment { get; set; } = null!; 

        public string Symptoms { get; set; } = string.Empty;

        public string Diagnosis { get; set; } = string.Empty; 

        public string? Notes { get; set; } 

        public Prescription? Prescription { get; set; }
    }
}
