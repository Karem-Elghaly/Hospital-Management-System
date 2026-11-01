using Hospital_Management_System.Domain.Enums;

namespace Hospital_Management_System.Application.Features.Patients.DTOs
{

    public class CreatePatientDto 
    {
        public string FullName { get; set; } = string.Empty; 

        public string NationalId { get; set; } = string.Empty; 

        public DateOnly DateOfBirth { get; set; } 

        public Gender Gender { get; set; } 

        public BloodType? BloodType { get; set; } 

        public string Phone { get; set; } = string.Empty; 

        public string? Address { get; set; } 
    }

}