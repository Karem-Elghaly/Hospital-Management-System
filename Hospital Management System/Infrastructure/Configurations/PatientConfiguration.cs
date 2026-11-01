using Hospital_Management_System.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital_Management_System.Infrastructure.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient> 
    {
        public void Configure(EntityTypeBuilder<Patient> builder) 
        {
            builder.ToTable("Patients"); 

            builder.Property(p => p.FullName).IsRequired().HasMaxLength(150); 
            builder.Property(p => p.NationalId).IsRequired().HasMaxLength(14); 
            builder.Property(p => p.Phone).IsRequired().HasMaxLength(15); 
            builder.Property(p => p.Address).HasMaxLength(300); 
            builder.Property(p => p.UserId).HasMaxLength(450); 

            builder.HasIndex(p => p.NationalId).IsUnique().HasFilter("[IsDeleted] = 0"); 
        }
    }
}
