namespace Hospital_Management_System.Application.Common.Exceptions
{
    public class ForbiddenException : Exception 
    {
        public ForbiddenException(string message = "You do not have permission to perform this action.") 
            : base(message)
        {
        }
    }
}
