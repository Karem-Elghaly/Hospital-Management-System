using Hospital_Management_System.Domain.Entities; 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital_Management_System.Infrastructure.Configurations
{

    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor> 
    {
        public void Configure(EntityTypeBuilder<Doctor> builder) 
        {
            builder.ToTable("Doctors");

            builder.Property(d => d.UserId).IsRequired().HasMaxLength(450); 
            builder.Property(d => d.FullName).IsRequired().HasMaxLength(150); 
            builder.Property(d => d.Specialization).IsRequired().HasMaxLength(100); 
            builder.Property(d => d.Phone).IsRequired().HasMaxLength(15); 
            builder.Property(d => d.ConsultationFee).HasPrecision(18, 2); 

            builder.HasIndex(d => d.UserId).IsUnique().HasFilter("[IsDeleted] = 0"); 

            builder.HasOne(d => d.Department) 
                .WithMany(dep => dep.Doctors) 
                .HasForeignKey(d => d.DepartmentId) 
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
