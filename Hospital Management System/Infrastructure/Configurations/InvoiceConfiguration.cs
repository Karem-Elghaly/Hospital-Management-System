using Hospital_Management_System.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital_Management_System.Infrastructure.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice> 
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.ToTable("Invoices"); 

            builder.Property(i => i.TotalAmount).HasPrecision(18, 2); 
            builder.Property(i => i.PaidAmount).HasPrecision(18, 2); 
            builder.Property(i => i.Description).HasMaxLength(500); 

            builder.Ignore(i => i.RemainingAmount);

            builder.HasOne(i => i.Appointment) 
                .WithOne(a => a.Invoice) 
                .HasForeignKey<Invoice>(i => i.AppointmentId) 
                .OnDelete(DeleteBehavior.Restrict); 

            builder.HasOne(i => i.Patient) 
                .WithMany() 
                .HasForeignKey(i => i.PatientId) 
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
