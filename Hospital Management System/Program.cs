using Hospital_Management_System.API.Extensions; 
using HospitalManagementSystem.API.Middlewares; 
using Hospital_Management_System.Application;
using Hospital_Management_System.Infrastructure; 
using Hospital_Management_System.Infrastructure.Seed; 
using Serilog; 
using System.Security.Claims; 
using System.Text.Json.Serialization; 

Log.Logger = new LoggerConfiguration() 
    .WriteTo.Console() 
    .CreateBootstrapLogger(); 

try 
{
    var builder = WebApplication.CreateBuilder(args); 

    builder.Host.UseSerilog((context, services, configuration) => configuration 
        .ReadFrom.Configuration(context.Configuration) 
        .ReadFrom.Services(services) 
        .Enrich.FromLogContext()); 

    builder.Services.AddControllers() 
        .AddJsonOptions(options => 
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())); 

    builder.Services.AddSwaggerDocumentation(); 
    builder.Services.AddApplication(); 
    builder.Services.AddInfrastructure(builder.Configuration); 
    builder.Services.AddJwtAuthentication(builder.Configuration); 

    var app = builder.Build(); 

    await DbInitializer.SeedAsync(app.Services, app.Configuration); 

    app.UseSerilogRequestLogging(options => 
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) => 
        {
            var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier); 
            diagnosticContext.Set("UserId", userId ?? "anonymous"); 
        };
    });

    app.UseMiddleware<ExceptionHandlingMiddleware>(); 

    if (app.Environment.IsDevelopment())
    {
        app.UseSwaggerDocumentation(); 
    }

    app.UseHttpsRedirection(); 

    app.UseAuthentication();
    app.UseAuthorization(); 

    app.MapControllers(); 

    app.Run(); 
}
catch (Exception ex) when (ex is not HostAbortedException) 
{
    Log.Fatal(ex, "Application terminated unexpectedly"); 
}
finally
{
    Log.CloseAndFlush(); 
}