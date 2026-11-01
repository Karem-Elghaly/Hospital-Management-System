using System.Linq.Expressions; 
using Hospital_Management_System.Domain.Entities; 
using Hospital_Management_System.Domain.Common;
using Hospital_Management_System.Infrastructure.Identity; 
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore; 

namespace Hospital_Management_System.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options) 
        {
        } 

        public DbSet<Patient> Patients { get; set; } = null!; 
        public DbSet<Doctor> Doctors { get; set; } = null!; 
        public DbSet<Department> Departments { get; set; } = null!; 
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; } = null!; 
        public DbSet<Appointment> Appointments { get; set; } = null!; 
        public DbSet<MedicalRecord> MedicalRecords { get; set; } = null!; 
        public DbSet<Prescription> Prescriptions { get; set; } = null!; 
        public DbSet<PrescriptionItem> PrescriptionItems { get; set; } = null!; 
        public DbSet<Invoice> Invoices { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!; 

        protected override void OnModelCreating(ModelBuilder builder) 
        {
            base.OnModelCreating(builder); 

            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly); 

            foreach (var entityType in builder.Model.GetEntityTypes()) 
            {
                if (!typeof(BaseEntity).IsAssignableFrom(entityType.ClrType)) continue; 

                var parameter = Expression.Parameter(entityType.ClrType, "e"); 
                var isDeleted = Expression.Property(parameter, nameof(BaseEntity.IsDeleted)); 
                var notDeleted = Expression.Not(isDeleted); 
                var lambda = Expression.Lambda(notDeleted, parameter);

                entityType.SetQueryFilter(lambda); 
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) 
        {
            ApplyAuditRules(); 
            return base.SaveChangesAsync(cancellationToken); 
        }

        private void ApplyAuditRules() 
        {
            var entries = ChangeTracker.Entries<BaseEntity>().ToList(); 

            foreach (var entry in entries) 
            {
                switch (entry.State) 
                {
                    case EntityState.Added: 
                        entry.Entity.CreatedAt = DateTime.UtcNow; 
                        break; 

                    case EntityState.Modified: 
                        entry.Entity.UpdatedAt = DateTime.UtcNow; 
                        break; 

                    case EntityState.Deleted: 
                        entry.State = EntityState.Modified; 
                        entry.Entity.IsDeleted = true;
                        entry.Entity.UpdatedAt = DateTime.UtcNow; 
                        break; 
                }
            }
        }
    }
}
