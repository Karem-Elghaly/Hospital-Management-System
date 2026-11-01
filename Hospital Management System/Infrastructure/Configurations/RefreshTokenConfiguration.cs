using Hospital_Management_System.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital_Management_System.Infrastructure.Configurations
{

    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken> 
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder) 
        {
            builder.ToTable("RefreshTokens");

            builder.Property(r => r.UserId).IsRequired().HasMaxLength(450); 
            builder.Property(r => r.Token).IsRequired().HasMaxLength(200); 
            builder.Property(r => r.ReplacedByToken).HasMaxLength(200); 

            builder.Ignore(r => r.IsExpired); 
            builder.Ignore(r => r.IsRevoked); 
            builder.Ignore(r => r.IsActive); 

            builder.HasIndex(r => r.Token).IsUnique(); 
            builder.HasIndex(r => r.UserId); 
        }
    }
}
