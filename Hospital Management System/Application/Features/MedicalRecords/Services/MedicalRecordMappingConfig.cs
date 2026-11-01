using Hospital_Management_System.Application.Features.MedicalRecords.DTOs;
using Hospital_Management_System.Domain.Entities;
using Mapster; 

namespace Hospital_Management_System.Application.Features.MedicalRecords.Services; 

public class MedicalRecordMappingConfig : IRegister 
{
    public void Register(TypeAdapterConfig config) 
    {
        config.NewConfig<MedicalRecord, MedicalRecordDto>() 
            .Map(dest => dest.AppointmentDate, src => src.Appointment.AppointmentDate); 
    }
}