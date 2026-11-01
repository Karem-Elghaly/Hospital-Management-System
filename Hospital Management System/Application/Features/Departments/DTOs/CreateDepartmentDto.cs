namespace Hospital_Management_System.Application.Features.Departments.DTOs
{

    public class CreateDepartmentDto // البيانات اللي الـ Client بيبعتها وهو بيعمل قسم جديد
    {
        public string Name { get; set; } = string.Empty; // اسم القسم (مطلوب)، مفيش Id لأن الداتابيز هي اللي بتولده

        public string? Description { get; set; } // الوصف (اختياري)
    }
}
