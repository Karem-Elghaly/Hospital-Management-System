using FluentValidation; // عشان AddValidatorsFromAssembly
using Hospital_Management_System.Application.Features.Appointments.Services;
using Hospital_Management_System.Application.Features.Auth.Services;
using Hospital_Management_System.Application.Features.Billing.Service;
using Hospital_Management_System.Application.Features.Departments.Services;
using Hospital_Management_System.Application.Features.Doctors.Service;
using Hospital_Management_System.Application.Features.MedicalRecords.Services;
using Hospital_Management_System.Application.Features.Patients.Service;
using Hospital_Management_System.Application.Features.Staff.Service;
using Mapster; 
using Microsoft.Extensions.DependencyInjection; 
using System.Reflection; 

namespace Hospital_Management_System.Application;

public static class DependencyInjection 
{
    public static IServiceCollection AddApplication(this IServiceCollection services) 
    {
        var assembly = Assembly.GetExecutingAssembly(); 

        services.AddValidatorsFromAssembly(assembly); 

        TypeAdapterConfig.GlobalSettings.Scan(assembly);

        services.AddScoped<IAuthService, AuthService>(); 
        services.AddScoped<IDepartmentService, DepartmentService>(); 
        services.AddScoped<IPatientService, PatientService>(); 
        services.AddScoped<IDoctorService, DoctorService>(); 
        services.AddScoped<IAppointmentService, AppointmentService>(); 
        services.AddScoped<IMedicalRecordService, MedicalRecordService>(); 
        services.AddScoped<IBillingService, BillingService>(); 
        services.AddScoped<IStaffService, StaffService>(); 

        return services; 
    }
}