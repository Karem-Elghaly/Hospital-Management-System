namespace Hospital_Management_System.Application.Features.MedicalRecords.DTOs; 

public class CreatePrescriptionItemDto 
{
    public string MedicineName { get; set; } = string.Empty; 

    public string Dosage { get; set; } = string.Empty; 

    public string Frequency { get; set; } = string.Empty; 

    public int DurationInDays { get; set; } 

    public string? Instructions { get; set; }
}