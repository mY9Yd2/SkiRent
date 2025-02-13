namespace SkiRent.Api.Exceptions;

public class UnhandledErrorException : Exception
{
    public UnhandledErrorException()
    { }

    public UnhandledErrorException(string message) : base(message)
    { }

    public UnhandledErrorException(string message, Exception inner) : base(message, inner)
    { }
}
