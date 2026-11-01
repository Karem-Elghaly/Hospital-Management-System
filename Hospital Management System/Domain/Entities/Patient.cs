using Hospital_Management_System.Domain.Common;
using Hospital_Management_System.Domain.Enums;

namespace Hospital_Management_System.Domain.Entities
{
    public class Patient : BaseEntity 
    {
        public string? UserId { get; set; } 

        public string FullName { get; set; } = string.Empty; 

        public string NationalId { get; set; } = string.Empty; 

        public DateOnly DateOfBirth { get; set; } 

        public Gender Gender { get; set; } 

        public BloodType? BloodType { get; set; } 

        public string Phone { get; set; } = string.Empty; 

        public string? Address { get; set; } 

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>(); 

        public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>(); 
    }
}
