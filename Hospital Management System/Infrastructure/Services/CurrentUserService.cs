using System.Security.Claims; 
using Hospital_Management_System.Application.Common.Interfaces; 
using Microsoft.AspNetCore.Http; 

namespace Hospital_Management_System.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService 
    {
        private readonly IHttpContextAccessor _accessor; 

        public CurrentUserService(IHttpContextAccessor accessor) 
        {
            _accessor = accessor; 
        }

        public string? UserId => _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier); 

        public string? Role => _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role); 

        public bool IsAuthenticated => _accessor.HttpContext?.User.Identity?.IsAuthenticated ?? false; 
    }
}
