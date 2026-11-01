using Hospital_Management_System.Domain.Common;

namespace Hospital_Management_System.Domain.Entities
{
    public class PrescriptionItem : BaseEntity 
    {
        public int PrescriptionId { get; set; } 

        public Prescription Prescription { get; set; } = null!; 

        public string MedicineName { get; set; } = string.Empty;

        public string Dosage { get; set; } = string.Empty; 

        public string Frequency { get; set; } = string.Empty;

        public int DurationInDays { get; set; } 

        public string? Instructions { get; set; } 
    }
}
