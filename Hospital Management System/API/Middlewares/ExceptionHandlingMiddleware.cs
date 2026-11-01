using Hospital_Management_System.Application.Common.Exceptions;
using Hospital_Management_System.Domain.Exceptions;
using Microsoft.EntityFrameworkCore; 
using System.Text.Json;

namespace HospitalManagementSystem.API.Middlewares; 

public class ExceptionHandlingMiddleware 
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web); 

    private readonly RequestDelegate _next; 
    private readonly ILogger<ExceptionHandlingMiddleware> _logger; 

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger) 
    {
        _next = next; 
        _logger = logger; 
    }

    public async Task InvokeAsync(HttpContext context) 
    {
        try 
        {
            await _next(context); 
        }
        catch (Exception ex) 
        {
            await HandleExceptionAsync(context, ex); 
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception) 
    {
        int statusCode; 
        string message = exception.Message; 
        IDictionary<string, string[]>? errors = null; 

        switch (exception) 
        {
            case ValidationException validationException: 
                statusCode = StatusCodes.Status400BadRequest; 
                errors = validationException.Errors; 
                break; 

            case NotFoundException: 
                statusCode = StatusCodes.Status404NotFound; 
                break; 

            case UnauthorizedException: 
                statusCode = StatusCodes.Status401Unauthorized; 
                break; 

            case ForbiddenException: 
                statusCode = StatusCodes.Status403Forbidden; 
                break; 

            case DoubleBookingException: 
            case BusinessRuleException: 
                statusCode = StatusCodes.Status409Conflict; 
                break; 

            case DbUpdateException dbEx when IsUniqueViolation(dbEx):
                statusCode = StatusCodes.Status409Conflict;
                message = "This record conflicts with an existing one (it may have just been created by another request). Please refresh and try again."; // رسالة عامة تنفع لأي جدول
                break; 

            default: 
                statusCode = StatusCodes.Status500InternalServerError; 
                message = "An unexpected error occurred.";
                break; 
        }

        if (statusCode >= StatusCodes.Status500InternalServerError) 
        {
            _logger.LogError(exception, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path); 
        }
        else 
        {
            _logger.LogWarning( 
                "Handled {ExceptionType} on {Method} {Path} -> {StatusCode}: {Message}", 
                exception.GetType().Name, context.Request.Method, context.Request.Path, statusCode, message); 
        }

        var response = new 
        {
            success = false,
            message, 
            errors 
        };

        context.Response.StatusCode = statusCode; 
        context.Response.ContentType = "application/json"; 

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions)); 
    }

    private static bool IsUniqueViolation(DbUpdateException exception) 
    {
        return exception.InnerException?.Message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) == true;
    }
}