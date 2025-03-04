namespace SkiRent.Api.Exceptions;

public class BookingRollbackException : Exception
{
    public BookingRollbackException()
    { }

    public BookingRollbackException(string message) : base(message)
    { }

    public BookingRollbackException(string message, Exception inner) : base(message, inner)
    { }
}
