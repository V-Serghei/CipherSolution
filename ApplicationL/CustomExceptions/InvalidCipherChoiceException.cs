namespace ApplicationL.CustomExceptions;

public class InvalidCipherChoiceException(string message) : ArgumentException(message);