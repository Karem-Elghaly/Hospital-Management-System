using Hospital_Management_System.Domain.Entities; 
using Microsoft.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital_Management_System.Infrastructure.Configurations
{
    public class PrescriptionItemConfiguration : IEntityTypeConfiguration<PrescriptionItem> 
    {
        public void Configure(EntityTypeBuilder<PrescriptionItem> builder)
        {
            builder.ToTable("PrescriptionItems"); 

            builder.Property(i => i.MedicineName).IsRequired().HasMaxLength(150); 
            builder.Property(i => i.Dosage).IsRequired().HasMaxLength(100);
            builder.Property(i => i.Frequency).IsRequired().HasMaxLength(100); 
            builder.Property(i => i.Instructions).HasMaxLength(500); 

            builder.HasOne(i => i.Prescription) 
                .WithMany(p => p.Items) 
                .HasForeignKey(i => i.PrescriptionId) 
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
