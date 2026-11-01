namespace Hospital_Management_System.Application.Features.MedicalRecords.DTOs; 

public class MedicalRecordDto 
{
    public int Id { get; set; } 

    public int PatientId { get; set; } 

    public string PatientFullName { get; set; } = string.Empty; 

    public int DoctorId { get; set; } 

    public string DoctorFullName { get; set; } = string.Empty; 

    public int AppointmentId { get; set; } 

    public DateOnly AppointmentDate { get; set; } 

    public string Symptoms { get; set; } = string.Empty; 

    public string Diagnosis { get; set; } = string.Empty; 

    public string? Notes { get; set; } 

    public PrescriptionDto? Prescription { get; set; } 

    public DateTime CreatedAt { get; set; } 
}