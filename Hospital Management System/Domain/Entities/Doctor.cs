using Hospital_Management_System.Domain.Common;

namespace Hospital_Management_System.Domain.Entities
{
    public class Doctor : BaseEntity 
    {
        public string UserId { get; set; } = string.Empty; 

        public string FullName { get; set; } = string.Empty; 

        public string Specialization { get; set; } = string.Empty; 

        public string Phone { get; set; } = string.Empty;

        public decimal ConsultationFee { get; set; } 

        public int DepartmentId { get; set; } 

        public Department Department { get; set; } = null!; 

        public ICollection<DoctorSchedule> Schedules { get; set; } = new List<DoctorSchedule>(); 

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>(); 
    }
}
