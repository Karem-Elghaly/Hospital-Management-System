namespace Hospital_Management_System.Application.Common.Models
{
    public class Result<T> 
    {
        public bool Success { get; private set; } 

        public string? Message { get; private set; } 

        public T? Data { get; private set; } 

        public static Result<T> Ok(T data, string? message = null) 
        {
            return new Result<T> { Success = true, Data = data, Message = message }; 
        }

        public static Result<T> Fail(string message) 
        {
            return new Result<T> { Success = false, Message = message }; 
        }
    }
}
