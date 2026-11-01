using Hospital_Management_System.Domain.Entities;
using Microsoft.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital_Management_System.Infrastructure.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department> 
    {
        public void Configure(EntityTypeBuilder<Department> builder) 
        {
            builder.ToTable("Departments"); 

            builder.Property(d => d.Name).IsRequired().HasMaxLength(100); 
            builder.Property(d => d.Description).HasMaxLength(500); 

            builder.HasIndex(d => d.Name).IsUnique().HasFilter("[IsDeleted] = 0"); 
        }
    }
}
