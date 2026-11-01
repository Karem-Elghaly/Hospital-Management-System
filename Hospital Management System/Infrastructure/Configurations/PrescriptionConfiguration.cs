using Hospital_Management_System.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital_Management_System.Infrastructure.Configurations
{
    public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription> 
    {
        public void Configure(EntityTypeBuilder<Prescription> builder) 
        {
            builder.ToTable("Prescriptions"); 

            builder.Property(p => p.GeneralInstructions).HasMaxLength(1000);

            builder.HasOne(p => p.MedicalRecord) 
                .WithOne(m => m.Prescription) 
                .HasForeignKey<Prescription>(p => p.MedicalRecordId) 
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
