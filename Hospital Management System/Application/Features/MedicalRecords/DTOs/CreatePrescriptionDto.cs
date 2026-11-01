namespace Hospital_Management_System.Application.Features.MedicalRecords.DTOs; 

public class CreatePrescriptionDto 
{
    public string? GeneralInstructions { get; set; } 

    public List<CreatePrescriptionItemDto> Items { get; set; } = new();
}