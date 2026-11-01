using Hospital_Management_System.Application.Common.Constants; 
using Hospital_Management_System.Infrastructure.Identity; 
using Hospital_Management_System.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity; 
using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.DependencyInjection; 


namespace Hospital_Management_System.Infrastructure.Seed
{
    public static class DbInitializer 
    {
        public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration) 
        {
            using var scope = services.CreateScope(); 
            var provider = scope.ServiceProvider; 

            var context = provider.GetRequiredService<ApplicationDbContext>(); 
            await context.Database.MigrateAsync(); 

            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>(); 
            foreach (var role in Roles.All) 
            {
                if (!await roleManager.RoleExistsAsync(role)) 
                    await roleManager.CreateAsync(new IdentityRole(role)); 
            }

            var email = configuration["SeedAdmin:Email"]; 
            var password = configuration["SeedAdmin:Password"]; 
            var fullName = configuration["SeedAdmin:FullName"] ?? "System Admin"; 

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return; 

            var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
            if (await userManager.FindByEmailAsync(email) is not null) return; 

            var admin = new ApplicationUser 
            {
                UserName = email, 
                Email = email, 
                FullName = fullName, 
                EmailConfirmed = true 
            };

            var result = await userManager.CreateAsync(admin, password); 
            if (!result.Succeeded) 
                throw new InvalidOperationException("Failed to seed admin: " + string.Join(" ", result.Errors.Select(e => e.Description)));

            await userManager.AddToRoleAsync(admin, Roles.Admin); 
        }
    }
}
