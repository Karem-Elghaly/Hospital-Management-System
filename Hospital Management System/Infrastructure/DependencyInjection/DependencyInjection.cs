using Hospital_Management_System.Application.Common.Interfaces; 
using Hospital_Management_System.Infrastructure.Identity; 
using Hospital_Management_System.Infrastructure.Persistence; 
using Hospital_Management_System.Infrastructure.Repositories; 
using Hospital_Management_System.Infrastructure.Services; 
using HospitalManagementSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Identity; 
using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.DependencyInjection; 

namespace Hospital_Management_System.Infrastructure; 

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) 
    {
        services.AddDbContext<ApplicationDbContext>(options => 
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))); 

        services.AddIdentityCore<ApplicationUser>(options => 
        {
            options.User.RequireUniqueEmail = true; 
            options.Password.RequiredLength = 8; 
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true; 
            options.Password.RequireUppercase = true; 
            options.Password.RequireNonAlphanumeric = false; 
            options.Lockout.MaxFailedAccessAttempts = 5; 
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); 
        })
            .AddRoles<IdentityRole>() 
            .AddEntityFrameworkStores<ApplicationDbContext>(); 

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName)); 

        services.AddHttpContextAccessor();

        services.AddScoped<IUnitOfWork, UnitOfWork>(); 
        services.AddScoped<ITokenService, TokenService>(); 
        services.AddScoped<IIdentityService, IdentityService>(); 
        services.AddScoped<ICurrentUserService, CurrentUserService>(); 

        return services; 
    }
}