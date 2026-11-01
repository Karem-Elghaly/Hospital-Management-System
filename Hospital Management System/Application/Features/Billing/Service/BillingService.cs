using FluentValidation; // عشان IValidator<T>
using Hospital_Management_System.Application.Common.Constants; // عشان Roles
using Hospital_Management_System.Application.Common.Interfaces; // عشان IUnitOfWork و ICurrentUserService
using Hospital_Management_System.Application.Common.Models; // عشان PagedResult و PaginationParams
using Hospital_Management_System.Application.Features.Billing.DTOs; // عشان الـ DTOs
using Hospital_Management_System.Domain.Entities; // عشان Invoice و Patient و Appointment و Doctor
using Hospital_Management_System.Domain.Enums; // عشان AppointmentStatus و PaymentStatus
using Hospital_Management_System.Domain.Exceptions; // عشان NotFoundException و BusinessRuleException
using Mapster; // عشان Adapt
using System.Linq.Expressions; // عشان Expression<Func<...>>
using ForbiddenException = Hospital_Management_System.Application.Common.Exceptions.ForbiddenException; // Alias
using ValidationException = Hospital_Management_System.Application.Common.Exceptions.ValidationException; // Alias

namespace Hospital_Management_System.Application.Features.Billing.Service; // namespace الـ Feature

public class BillingService : IBillingService // بينفذ العقد
{
    private static readonly Expression<Func<Invoice, object>>[] DetailIncludes = // الـ Includes المطلوبة للقراءة
    {
        i => i.Patient, // المريض (لاسمه)
        i => i.Appointment // الحجز (لتاريخه)
    };

    private readonly IUnitOfWork _unitOfWork; // الـ Repositories والحفظ
    private readonly ICurrentUserService _currentUser; // المستخدم الحالي
    private readonly IValidator<CreateInvoiceDto> _createValidator; // Validator الإنشاء
    private readonly IValidator<PayInvoiceDto> _payValidator; // Validator الدفع
    private readonly IValidator<RevenueReportQueryDto> _reportValidator; // Validator التقرير

    public BillingService( // الـ DI
        IUnitOfWork unitOfWork, // Unit of Work
        ICurrentUserService currentUser, // المستخدم الحالي
        IValidator<CreateInvoiceDto> createValidator, // Validator الإنشاء
        IValidator<PayInvoiceDto> payValidator, // Validator الدفع
        IValidator<RevenueReportQueryDto> reportValidator) // Validator التقرير
    {
        _unitOfWork = unitOfWork; // تخزين
        _currentUser = currentUser; // تخزين
        _createValidator = createValidator; // تخزين
        _payValidator = payValidator; // تخزين
        _reportValidator = reportValidator; // تخزين
    }

    public async Task<PagedResult<InvoiceDto>> GetAllAsync(PaginationParams pagination, InvoiceFilterDto filter) // قايمة الفواتير
    {
        var patientId = filter.PatientId; // فلتر المريض
        var appointmentId = filter.AppointmentId; // فلتر الحجز
        var status = filter.Status; // فلتر الحالة

        if (_currentUser.Role == Roles.Patient) // لو مريض
        {
            var me = await GetCurrentPatientAsync(); // ملفه
            if (me is null) return EmptyPage(pagination); // مفيش ملف يبقى مفيش فواتير
            patientId = me.Id; // نفرض الفلتر على نفسه
        }
        else if (_currentUser.Role != Roles.Admin && _currentUser.Role != Roles.Receptionist) // أي دور تاني
        {
            throw new ForbiddenException(); // 403
        }

        Expression<Func<Invoice, bool>> predicate = i => // فلتر واحد بيجمع الشروط
            (patientId == null || i.PatientId == patientId) // المريض
            && (appointmentId == null || i.AppointmentId == appointmentId) // الحجز
            && (status == null || i.Status == status); // الحالة

        var paged = await _unitOfWork.Invoices.GetPagedAsync( // نجيب الصفحة
            pagination, predicate, i => i.CreatedAt, true, DetailIncludes); // الترتيب بالأحدث

        return paged.Map(i => i.Adapt<InvoiceDto>()); // نحول لـ DTO
    }

