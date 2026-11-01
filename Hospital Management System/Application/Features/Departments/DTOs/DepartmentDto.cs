namespace Hospital_Management_System.Application.Features.Departments.DTOs
{
    public class DepartmentDto // الشكل اللي بيرجع من الـ API (من غير IsDeleted أو أي حاجة داخلية)
    {
        public int Id { get; set; } // رقم القسم

        public string Name { get; set; } = string.Empty; // اسم القسم

        public string? Description { get; set; } // وصف القسم (اختياري)

        public DateTime CreatedAt { get; set; } // وقت إنشاء القسم
    }
}
