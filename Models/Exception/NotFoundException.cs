namespace Vital.Models.Exception;

public class NotFoundException : AppException
{
    public NotFoundException() : base("Not found")
    {
    }
    public NotFoundException(string error) : base(error)
    {
    }
}
