using Hospital_Management_System.Application.Common.Interfaces;
using Hospital_Management_System.Domain.Entities;
using Hospital_Management_System.Infrastructure.Persistence;

namespace Hospital_Management_System.Infrastructure.Repositories
{

    public class UnitOfWork : IUnitOfWork 
    {
        private readonly ApplicationDbContext _context; 

        private IGenericRepository<Patient>? _patients; 
        private IGenericRepository<Doctor>? _doctors; 
        private IGenericRepository<Department>? _departments; 
        private IGenericRepository<DoctorSchedule>? _doctorSchedules;
        private IGenericRepository<Appointment>? _appointments; 
        private IGenericRepository<MedicalRecord>? _medicalRecords; 
        private IGenericRepository<Prescription>? _prescriptions; 
        private IGenericRepository<Invoice>? _invoices; 
        private IGenericRepository<RefreshToken>? _refreshTokens; 

        public UnitOfWork(ApplicationDbContext context) 
        {
            _context = context; 
        }

        public IGenericRepository<Patient> Patients => _patients ??= new GenericRepository<Patient>(_context); 
        public IGenericRepository<Doctor> Doctors => _doctors ??= new GenericRepository<Doctor>(_context); 
        public IGenericRepository<Department> Departments => _departments ??= new GenericRepository<Department>(_context); 
        public IGenericRepository<DoctorSchedule> DoctorSchedules => _doctorSchedules ??= new GenericRepository<DoctorSchedule>(_context);
        public IGenericRepository<Appointment> Appointments => _appointments ??= new GenericRepository<Appointment>(_context); 
        public IGenericRepository<MedicalRecord> MedicalRecords => _medicalRecords ??= new GenericRepository<MedicalRecord>(_context); 
        public IGenericRepository<Prescription> Prescriptions => _prescriptions ??= new GenericRepository<Prescription>(_context); 
        public IGenericRepository<Invoice> Invoices => _invoices ??= new GenericRepository<Invoice>(_context); 
        public IGenericRepository<RefreshToken> RefreshTokens => _refreshTokens ??= new GenericRepository<RefreshToken>(_context); 

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) 
        {
            return await _context.SaveChangesAsync(cancellationToken); 
        }

        public void Dispose() 
        {
            _context.Dispose(); 
        }
    }
}
