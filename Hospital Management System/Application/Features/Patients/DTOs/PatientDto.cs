using Hospital_Management_System.Domain.Enums;

namespace Hospital_Management_System.Application.Features.Patients.DTOs
{
    public class PatientDto 
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty; 

        public string NationalId { get; set; } = string.Empty; 

        public DateOnly DateOfBirth { get; set; } 

        public int Age 
        {
            get 
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow); 
                var age = today.Year - DateOfBirth.Year; 
                if (DateOfBirth > today.AddYears(-age)) age--; 
                return age; 
            }
        }

        public Gender Gender { get; set; } 

        public BloodType? BloodType { get; set; } 

        public string Phone { get; set; } = string.Empty; 

        public string? Address { get; set; } 

        public DateTime CreatedAt { get; set; }
    }
}
