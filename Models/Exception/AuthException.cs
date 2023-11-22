namespace Vital.Models.Exception;

public class AuthException : AppException
{
    public AuthException() : base("Something went wrong")
    {
    }
    public AuthException(string error) : base(error)
    {
    }
}
