namespace Hospital_Management_System.Domain.Exceptions
{
    public class DoubleBookingException : Exception 
    {
        public DoubleBookingException(string message) 
            : base(message) 
        {
        } 
    }
}
