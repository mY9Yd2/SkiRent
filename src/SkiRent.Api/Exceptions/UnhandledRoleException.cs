namespace SkiRent.Api.Exceptions;

public class UnhandledRoleException : Exception
{
    public UnhandledRoleException()
    { }

    public UnhandledRoleException(string message) : base(message)
    { }

    public UnhandledRoleException(string message, Exception inner) : base(message, inner)
    { }
}
