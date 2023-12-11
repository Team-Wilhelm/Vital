namespace Models.Exception;

public abstract class AppException(string error) : System.Exception(error);
