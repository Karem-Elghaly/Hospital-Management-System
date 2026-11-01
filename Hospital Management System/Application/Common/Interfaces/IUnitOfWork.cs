using Hospital_Management_System.Domain.Entities;

namespace Hospital_Management_System.Application.Common.Interfaces
{
    public interface IUnitOfWork : IDisposable 
    {
        IGenericRepository<Patient> Patients { get; } 
        IGenericRepository<Doctor> Doctors { get; } 
        IGenericRepository<Department> Departments { get; } 
        IGenericRepository<DoctorSchedule> DoctorSchedules { get; } 
        IGenericRepository<Appointment> Appointments { get; } 
        IGenericRepository<MedicalRecord> MedicalRecords { get; } 
        IGenericRepository<Prescription> Prescriptions { get; } 
        IGenericRepository<Invoice> Invoices { get; } 
        IGenericRepository<RefreshToken> RefreshTokens { get; } 

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default); 
    }
}
