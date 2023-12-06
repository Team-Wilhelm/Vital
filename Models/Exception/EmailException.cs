namespace Vital.Models.Exception;

public class EmailException : AppException
{
    public EmailException(string error) : base(error)
    {
    }
}
