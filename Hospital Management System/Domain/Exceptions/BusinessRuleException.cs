namespace Hospital_Management_System.Domain.Exceptions
{
    public class BusinessRuleException : Exception 
    {
        public BusinessRuleException(string message)
            : base(message) 
        {
        } 
    }
}
