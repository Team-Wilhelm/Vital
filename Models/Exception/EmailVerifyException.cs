namespace Vital.Models.Exception;

public class EmailVerifyException : AppException
{
    public EmailVerifyException() : base("Token or User Id is invalid.")
    {
    }
    
    public EmailVerifyException(string message) : base(message)
    {
    }
}
