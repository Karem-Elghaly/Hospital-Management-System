using System.Text; 
using Hospital_Management_System.Infrastructure.Identity; 
using Microsoft.AspNetCore.Authentication.JwtBearer; 
using Microsoft.IdentityModel.Tokens; 


namespace Hospital_Management_System.API.Extensions
{

    public static class JwtExtensions 
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration) 
        {
            var jwt = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() 
                ?? throw new InvalidOperationException("JwtSettings section is missing in appsettings.json.");

            services.AddAuthentication(options => 
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; 
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; 
            })
                .AddJwtBearer(options => 
                {
                    options.TokenValidationParameters = new TokenValidationParameters 
                    {
                        ValidateIssuer = true, 
                        ValidateAudience = true, 
                        ValidateLifetime = true, 
                        ValidateIssuerSigningKey = true, 
                        ValidIssuer = jwt.Issuer, 
                        ValidAudience = jwt.Audience, 
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret)), 
                        ClockSkew = TimeSpan.Zero 
                    };
                });

            services.AddAuthorization(); 

            return services; 
        }
    }
}
