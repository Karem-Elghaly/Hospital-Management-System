using Hospital_Management_System.Application.Common.Models; 
namespace Hospital_Management_System.Application.Common.Interfaces
{
    public interface IIdentityService 
    {
        Task<UserInfo> CreateUserAsync(string fullName, string email, string password, string role); 

        Task<UserInfo?> ValidateCredentialsAsync(string email, string password); 

        Task<UserInfo?> GetUserByIdAsync(string userId); 

        Task DeleteUserAsync(string userId); 

        Task DeactivateUserAsync(string userId); 
   
    }


}
