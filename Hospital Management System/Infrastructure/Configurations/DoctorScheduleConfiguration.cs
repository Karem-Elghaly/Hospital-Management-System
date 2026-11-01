using Hospital_Management_System.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital_Management_System.Infrastructure.Configurations
{
    public class DoctorScheduleConfiguration : IEntityTypeConfiguration<DoctorSchedule> 
    {
        public void Configure(EntityTypeBuilder<DoctorSchedule> builder) 
        {
            builder.ToTable("DoctorSchedules"); 

            builder.HasOne(s => s.Doctor) 
                .WithMany(d => d.Schedules) 
                .HasForeignKey(s => s.DoctorId) 
                .OnDelete(DeleteBehavior.Restrict); 

            builder.HasIndex(s => new { s.DoctorId, s.DayOfWeek }); 
        }
    }
}
