using Hospital_Management_System.Domain.Common;

namespace Hospital_Management_System.Domain.Entities
{
    public class Prescription : BaseEntity 
    {
        public int MedicalRecordId { get; set; } 

        public MedicalRecord MedicalRecord { get; set; } = null!; 

        public string? GeneralInstructions { get; set; } 

        public ICollection<PrescriptionItem> Items { get; set; } = new List<PrescriptionItem>();
    }
}