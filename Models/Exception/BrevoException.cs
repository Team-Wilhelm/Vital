namespace Models.Exception;

public class BrevoException : AppException
{
    public BrevoException() : base("Something went wrong")
    {
    }
    public BrevoException(string error) : base(error)
    {
    }
}
