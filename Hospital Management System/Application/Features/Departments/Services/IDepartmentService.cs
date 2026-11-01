using Hospital_Management_System.Application.Common.Models;
using Hospital_Management_System.Application.Features.Departments.DTOs;

namespace Hospital_Management_System.Application.Features.Departments.Services
{
    public interface IDepartmentService // عقد الـ Service: الـ Controller بيعتمد على الـ Interface مش على الكلاس (ده اللي بيسهّل الـ Unit Testing)
    {
        Task<PagedResult<DepartmentDto>> GetAllAsync(PaginationParams pagination); // قايمة الأقسام مقسمة صفحات مع بحث

        Task<DepartmentDto> GetByIdAsync(int id); // قسم واحد بالـ Id

        Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto); // إنشاء قسم وبيرجع القسم بعد الحفظ (فيه الـ Id)

        Task<DepartmentDto> UpdateAsync(int id, UpdateDepartmentDto dto); // تعديل قسم

        Task DeleteAsync(int id); // حذف قسم (Soft Delete)، مفيش قيمة راجعة
    }
}
