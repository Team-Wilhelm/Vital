namespace Vital.Models.Exception;

public class ResetPasswordException : AppException
{
    public ResetPasswordException(string error) : base(error)
    {
    }
}
