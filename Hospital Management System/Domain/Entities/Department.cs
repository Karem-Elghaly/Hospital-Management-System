using Hospital_Management_System.Domain.Common;

namespace Hospital_Management_System.Domain.Entities
{
    public class Department : BaseEntity 
    {
        public string Name { get; set; } = string.Empty; 

        public string? Description { get; set; } 

        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>(); 
    }
}
