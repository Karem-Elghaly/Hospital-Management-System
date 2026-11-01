using FluentValidation; // عشان IValidator<T>
using Hospital_Management_System.Domain.Entities; // عشان نقدر نستخدم Department
using Hospital_Management_System.Application.Common.Interfaces;// عشان IUnitOfWork
using Hospital_Management_System.Application.Common.Models; // عشان PagedResult و PaginationParams
using Hospital_Management_System.Domain.Exceptions;// عشان NotFoundException و BusinessRuleException
//using ValidationException = FluentValidation.ValidationException;
using Mapster;// عشان Adapt (التحويل بين الـ Entity والـ DTO)
using ValidationException = Hospital_Management_System.Application.Common.Exceptions.ValidationException;
using Hospital_Management_System.Application.Features.Departments.DTOs;

namespace Hospital_Management_System.Application.Features.Departments.Services
{
    public class DepartmentService : IDepartmentService // بينفذ العقد اللي عملناه
    {
        private readonly IUnitOfWork _unitOfWork; // للوصول للـ Repositories والحفظ
        private readonly IValidator<CreateDepartmentDto> _createValidator; // Validator الإنشاء
        private readonly IValidator<UpdateDepartmentDto> _updateValidator; // Validator التعديل

        public DepartmentService( // الـ Constructor: الـ DI Container بيبعتلنا الـ Dependencies دي أوتوماتيك
            IUnitOfWork unitOfWork, // الـ Unit of Work
            IValidator<CreateDepartmentDto> createValidator, // Validator الإنشاء
            IValidator<UpdateDepartmentDto> updateValidator) // Validator التعديل
        {
            _unitOfWork = unitOfWork; // نخزنه في الحقل
            _createValidator = createValidator; // نخزنه
            _updateValidator = updateValidator; // نخزنه
        }

        public async Task<PagedResult<DepartmentDto>> GetAllAsync(PaginationParams pagination) // قايمة الأقسام
        {
            var term = pagination.SearchTerm?.Trim(); // ناخد كلمة البحث ونشيل المسافات من الأطراف (الـ ?. عشان لو null)

            System.Linq.Expressions.Expression<Func<Department, bool>>? filter = null; // متغير الفلتر، null يعني من غير فلترة
            if (!string.IsNullOrWhiteSpace(term)) // لو المستخدم كتب كلمة بحث
            {
                filter = d => d.Name.Contains(term); // نفلتر الأقسام اللي اسمها فيه الكلمة (EF هيحولها لـ LIKE في SQL)
            }

            var paged = await _unitOfWork.Departments.GetPagedAsync( // بنجيب الصفحة من الـ Repository
                pagination, // رقم الصفحة وحجمها
                filter, // الفلتر (ممكن يكون null)
                orderBy: d => d.Name); // ترتيب أبجدي بالاسم

            return paged.Map(d => d.Adapt<DepartmentDto>()); // نحول كل Department لـ DepartmentDto مع الحفاظ على بيانات الصفحات
        }

        public async Task<DepartmentDto> GetByIdAsync(int id) // قسم بالـ Id
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id) // بندور عليه
                ?? throw new NotFoundException(nameof(Department), id); // لو رجع null (??) نرمي NotFoundException، nameof بتطلع اسم الكلاس كنص "Department"

            return department.Adapt<DepartmentDto>(); // نحوله DTO ونرجعه
        }

        public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto) // إنشاء قسم
        {
            var validation = await _createValidator.ValidateAsync(dto); // نشغل قواعد الـ Validator على الـ DTO
            if (!validation.IsValid) // لو فيه أخطاء
                throw new ValidationException(validation.Errors); // نرمي الـ Exception بتاعنا بالأخطاء

            var exists = await _unitOfWork.Departments.AnyAsync(d => d.Name == dto.Name); // هل فيه قسم بنفس الاسم؟
            if (exists) // لو موجود
                throw new BusinessRuleException($"Department '{dto.Name}' already exists."); // نرفض عشان نمنع التكرار

            var department = dto.Adapt<Department>(); // نحول الـ DTO لـ Entity (Mapster بيطابق الحقول بالاسم)

            await _unitOfWork.Departments.AddAsync(department); // نضيفه للـ Repository (لسه متحفظش)
            await _unitOfWork.SaveChangesAsync(); // نحفظ في الداتابيز، وهنا الـ Id بيتولد ويتحط في department

            return department.Adapt<DepartmentDto>(); // نرجع القسم الجديد كـ DTO فيه الـ Id
        }

        public async Task<DepartmentDto> UpdateAsync(int id, UpdateDepartmentDto dto) // تعديل قسم
        {
            var validation = await _updateValidator.ValidateAsync(dto); // نتحقق من الـ DTO
            if (!validation.IsValid) // لو فيه أخطاء
                throw new ValidationException(validation.Errors); // نرميها

            var department = await _unitOfWork.Departments.GetByIdAsync(id) // نجيب القسم الحالي
                ?? throw new NotFoundException(nameof(Department), id); // مش موجود: 404

            var nameTaken = await _unitOfWork.Departments.AnyAsync(d => d.Name == dto.Name && d.Id != id); // هل اسم تاني قسم (غير ده) بنفس الاسم؟ d.Id != id عشان القسم ميتعارضش مع نفسه
            if (nameTaken) // لو الاسم مستخدم
                throw new BusinessRuleException($"Department '{dto.Name}' already exists."); // نرفض

            dto.Adapt(department); // بننسخ قيم الـ DTO فوق الـ Entity الموجود (Adapt بـ Object موجود بيعدّله بدل ما يعمل جديد)

            _unitOfWork.Departments.Update(department); // نعلّم إنه اتعدل
            await _unitOfWork.SaveChangesAsync(); // نحفظ

            return department.Adapt<DepartmentDto>(); // نرجع النسخة المحدثة
        }

        public async Task DeleteAsync(int id) // حذف قسم
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id) // نجيب القسم
                ?? throw new NotFoundException(nameof(Department), id); // مش موجود: 404

            var hasDoctors = await _unitOfWork.Doctors.AnyAsync(d => d.DepartmentId == id); // هل فيه دكاترة تابعين له؟
            if (hasDoctors) // لو فيه
                throw new BusinessRuleException("Cannot delete a department that still has doctors."); // قاعدة Business: مينفعش نمسح قسم فيه دكاترة

            _unitOfWork.Departments.Delete(department); // Soft Delete (الـ Repository هيحط IsDeleted = true)
            await _unitOfWork.SaveChangesAsync(); // نحفظ
        }
    }
}
