namespace ApplicationL.CustomExceptions;

public class InvalidTextException(string message) : ArgumentException(message);