    public async Task<InvoiceDto> GetByIdAsync(int id) // فاتورة بالـ Id
    {
        var invoice = await LoadWithDetailsAsync(id); // نجيبها
        await EnsureCanAccessAsync(invoice); // نتأكد إنها بتاعته
        return invoice.Adapt<InvoiceDto>(); // DTO
    }

    public async Task<RevenueReportDto> GetRevenueReportAsync(RevenueReportQueryDto query) // تقرير الإيرادات
    {
        if (_currentUser.Role != Roles.Admin) // الأدمن بس
            throw new ForbiddenException(); // 403

        await ValidateAsync(_reportValidator, query); // نتحقق

        var from = query.From; // بداية الفترة
        var to = query.To; // نهايتها

        var invoices = await _unitOfWork.Invoices.GetAllAsync( // فواتير الفترة
            i => i.Appointment.AppointmentDate >= from && i.Appointment.AppointmentDate <= to, // بتاريخ الكشف
            i => i.Appointment); // Include للحجز

        var doctorIds = invoices.Select(i => i.Appointment.DoctorId).Distinct().ToList(); // الدكاترة الظاهرين
        var doctors = await _unitOfWork.Doctors.GetAllAsync(d => doctorIds.Contains(d.Id)); // أسماءهم
        var names = doctors.ToDictionary(d => d.Id, d => d.FullName); // قاموس Id ← الاسم

        var totalBilled = invoices.Sum(i => i.TotalAmount); // إجمالي المفوتر
        var totalCollected = invoices.Sum(i => i.PaidAmount); // إجمالي المحصّل

        return new RevenueReportDto // نبني التقرير
        {
            From = from, // البداية
            To = to, // النهاية
            InvoiceCount = invoices.Count, // العدد
            TotalBilled = totalBilled, // المفوتر
            TotalCollected = totalCollected, // المحصّل
            TotalOutstanding = totalBilled - totalCollected, // المتبقي
            PaidCount = invoices.Count(i => i.Status == PaymentStatus.Paid), // المدفوعة
            PartialCount = invoices.Count(i => i.Status == PaymentStatus.Partial), // الجزئية
            UnpaidCount = invoices.Count(i => i.Status == PaymentStatus.Unpaid), // غير المدفوعة
            ByDoctor = invoices // التفصيل لكل دكتور
                .GroupBy(i => i.Appointment.DoctorId) // نجمّع بالدكتور
                .Select(g => new DoctorRevenueDto // لكل مجموعة
                {
                    DoctorId = g.Key, // رقم الدكتور
                    DoctorName = names.TryGetValue(g.Key, out var name) ? name : "Unknown (deleted doctor)", // اسمه
                    InvoiceCount = g.Count(), // عدد فواتيره
                    TotalBilled = g.Sum(i => i.TotalAmount), // مفوتره
                    TotalCollected = g.Sum(i => i.PaidAmount) // محصّله
                })
                .OrderByDescending(d => d.TotalCollected) // الأعلى تحصيلاً الأول
                .ToList() // List
        };
    }

    public async Task<InvoiceDto> CreateAsync(CreateInvoiceDto dto) // إنشاء فاتورة
    {
        EnsureStaff(); // أدمن أو استقبال

        await ValidateAsync(_createValidator, dto); // نتحقق

        var appointment = await _unitOfWork.Appointments.GetByIdAsync(dto.AppointmentId) // الحجز
            ?? throw new NotFoundException(nameof(Appointment), dto.AppointmentId); // 404

        if (appointment.Status != AppointmentStatus.Completed) // لازم مكتمل
            throw new BusinessRuleException("An invoice can only be created for a completed appointment."); // 409

        var exists = await _unitOfWork.Invoices.AnyAsync(i => i.AppointmentId == dto.AppointmentId); // فيه فاتورة؟
        if (exists) // لو فيه
            throw new BusinessRuleException("This appointment already has an invoice."); // 409

        var doctor = await _unitOfWork.Doctors.GetByIdAsync(appointment.DoctorId) // الدكتور لسعر الكشف
            ?? throw new NotFoundException(nameof(Doctor), appointment.DoctorId); // 404

        var invoice = new Invoice // نبني الفاتورة
        {
            AppointmentId = appointment.Id, // الحجز
            PatientId = appointment.PatientId, // المريض من الحجز
            TotalAmount = doctor.ConsultationFee + dto.AdditionalAmount, // السعر بيتثبت وقت الإنشاء
            PaidAmount = 0, // لسه مدفوعش
            Status = PaymentStatus.Unpaid, // الحالة الأولى
            Description = dto.Description // الوصف
        };

        await _unitOfWork.Invoices.AddAsync(invoice); // نضيف
        await _unitOfWork.SaveChangesAsync(); // نحفظ

        var created = await LoadWithDetailsAsync(invoice.Id); // نجيبها بالتفاصيل
        return created.Adapt<InvoiceDto>(); // DTO
    }

