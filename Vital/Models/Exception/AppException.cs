namespace Vital.Models.Exception;

public abstract class AppException : System.Exception {
    public AppException(string error) : base(error) {

    }
}
