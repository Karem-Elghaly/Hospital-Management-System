namespace Hospital_Management_System.Application.Features.Staff.DTOs
{
    public class CreateStaffDto 
    {
        public string FullName { get; set; } = string.Empty; 

        public string Email { get; set; } = string.Empty; 

        public string Password { get; set; } = string.Empty; 
    }
}