    public async Task<InvoiceDto> PayAsync(int id, PayInvoiceDto dto) // تسجيل دفعة
    {
        EnsureStaff(); // أدمن أو استقبال

        await ValidateAsync(_payValidator, dto); // نتحقق

        var invoice = await _unitOfWork.Invoices.GetByIdAsync(id) // الفاتورة
            ?? throw new NotFoundException(nameof(Invoice), id); // 404

        if (invoice.Status == PaymentStatus.Paid) // مدفوعة كلها
            throw new BusinessRuleException("This invoice is already fully paid."); // 409

        if (dto.Amount > invoice.RemainingAmount) // أكبر من المتبقي
            throw new BusinessRuleException($"Payment exceeds the remaining amount ({invoice.RemainingAmount})."); // 409

        invoice.PaidAmount += dto.Amount; // نزود المدفوع
        invoice.PaidAt = DateTime.UtcNow; // وقت الدفعة
        invoice.Status = invoice.PaidAmount >= invoice.TotalAmount ? PaymentStatus.Paid : PaymentStatus.Partial; // نحدث الحالة

        _unitOfWork.Invoices.Update(invoice); // نعلّم إنها اتعدلت
        await _unitOfWork.SaveChangesAsync(); // نحفظ

        var updated = await LoadWithDetailsAsync(id); // نجيبها بالتفاصيل
        return updated.Adapt<InvoiceDto>(); // DTO
    }

    private void EnsureStaff() // أدمن أو استقبال بس
    {
        if (_currentUser.Role != Roles.Admin && _currentUser.Role != Roles.Receptionist) // أي دور تاني
            throw new ForbiddenException(); // 403
    }

    private async Task EnsureCanAccessAsync(Invoice invoice) // يقدر يشوف الفاتورة دي؟
    {
        if (_currentUser.Role == Roles.Admin || _currentUser.Role == Roles.Receptionist) return; // الموظفين يشوفوا الكل

        if (_currentUser.Role == Roles.Patient) // المريض
        {
            var patient = await GetCurrentPatientAsync(); // ملفه
            if (patient is not null && patient.Id == invoice.PatientId) return; // فاتورته
        }

        throw new ForbiddenException(); // غير كده 403
    }

    private async Task<Patient?> GetCurrentPatientAsync() // ملف المريض الحالي
    {
        var userId = _currentUser.UserId; // Id الحساب
        if (userId is null) return null; // مفيش مستخدم

        var matches = await _unitOfWork.Patients.GetAllAsync(p => p.UserId == userId); // ندور بالـ UserId
        return matches.FirstOrDefault(); // أول نتيجة أو null
    }

    private async Task<Invoice> LoadWithDetailsAsync(int id) // فاتورة بتفاصيلها
    {
        return await _unitOfWork.Invoices.GetByIdAsync(id, DetailIncludes) // نجيبها مع الـ Includes
            ?? throw new NotFoundException(nameof(Invoice), id); // 404
    }

    private static PagedResult<InvoiceDto> EmptyPage(PaginationParams pagination) // صفحة فاضية
    {
        return new PagedResult<InvoiceDto>(new List<InvoiceDto>(), 0, pagination.PageNumber, pagination.PageSize); // من غير عناصر
    }

    private static async Task ValidateAsync<T>(IValidator<T> validator, T dto) // بتشغل أي Validator
    {
        var result = await validator.ValidateAsync(dto); // نشغله
        if (!result.IsValid) // لو فيه أخطاء
            throw new ValidationException(result.Errors); // نرميها
    }
}