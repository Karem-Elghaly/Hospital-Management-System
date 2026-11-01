namespace Hospital_Management_System.Application.Features.MedicalRecords.DTOs; 

public class PrescriptionDto 
{
    public int Id { get; set; } 

    public string? GeneralInstructions { get; set; } 

    public List<PrescriptionItemDto> Items { get; set; } = new(); 

    public DateTime CreatedAt { get; set; } 
}