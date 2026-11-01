namespace Hospital_Management_System.Application.Features.Doctors.DTOs
{
    public class UpdateDoctorDto 
    {
        public string FullName { get; set; } = string.Empty; 

        public string Specialization { get; set; } = string.Empty; 

        public string Phone { get; set; } = string.Empty; 

        public decimal ConsultationFee { get; set; } 

        public int DepartmentId { get; set; } 
    }
}
