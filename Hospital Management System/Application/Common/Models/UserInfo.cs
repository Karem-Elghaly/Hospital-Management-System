namespace Hospital_Management_System.Application.Common.Models; 

public record UserInfo(string Id, string Email, string FullName, IList<string> Roles); 
