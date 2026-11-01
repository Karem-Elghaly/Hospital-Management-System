namespace Hospital_Management_System.Application.Features.MedicalRecords.DTOs; 

public class CreateMedicalRecordDto 
{
    public int AppointmentId { get; set; } 

    public string Symptoms { get; set; } = string.Empty; 

    public string Diagnosis { get; set; } = string.Empty; 

    public string? Notes { get; set; }
}