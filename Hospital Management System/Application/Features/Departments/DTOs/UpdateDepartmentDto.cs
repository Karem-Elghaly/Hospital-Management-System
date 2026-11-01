namespace Hospital_Management_System.Application.Features.Departments.DTOs
{
    public class UpdateDepartmentDto // الحقول المسموح تتعدل بس
    {
        public string Name { get; set; } = string.Empty; // الاسم الجديد

        public string? Description { get; set; } // الوصف الجديد
    }
}
