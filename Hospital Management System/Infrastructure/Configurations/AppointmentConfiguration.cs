using Hospital_Management_System.Domain.Entities; 
using Microsoft.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital_Management_System.Infrastructure.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment> 
    {
        public void Configure(EntityTypeBuilder<Appointment> builder) 
        {
            builder.ToTable("Appointments"); 

            builder.Property(a => a.Notes).HasMaxLength(500); 

            builder.HasOne(a => a.Patient) 
                .WithMany(p => p.Appointments) 
                .HasForeignKey(a => a.PatientId) 
                .OnDelete(DeleteBehavior.Restrict); 

            builder.HasOne(a => a.Doctor) 
                .WithMany(d => d.Appointments) 
                .HasForeignKey(a => a.DoctorId) 
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(a => new { a.DoctorId, a.AppointmentDate, a.StartTime }) 
                .IsUnique() 
                .HasFilter("[IsDeleted] = 0 AND [Status] <> 3"); 
        }
    }
}
