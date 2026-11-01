using Hospital_Management_System.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital_Management_System.Infrastructure.Configurations
{
    public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord> 
    {
        public void Configure(EntityTypeBuilder<MedicalRecord> builder) 
        {
            builder.ToTable("MedicalRecords"); 

            builder.Property(m => m.Symptoms).IsRequired().HasMaxLength(1000); 
            builder.Property(m => m.Diagnosis).IsRequired().HasMaxLength(1000); 
            builder.Property(m => m.Notes).HasMaxLength(2000); 

            builder.HasOne(m => m.Patient) 
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(m => m.PatientId) 
                .OnDelete(DeleteBehavior.Restrict); 

            builder.HasOne(m => m.Doctor) 
                .WithMany() 
                .HasForeignKey(m => m.DoctorId) 
                .OnDelete(DeleteBehavior.Restrict); 

            builder.HasOne(m => m.Appointment) 
                .WithOne(a => a.MedicalRecord) 
                .HasForeignKey<MedicalRecord>(m => m.AppointmentId) 
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
