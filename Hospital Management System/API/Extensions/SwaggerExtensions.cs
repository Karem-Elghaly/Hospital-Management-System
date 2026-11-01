using Microsoft.OpenApi.Models; 

namespace Hospital_Management_System.API.Extensions; 

public static class SwaggerExtensions 
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services) 
    {
        services.AddEndpointsApiExplorer(); 
        services.AddSwaggerGen(options => 
        {
            options.SwaggerDoc("v1", new OpenApiInfo 
            {
                Title = "Hospital Management API", 
                Version = "v1", 
                Description = "Hospital Management System built with Clean Architecture" 
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme 
            {
                Name = "Authorization", 
                Type = SecuritySchemeType.Http, 
                Scheme = "bearer", 
                BearerFormat = "JWT", 
                In = ParameterLocation.Header, 
                Description = "Paste the JWT access token only (without the word Bearer)." 
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement 
            {
                {
                    new OpenApiSecurityScheme 
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme, 
                            Id = "Bearer" 
                        }
                    },
                    Array.Empty<string>() 
                }
            });
        });

        return services; 
    }

    public static WebApplication UseSwaggerDocumentation(this WebApplication app) 
    {
        app.UseSwagger(); 
        app.UseSwaggerUI(); 

        return app; 
    }
}