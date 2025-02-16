namespace ApplicationL.CustomExceptions;

public class InvalidKeyException(string message) : ArgumentException(message);